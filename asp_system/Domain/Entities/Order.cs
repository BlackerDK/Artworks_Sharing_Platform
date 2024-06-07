using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Order
    {
        //OrderId	UserId	Date	ReOrderStatus	ArtworkId	Code
        [Key]
        public int OrderId { get; set; }
        //public int? UserId { get; set; }
        public DateTime? Date { get; set; }
        public bool? ReOrderStatus { get; set; }
        public int? ArtworkId { get; set; }
        public required string Code { get; set; }

        public string UserId {  get; set; } 
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
        public Artwork Artwork { get; set; }   

    }
}
