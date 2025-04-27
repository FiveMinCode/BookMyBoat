using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.API.Dtos;
using UserManagement.API.Mappings;
using UserManagement.API.Services;
using UserManagment.Model;

namespace UserManagement.API.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class UserManagementController : ControllerBase
    {
        private readonly ILogger<UserManagementController> _logger;
        private readonly IUserManagementRepository _userManagementRepository;
        private readonly IMapper _mapper;

        public UserManagementController(ILogger<UserManagementController> logger, IUserManagementRepository userManagementRepository,
            IMapper mapper)
        {
            _logger = logger;
            _userManagementRepository = userManagementRepository;
            _mapper = mapper;
        }

        [HttpGet("GetUsers")]
        public IEnumerable<User> GetUsers()
        {
            _logger.LogInformation("Getting users");
            return _userManagementRepository.GetUsers();
        }

        [HttpGet("GetUsersList")]
        public IEnumerable<UserDto> GetUsersList()
        {
            _logger.LogInformation("Getting GetUsersList");
            return _mapper.Map<List<UserDto>>(_userManagementRepository.GetUsersList());
        }

        [HttpGet("GetUser")]
        public UserDto GetUser(int id)
        {
            _logger.LogInformation("Getting GetUsersList");
            return _mapper.Map<UserDto>(_userManagementRepository.GetUser(id));
        }

        [HttpPost("SaveUser")]
        public bool SaveUser(User user)
        {
            _logger.BeginScope("Saving user {UserName}", user.UserName);
            return _userManagementRepository.SaveUser(user);
        }
        [Authorize]
        [HttpGet("GetBookings")]
        public IEnumerable<UserBooking> GetBookings()
        {
            _logger.LogInformation("Getting bookings");
            var ownerId = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            return _userManagementRepository.GetAll(ownerId);
        }
    }
}
