using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class LikeDTO
    {
        [Key]
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }
        public int? ArtworkId { get; set; }
        public string UserId { get; set; }
    }
    public class LikeDeleteDTO
    {
        [Key]
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }

    }
    public class LikeCreateDTO
    {   
        public int? ArtworkId { get; set; }
        public string UserId { get; set; }
    }
}
