using Microsoft.AspNetCore.Mvc.Rendering;

namespace asp_mvc_website.Models
{
    public class UserModel
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime Birthday { get; set; }

    }

    public class UserRolseResponse
    {
        public int total { get; set; }
        public int page { get; set; }
        public List<UserRolesVM> data { get; set; }
    }
    public class UserRolesVM
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime Birthday { get; set; }
        public String UserName { get; set; }
        public String Id { get; set; }
        public String Email { get; set; }
        public Boolean IsActive { get; set; }
        public string[] RolesName { get; set; }
    }

    public class RolesNameVM
    {
        public string RoleName { get; set; }    
        public SelectList ListRole { get; set; }  
    }

    public class RolesVM
    {
        public string id { get; set; }
        public string name { get; set; }
    }
    public class UserRolesCM
    {
        public string userId { get; set; }
        public string[] roleName { get; set; }
    }

    public class UserCM
    {
        public string userId { get; set; }
    }
}
