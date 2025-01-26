using UserManagement.Client.ViewModels;

namespace UserManagement.Client.Clients
{
    public interface IUserManagementClient
    {
        Task<List<UserBookings>> GetAll(string accessToken);
    }
}
