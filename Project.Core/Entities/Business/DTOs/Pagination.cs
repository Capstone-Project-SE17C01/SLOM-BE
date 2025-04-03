namespace Project.Core.Entities.Business.DTOs
{
    public class Pagination
    {
        public int CurrentPage { get; private set; }
        public int TotalPages { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
    }
}
