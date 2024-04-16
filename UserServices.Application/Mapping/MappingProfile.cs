using AutoMapper;
using UserServices.Application.ModelsDTO;
using UserServices.Domain.Models;

namespace UserServices.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDTO>().ReverseMap();
        }

    }
}
