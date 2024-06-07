using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
	public class CommentDTO
	{
		[Key]
		[Required(ErrorMessage = "Id is required")]
		public int Id { get; set; }
		public string Content { get; set; }
		public int? ArtworkId { get; set; }
		public string UserId { get; set; }
	}
	public class CommentAddDTO
	{
		public string Content { get; set; }
		public int? ArtworkId { get; set; }
		public string UserId { get; set; }
	}
	public class CommentUpdateDTO
	{
		public int Id { get; set; }
		public string Content { get; set; }
	}
}
