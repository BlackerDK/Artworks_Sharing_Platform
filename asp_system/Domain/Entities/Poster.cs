using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Poster
    {
        //Id	UserId	PackageId	QuantityPost
        [Key]
        public int Id { get; set; }
        //public int? UserId { get; set; }
        public int? PackageId { get; set; }
        public int? QuantityPost { get; set; }

        public Package Package { get; set; }
        public ApplicationUser User { get; set; }

    }
}
