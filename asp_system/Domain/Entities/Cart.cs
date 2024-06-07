using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
	public class Cart
	{
		public Guid Id { get; set; }
		public string Title { get; set;}
		public double Price {  get; set; }
		public string ArtWorkImage { get; set; }
		[ForeignKey("ArtWorkId")]

		public int ArtWorkId { get; set; }
		[ForeignKey("UserId")]
		public string UserId { get; set; }
		public virtual ICollection<ApplicationUser> Users { get; set; }
		public virtual ICollection<Artwork> Artworks { get; set; }

	}
}
