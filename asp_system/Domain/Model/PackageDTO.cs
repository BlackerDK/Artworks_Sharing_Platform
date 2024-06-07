using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class PackageDTO
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Name Package is required")]
        public string Name { get; set; }
        public int? Quantity { get; set; }
        public double? Price { get; set; }
        public bool Status { get; set; }
    }
    public class PackageAddDTO
    {
        [Required(ErrorMessage = "Name Package is required")]
        public string Name { get; set; }
        public int? Quantity { get; set; }
        public double? Price { get; set; }
        public bool Status { get; set; }
    }
}
