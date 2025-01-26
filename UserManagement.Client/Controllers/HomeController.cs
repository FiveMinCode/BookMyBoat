using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Diagnostics;
using System.Text;
using UserManagement.Client.Clients;
using UserManagement.Client.Models;

namespace UserManagement.Client.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserManagementClient _userManagementClient;

        public HomeController(ILogger<HomeController> logger, IUserManagementClient userManagementClient)
        {
            _logger = logger;
            _userManagementClient = userManagementClient;
        }

        public async Task<IActionResult> Index()
        {
            await LogIdentityInformation();
            var accessToken = await HttpContext
                .GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            return View(_userManagementClient.GetAll(accessToken).Result);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task LogIdentityInformation()
        {
            // get the saved identity token
            var identityToken = await HttpContext
                .GetTokenAsync(OpenIdConnectParameterNames.IdToken);

            // get the saved access token
            var accessToken = await HttpContext
                .GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            // get the refresh token
            var refreshToken = await HttpContext
                .GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);

            var userClaimsStringBuilder = new StringBuilder();
            foreach (var claim in User.Claims)
            {
                userClaimsStringBuilder.AppendLine(
                    $"Claim type: {claim.Type} - Claim value: {claim.Value}");
            }

            // log token & claims
            _logger.LogInformation($"Identity token & user claims: " +
                $"\n{identityToken} \n{userClaimsStringBuilder}");
            _logger.LogInformation($"Access token: " +
                $"\n{accessToken}");
            _logger.LogInformation($"Refresh token: " +
                $"\n{refreshToken}");
        }
    }
}
