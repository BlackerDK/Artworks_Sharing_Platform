using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class OrderDTO
    {
        [Key]
        [Required(ErrorMessage = "Id is required")]
        public int OrderId { get; set; }
        public DateTime Date { get; set; }
        public string? Code { get; set; }
        public int? ArtworkId { get; set; }
        public bool? ReOrderStatus { get; set; }
        public string UserId { get; set; }
    }

    public class OrderDeleteDTO
    {
        [Key]
        [Required(ErrorMessage = "Id is required")]
        public int OrderId { get; set; }
        
    }

    public class OrderUpdateDTO
    {
        [Key]
        [Required(ErrorMessage = "Id is required")]
        public int OrderId { get; set; }
        public string? Code { get; set; }
        public bool? ReOrderStatus { get; set; }
       
    }

    public class OrderCreateDTO
    {
        public string Code { get; set; }
        public int ArtworkId { get; set; }
        public bool ReOrderStatus { get; set; }
        public string UserId { get; set; }
        
    }
    
}
