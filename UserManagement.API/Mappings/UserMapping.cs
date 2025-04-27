using AutoMapper;
using UserManagement.API.Dtos;
using UserManagment.Model;

namespace UserManagement.API.Mappings
{
    public class UserMapping : Profile
    {
        public UserMapping()
        {
            // Define your mappings here
            CreateMap<User, UserDto>()
                .ForMember(t => t.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(t => t.Name, opt => opt.MapFrom(src => src.UserName))
                .ForMember(t => t.Email, opt => opt.MapFrom(src => src.Profile.Email))
                .ForMember(t => t.Phone, opt => opt.MapFrom(src => src.Profile.PhoneNumber));
            CreateMap<UserDto, User>();
        }
    }
}
