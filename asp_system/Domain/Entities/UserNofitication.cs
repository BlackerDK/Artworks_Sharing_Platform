using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class UserNofitication
    {
        //Id	NotificationId	ArtworkId	UserId
        [Key]
        public int Id { get; set; }
        public int? NotificationId { get; set; }
        public int? ArtworkId { get; set; }
        public string? UserIdFor { get; set; }

        public virtual Notification Notification { get; set; }
        public virtual Artwork Artwork { get; set; }
        public virtual ApplicationUser User { get; set; }

    }
}
