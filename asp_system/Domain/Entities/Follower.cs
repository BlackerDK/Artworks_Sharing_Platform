using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Follower
    {
        //FollowerId	UserId
        [Key]
        public int FollowerId { get; set; }
        //public int? UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

    }
}
