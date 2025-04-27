using Microsoft.EntityFrameworkCore;
using UserManagement.API.DbContexts;
using UserManagment.Model;

namespace UserManagement.API.Services
{
    public class UserManagementRepository : IUserManagementRepository
    {
        private readonly UserManagementContext _context;

        public UserManagementRepository(UserManagementContext context)
        {
            _context = context;
        }
        public List<UserBooking> GetAll(string ownerId)
        {
            return _context.UserBookings.Where(t => t.User.Id.ToString() == ownerId).ToList();
        }

        public List<User> GetUsers()
        {
            return _context.Users.ToList();
        }

        public User GetUser(int id)
        {
            return _context.Users.Include(t => t.Profile).FirstOrDefault(t => t.Id == id) ?? new User();
        }

        public List<User> GetUsersList()
        {
            return _context.Users.Include(t => t.Profile).ToList();
        }

        public bool SaveUser(User user)
        {
            try
            {
                var profile = new UserProfile { Name = user.UserName, Email = user.UserName + "@User.com", PhoneNumber = "5435435534" };
                _context.UserProfiles.Add(profile);
                user.Profile = profile;
                _context.Users.Add(user);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            //var maxId = _context.Users.Max(x => x.Id);
            //user.Id = maxId + 1;
            //var profile = new UserProfile { Name = user.UserName, Email = user.UserName + "@User.com", PhoneNumber = "5435435534"};
            //_context.UserProfiles.Add(profile);
            //user.Profile = profile;
            //_context.Users.Add(user);
            //return true;
        }
    }
}
