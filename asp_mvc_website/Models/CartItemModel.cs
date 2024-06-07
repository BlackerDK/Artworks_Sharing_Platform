using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace asp_mvc_website.Models
{
    public class CartItemModel
    {
        public int artworkId { get; set; }
        public string Title { get; set; } = string.Empty;
        public double Price { get; set; }
        public string Image { get; set; } = string.Empty;
		public ArtWorkStatus Status { get; set; }

		[JsonProperty("ImageUrl")]
        private List<string> ImageUrl { get; set; } = new List<string>();

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (ImageUrl != null && ImageUrl.Count > 0)
            {
                Image = ImageUrl[0];
            }
        }
    }

    public class CartResponseVM
	{
        public int total { get; set; } 
        public List<CartItemVM> data { get; set; }
	}
    public class CartItemVM
    {
        public string Id {  get; set; }
		public int ArtWorkId { get; set; }
		public string Title { get; set; } = string.Empty;
		public double Price { get; set; }
		public string ArtWorkImage { get; set; } = string.Empty;
        public ArtInCartVM ArtWork { get; set; }
	}

    public class ArtInCartVM
	{
        public ArtWorkStatus Status {  get; set; }
        public String Description { get; set; }

	}

    public class AddToCartCM
    {
        public int ArtworkId { get; set; }
        public string UserId { get; set; }
    }
}
