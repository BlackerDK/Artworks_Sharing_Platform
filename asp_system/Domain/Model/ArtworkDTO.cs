using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class ArtworkDTO
    {
        [Key]
        [Required(ErrorMessage = "Id is required")]
        public int ArtworkId { get; set; }
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }
        public string Description { get; set; }
        [Required(ErrorMessage = "Price is required")]
        public double? Price { get; set; }
        public string? UserId { get; set; }
        public DateTime? CreateOn { get; set; }
        public DateTime? UpdateOn { get; set; }
        public ArtWorkStatus Status { get; set; }
        public int? ReOrderQuantity { get; set; }
        public List<string> ImageUrl { get; set; }
        public List<int?> Categories { get; set; }
    }

    public class ArtworkUpdateDTO
    {
        [Required(ErrorMessage = "Id is required")]
        public int ArtworkId { get; set; }

        //[Required(ErrorMessage = "Title is required")]
        //public string Title { get; set; }
        //public string Description { get; set; }
        //[Required(ErrorMessage = "Price is required")]
        //public double? Price { get; set; }
        public ArtWorkStatus Status { get; set; }
        //public int? ReOrderQuantity { get; set; }
    }
    public class ArtworkAddDTO
    {
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        public double? Price { get; set; }
        public string UserId { get; set; }
        public int? ReOrderQuantity { get; set; }

        public List<string> ImagesUrl { get; set; }

        public List<int> CategoryIds { get; set; }
    }

    public class ArtworkImageDTO
    {
        public int ArtworkId { get; set; }
        public string ImageUrl { get; set; }
    }
}
