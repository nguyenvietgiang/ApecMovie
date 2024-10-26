using ApecMovieCore.Common;
using BlogServices.Models;

namespace BlogServices.Common
{
    public class Genarater
    {
        private readonly ApplicationDbContext _dataContext;
        public Genarater(ApplicationDbContext dataContext)
        {
            _dataContext = dataContext;
        }
        public string GenerateUniqueSlug<T>(string stringName, Func<T, string> slugPropertySelector) where T : class
        {
            string slug = Generator.GenerateSlug(stringName);
            string originalSlug = slug;
            int counter = 1;

            while (_dataContext.Set<T>().Any(p => slugPropertySelector(p) == slug))
            {
                slug = $"{originalSlug}-{counter++}";
            }

            return slug;
        }

    }
}
