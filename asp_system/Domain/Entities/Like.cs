using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Like
    {
        //Id	ArtworkId	UserId
        [Key]
        public int Id { get; set; }
        //public int? ArtworkId { get; set; }
        //public int? UserId { get; set; }

        public Artwork Artwork { get; set; }
        public ApplicationUser User { get; set; }
    }
}
