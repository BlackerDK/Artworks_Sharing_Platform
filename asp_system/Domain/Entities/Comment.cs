using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
	public class Comment
	{
		[Key]
		public int Id { get; set; }
		public string Content { get; set; }
		public Artwork Artwork { get; set; }
		public ApplicationUser User { get; set; }
	}
}
