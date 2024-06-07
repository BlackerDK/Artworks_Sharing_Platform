namespace asp_mvc_website.Models
{
    public class PosterModel
    {
        public int? PackageId { get; set; }
        public int? QuantityPost { get; set; }
    }
    public class PosterModelAdd
    {
        public int? PackageId { get; set; }
        public string UserId { get; set; }
    }
}
