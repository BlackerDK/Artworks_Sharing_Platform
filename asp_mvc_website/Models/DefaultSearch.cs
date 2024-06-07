namespace asp_mvc_website.Models
{
	public class DefaultSearch
	{
		public int perPage { get; set; } = 10;
		public int currentPage { get; set; } = 0;
		public string sortBy { get; set; } = string.Empty;
		public bool isAscending { get; set; }
	}
}
