using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using MovieServices.Application.ModelsDTO;
using MovieServices.Domain.Models;


namespace MovieServices.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Movie, MovieDTO>()
                .ForMember(dest => dest.Image, opt => opt.Ignore());

            CreateMap<MovieDTO, Movie>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => true));

            CreateMap(typeof(JsonPatchDocument<MovieDTO>), typeof(JsonPatchDocument<Movie>))
                .ConstructUsing(src => src);

            CreateMap<JsonPatchDocument<MovieDTO>, JsonPatchDocument<Movie>>().ConvertUsing<JsonPatchDocumentConverter>();
    }
    }

}
