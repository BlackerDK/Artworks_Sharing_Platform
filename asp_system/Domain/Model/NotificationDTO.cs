using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class NotificationDTO
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime? Date { get; set; }
        public bool? IsRead { get; set; }
        public NotiStatus notiStatus { get; set; }
    }

    public class CreateNotificationDTO
    {
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; } = string.Empty;
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; } = string.Empty;
        public NotiStatus notiStatus { get; set; }
    }
    public class UpdateNotiStatusDTO
    {
        public int Id { get; set; }
		public NotiStatus notiStatus { get; set; }
	}
}
