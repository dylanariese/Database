namespace Database.Models.Models
{
    public class SearchPagingViewModel
    {
        public int Page { get; set; }
        public int Size { get; set; }
        public string SearchTerm { get; set; } = null;
    }
}