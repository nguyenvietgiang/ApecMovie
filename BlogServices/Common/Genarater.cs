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

            // Tải toàn bộ dữ liệu vào bộ nhớ và sau đó thực hiện kiểm tra slug
            var existingSlugs = _dataContext.Set<T>().AsEnumerable().Select(slugPropertySelector).ToList();

            while (existingSlugs.Any(existingSlug => existingSlug == slug))
            {
                slug = $"{originalSlug}-{counter++}";
            }

            return slug;
        }


    }
}
