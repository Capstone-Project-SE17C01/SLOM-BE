namespace Project.Core.Entities.Business.DTOs
{
    public class APIResponse
    {
        public APIResponse()
        {
            errorMessages = new List<string>();
        }
        public List<string> errorMessages { get; set; }
        public object result { get; set; }
        public Pagination? pagination { get; set; }
    }
}
