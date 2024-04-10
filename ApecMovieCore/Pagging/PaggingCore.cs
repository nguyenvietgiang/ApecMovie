using Microsoft.AspNetCore.Http;

namespace ApecMovieCore.Pagging
{
    public class PaggingCore<T>
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public IEnumerable<T> Content { get; set; }
        public string PreviousPageUrl { get; set; }
        public string NextPageUrl { get; set; }
        public PaggingCore(IEnumerable<T> content, int totalRecords, int currentPage, int pageSize, IHttpContextAccessor httpContextAccessor, string serviceRoute)
        {
            Content = content;
            TotalRecords = totalRecords;
            CurrentPage = currentPage;
            PageSize = pageSize;
            CalculateTotalPages();
            CalculatePreviousAndNextPages(httpContextAccessor, serviceRoute);
        }

        private void CalculateTotalPages()
        {
            TotalPages = (int)Math.Ceiling((double)TotalRecords / PageSize);
        }

        private void CalculatePreviousAndNextPages(IHttpContextAccessor httpContextAccessor, string serviceRoute)
        {
            var baseUrl = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}";
            int previousPage = CurrentPage - 1;
            int nextPage = CurrentPage + 1;

            PreviousPageUrl = previousPage > 0 ? $"{baseUrl}{serviceRoute}?currentPage={previousPage}&pageSize={PageSize}" : null;
            NextPageUrl = nextPage <= TotalPages ? $"{baseUrl}{serviceRoute}?currentPage={nextPage}&pageSize={PageSize}" : null;
        }
    }
}

