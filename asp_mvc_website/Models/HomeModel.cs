namespace asp_mvc_website.Models
{
    public class HomeModel
    {
        public List<ArtworkModel> ArtworkList { get; set; }
        public List<CategoryModel> CategoryList { get; set; }
        public string IsPoster { get; set; } = string.Empty;
    }
}
