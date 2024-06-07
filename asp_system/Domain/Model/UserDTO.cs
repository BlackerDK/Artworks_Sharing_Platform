using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class UserDTO
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime Birthday { get; set; }
    }

    public class UserRolesVM : UserDTO
    {
        public String UserName { get; set; }
        public String Id { get; set; }
        public String Email { get; set; }
        public Boolean IsActive { get; set; }
        public List<string> RolesName { get; set; }
    }

    public class UserRoles : ApplicationUser
    {
        public List<string> RolesName { get; set; }
    }

    public class ProfileUserDTO
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime Birthday { get; set; }
        public string Email { get; set; } = string.Empty;
        public List<Artwork_Profile> Artworks { get; set; }       
    }
    public class Artwork_Profile
    {
        public int ArtworkId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public double? Price { get; set; }
        public List<LikeDTO> LikeNumber { get; set; }
        public List<CommentDTO> CommentNumber { get; set; }
        public string Image { get; set; }  
    }

}
