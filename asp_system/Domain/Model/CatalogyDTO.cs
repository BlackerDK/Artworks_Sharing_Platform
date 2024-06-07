using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class CatalogyDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        public bool Status { get; set; }
    }
    public class CatalogyAddDTO
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        public bool Status { get; set; }
    }
}
