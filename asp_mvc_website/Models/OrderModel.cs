using System.ComponentModel.DataAnnotations;

namespace asp_mvc_website.Models
{
    public class OrderModel
    {
            public int OrderId { get; set; }
            public DateTime Date { get; set; }
            public string? Code { get; set; }
            public int? ArtworkId { get; set; }
            public bool? ReOrderStatus { get; set; }
            public string UserId { get; set; }
    }

    public class OrderCM
    {
		public string? Code { get; set; }
		public int? ArtworkId { get; set; }
		public bool? ReOrderStatus { get; set; }
		public string UserId { get; set; }
	}
}
