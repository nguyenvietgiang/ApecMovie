using AutoMapper;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.JsonPatch;
using MovieServices.Application.ModelsDTO;
using MovieServices.Domain.Models;


namespace MovieServices.Application.Mapping
{
    public class JsonPatchDocumentConverter : ITypeConverter<JsonPatchDocument<MovieDTO>, JsonPatchDocument<Movie>>
    {
        public JsonPatchDocument<Movie> Convert(JsonPatchDocument<MovieDTO> source, JsonPatchDocument<Movie> destination, ResolutionContext context)
        {
            var result = new JsonPatchDocument<Movie>();

            foreach (var op in source.Operations)
            {
                switch (op.path.ToLower())
                {
                    case "/title":
                        result.Operations.Add(new Operation<Movie>("replace", op.path, (string)op.value));
                        break;
                    case "/category":
                        result.Operations.Add(new Operation<Movie>("replace", op.path, (string)op.value));
                        break;
                    case "/description":
                        result.Operations.Add(new Operation<Movie>("replace", op.path , (string)op.value));
                        break;
                    case "/director":
                        result.Operations.Add(new Operation<Movie>("replace", op.path, (string)op.value));
                        break;
                    default:
                        break;
                }
            }

            return result;
        }
    }
}
