namespace asp_mvc_website.Models
{
    public class CategoryModel
    {
        public int id { get; set; }
        public string Name { get; set; }
        public bool Status { get; set; }
    }
    public class CategoryAddDTO
    {
        public string Name { get; set; }
        public bool Status { get; set; }
    }
    public class ResponseCategoryDTO
    {
        public string Message { get; set; } = string.Empty;
        public bool IsSuccess { get; set; }
        public CategoryModel Data { get; set; } = null;

    }

}
