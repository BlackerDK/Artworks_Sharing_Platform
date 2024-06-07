namespace asp_mvc_website.Models
{
    public class ApplicationUserModel
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime Birthday { get; set; }
        public bool Gender { get; set; }
        public bool Status { get; set; }
        public bool IsActive { get; set; } = false;
    }
}
