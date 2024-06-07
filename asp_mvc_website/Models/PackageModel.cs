namespace asp_mvc_website.Models
{
    public class PackageModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? Quantity { get; set; }
        public double? Price { get; set; }
        public bool Status { get; set; }
    }
}
