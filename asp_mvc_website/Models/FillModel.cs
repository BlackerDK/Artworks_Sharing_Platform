namespace asp_mvc_website.Models
{
    public class FillModel
    {
        public string? txtSearch { get; set; }
        public double? MinPrice { get; set; }
        public double? MaxPrice { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
