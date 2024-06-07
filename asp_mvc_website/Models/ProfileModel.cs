namespace asp_mvc_website.Models
{
	public class ProfileModel
	{
       
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime Birthday { get; set; }
        public string Email { get; set; }
        public List<ProfileArt> artworks { get; set; }
        public string UserId { get; set; } = string.Empty;
        public PosterModel poster { get; set; }		
	}
}
