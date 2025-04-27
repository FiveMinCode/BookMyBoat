using UserManagment.Model;

namespace UserManagement.API.Services
{
    public interface IUserManagementRepository
    {
        List<UserBooking> GetAll(string ownerId);
        List<User> GetUsers();
        User GetUser(int id);
        List<User> GetUsersList();
        bool SaveUser(User user);
    }
}
