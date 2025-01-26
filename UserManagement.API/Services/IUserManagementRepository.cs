using UserManagment.Model;

namespace UserManagement.API.Services
{
    public interface IUserManagementRepository
    {
        List<UserBooking> GetAll(string ownerId);
        List<User> GetUsers();
        bool SaveUser(User user);
    }
}
