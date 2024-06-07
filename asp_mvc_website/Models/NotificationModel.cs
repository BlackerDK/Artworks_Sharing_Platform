using asp_mvc_website.Enums;

namespace asp_mvc_website.Models
{
    public class NotificationModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime? Date { get; set; }
        public bool? IsRead { get; set; }
    }

    public class createNotificationModel
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public NotiStatus notiStatus { get; set; }
    }
    public class ResponseNotificationDTO
    {
        public string Message { get; set; } = string.Empty;
        public bool IsSuccess { get; set; }
        public NotificationModel Data { get; set; } = null;

    }

	public class UpdateNotiStatusDTO
	{
		public int Id { get; set; }
		public NotiStatus notiStatus { get; set; }
	}
}
