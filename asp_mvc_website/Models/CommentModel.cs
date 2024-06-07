namespace asp_mvc_website.Models
{
    public class CommentModel
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int? ArtworkId { get; set; }
        public string UserId { get; set; }
    }
}
