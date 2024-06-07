using System.ComponentModel.DataAnnotations;

namespace asp_mvc_website.Models
{
    public class PostArtworkModel
    {
        public IFormFile File { get; set; }
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Invalid price format.")]
        public decimal Price { get; set; }
        public string Category { get; set; }
        public bool IsNewCategory { get; set; }

    }
    public class PostArtworkDTO
    {
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; }

        public double Price { get; set; }
        public string UserId { get; set; }
        public int ReOrderQuantity { get; set; }

        public List<string> ImagesUrl { get; set; }

        public List<int> CategoryIds { get; set; }
    }
}
