using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Composition.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CompositionController : ControllerBase
    {

        //  Aggregator Pattern
        private readonly HttpClient _httpClient;

        public CompositionController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        // GET /Composition/boat-info?boatId=123
        [HttpGet("boat-info")]
        public async Task<IActionResult> GetBoatInfo([FromQuery] string boatId)
        {
            if (string.IsNullOrEmpty(boatId))
            {
                return BadRequest(new { error = "Boat ID is required" });
            }

            try
            {
                // URLs for individual microservices
                var boatServiceUrl = $"http://localhost:5001/boats/{boatId}";
                var pricingServiceUrl = $"http://localhost:5002/pricing/{boatId}";
                var reviewsServiceUrl = $"http://localhost:5003/reviews/{boatId}";

                // Send parallel requests to microservices
                var boatTask = _httpClient.GetAsync(boatServiceUrl);
                var pricingTask = _httpClient.GetAsync(pricingServiceUrl);
                var reviewsTask = _httpClient.GetAsync(reviewsServiceUrl);

                await Task.WhenAll(boatTask, pricingTask, reviewsTask);

                // Read responses
                var boatDetails = await boatTask.Result.Content.ReadAsStringAsync();
                var pricingDetails = await pricingTask.Result.Content.ReadAsStringAsync();
                var reviews = await reviewsTask.Result.Content.ReadAsStringAsync();

                // Aggregate data
                var aggregatedResponse = new
                {
                    BoatDetails = boatDetails,
                    PricingDetails = pricingDetails,
                    Reviews = reviews
                };

                return Ok(aggregatedResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to retrieve data", details = ex.Message });
            }
        }

        //GET /proxy/forward?serviceUrl=http://localhost:5001&endpoint=boats
        [HttpGet("forward")]
        public async Task<IActionResult> ForwardRequest(
            [FromQuery] string serviceUrl,
            [FromQuery] string endpoint)
        {
            if (string.IsNullOrEmpty(serviceUrl) || string.IsNullOrEmpty(endpoint))
            {
                return BadRequest(new { error = "Service URL and Endpoint are required" });
            }

            try
            {
                // Forward the request to the target microservice
                var targetUrl = $"{serviceUrl}/{endpoint}";
                var response = await _httpClient.GetAsync(targetUrl);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return Ok(new
                    {
                        data = responseContent,
                        message = "Request forwarded successfully"
                    });
                }
                else
                {
                    return StatusCode((int)response.StatusCode, new
                    {
                        error = "Failed to retrieve data from the service",
                        statusCode = response.StatusCode
                    });
                }
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, new { error = "Internal Server Error", details = ex.Message });
            }
        }

        //Chained Pattern
        [HttpPost("book-boat")]
        public async Task<IActionResult> BookBoat([FromBody] BookingRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.UserId) || string.IsNullOrEmpty(request.BoatId))
            {
                return BadRequest(new { error = "Invalid booking request" });
            }

            try
            {
                // Step 1: Validate booking
                var validateResponse = await _httpClient.PostAsJsonAsync("http://localhost:5001/validate", request);
                if (!validateResponse.IsSuccessStatusCode)
                {
                    return BadRequest(new { error = "Validation failed" });
                }

                // Step 2: Process payment
                var paymentResponse = await _httpClient.PostAsJsonAsync("http://localhost:5002/payment", request);
                if (!paymentResponse.IsSuccessStatusCode)
                {
                    return BadRequest(new { error = "Payment processing failed" });
                }

                // Step 3: Confirm booking
                var confirmResponse = await _httpClient.PostAsJsonAsync("http://localhost:5003/confirm", request);
                if (!confirmResponse.IsSuccessStatusCode)
                {
                    return BadRequest(new { error = "Booking confirmation failed" });
                }

                // Step 4: Send notification
                var notifyResponse = await _httpClient.PostAsJsonAsync("http://localhost:5004/notify", request);
                if (!notifyResponse.IsSuccessStatusCode)
                {
                    return BadRequest(new { error = "Notification sending failed" });
                }

                // Aggregate the final response
                return Ok(new { message = "Booking completed successfully" });
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, new { error = "Internal Server Error", details = ex.Message });
            }
        }

        //GET /api/branch/boat-info?boatId=123
        // Branch Pattern
        [HttpGet("boat-information")]
        public async Task<IActionResult> GetBoatInformation([FromQuery] string boatId)
        {
            if (string.IsNullOrEmpty(boatId))
            {
                return BadRequest(new { error = "Boat ID is required" });
            }

            try
            {
                // URLs for individual microservices
                var boatServiceUrl = $"http://localhost:5001/boats/{boatId}";
                var pricingServiceUrl = $"http://localhost:5002/pricing/{boatId}";
                var reviewsServiceUrl = $"http://localhost:5003/reviews/{boatId}";

                // Parallel requests to services
                var boatTask = _httpClient.GetAsync(boatServiceUrl);
                var pricingTask = _httpClient.GetAsync(pricingServiceUrl);
                var reviewsTask = _httpClient.GetAsync(reviewsServiceUrl);

                await Task.WhenAll(boatTask, pricingTask, reviewsTask);

                // Retrieve responses
                var boatDetails = await boatTask.Result.Content.ReadAsStringAsync();
                var pricingDetails = await pricingTask.Result.Content.ReadAsStringAsync();
                var reviews = await reviewsTask.Result.Content.ReadAsStringAsync();

                // Aggregate responses
                var aggregatedResponse = new
                {
                    BoatDetails = JsonSerializer.Deserialize<object>(boatDetails),
                    PricingDetails = JsonSerializer.Deserialize<object>(pricingDetails),
                    Reviews = JsonSerializer.Deserialize<object>(reviews)
                };

                return Ok(aggregatedResponse);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, new { error = "Internal Server Error", details = ex.Message });
            }
        }
    }

    // BookingRequest model
    public class BookingRequest
    {
        public string UserId { get; set; }
        public string BoatId { get; set; }
        public string PaymentDetails { get; set; }
    }
}
