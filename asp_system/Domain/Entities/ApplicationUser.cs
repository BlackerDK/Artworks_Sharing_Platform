using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ApplicationUser: IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime Birthday { get; set; }
        public bool Gender { get; set; }
        public bool Status { get; set; }
        public bool IsActive { get; set; } = false;
        public String? RefreshToken { get; set; }
        public DateTime? DateExpireRefreshToken { get; set; }

        public virtual ICollection<UserNofitication> UserNofitications { get; set;}
        public virtual ICollection<Artwork> Artworks { get; set; }
        public virtual ICollection<Poster> Posters { get; set; }
        public virtual ICollection<Follower> Followers { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual Cart? Cart { get; set; } 
    }
}
