using AutoMapper;

namespace RESTful_Books_API.Profiles
{
    public class AdminProfile : Profile
    {
        public AdminProfile()
        {
            CreateMap<DTO.Admin.CreateAdminDto, Models.AdminModel>()
                .ForMember(dest => dest.PasswordHash, src => src.Ignore());
        }
    }
}
