using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class PosterDTO
    {
        public int Id { get; set; }
        public int? PackageId { get; set; }
        public int? QuantityPost { get; set; }
        public string UserId { get; set; }
    }
    public class PosterAddDTO
    {
        public int? PackageId { get; set; }
        public string UserId { get; set; }
    }
}
