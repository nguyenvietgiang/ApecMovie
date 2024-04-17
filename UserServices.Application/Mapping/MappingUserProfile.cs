using AutoMapper;
using UserServices.Application.ModelsDTO;
using UserServices.Domain.Models;

namespace UserServices.Application.Mapping
{
    public class MappingUserProfile : Profile
    {
        public MappingUserProfile()
        {
            CreateMap<User, UserDTO>().ReverseMap();
        }

    }
}
