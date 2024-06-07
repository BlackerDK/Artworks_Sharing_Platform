using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
	public class CartDTO
	{
		public Guid Id { get; set; }
		public string Title { get; set; }
		public double Price { get; set; }
		public string ArtWorkImage { get; set; }
		[ForeignKey("ArtWorkId")]

		public int ArtWorkId { get; set; }
		[ForeignKey("UserId")]
		public string UserId { get; set; }
		public virtual ApplicationUser User { get; set; }
		public virtual Artwork Artwork { get; set; }
	}

	public class CartCM
	{
		public int ArtWorkId { get; set; }
		public string UserId { get; set; }
	}
}
