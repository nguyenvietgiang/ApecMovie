namespace ApecMoviePortal.Models
{
    public class PaginatedResponse<T>
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public List<T> Content { get; set; }
        public string PreviousPageUrl { get; set; }
        public string NextPageUrl { get; set; }
    }

}
