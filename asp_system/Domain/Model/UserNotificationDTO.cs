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
    public class UserNotificationDTO
    {
        [Key]
        public int Id { get; set; }
        public int? NotificationId { get; set; }
        public int? ArtworkId { get; set; }
        public string UserId { get; set; }
        public virtual Notification Notification { get; set; }
        public virtual Artwork Artwork { get; set; }
        public virtual ApplicationUser User { get; set; }
    }

    public class CreateUserNotificationDTO
    {
        [Required(ErrorMessage = "userID is required")]
        public string? userId { get; set; }
        [Required(ErrorMessage = "Title is required")]
        public int? NotificationId { get; set; }
        [Required(ErrorMessage = "Title is required")]
        public int? ArtworkId { get; set; } 
        public string? UserIdFor {  get; set; }
    }
    public class CreateAdminNotificationDTO
    {
        [Required(ErrorMessage = "Title is required")]
        public int? NotificationId { get; set; }
        [Required(ErrorMessage = "Title is required")]
        public int? ArtworkId { get; set; }
    }
    public class GetUserNotificationDTO
    {
        public int Id { get; set; }
        public string? ArtworkTitle { get; set; }
        public string? NotificationTitle { get; set; }
        public string? NotificationDescription { get; set; }
        public bool? isRead { get; set; }
        public DateTime dateTime { get; set; }
        public NotiStatus notiStatus { get; set; }
        public string? nameUser { get; set; }
        public string artwordUrl { get; set; }
        public int artworkId { get; set; }
        public int notificationId { get; set; }
    }

    public class UserVM
    {
        public String LastName { get; set; }
        public String FirstName { get; set; }

    }

    public class ArtWorkVM
    {
        public String ArtworkId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
		public double? Price { get; set; }
		public DateTime? CreateOn { get; set; }
		public string UserId { get; set; }

    }

    public class ArtWorkImageVM
    {
        public string Image { get; set; }

    }

    public class NotificationVM
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public bool? IsRead { get; set; }
        public NotiStatus notiStatus { get; set; }
    }

    public class GetUserNotificationDTO1
    {
        public DateTime Date { get; set; }
        public string? UserIdFor {  get; set; }
        public UserVM UserVM {  get; set; }
        public ArtWorkVM ArtWorkVM { get; set; }
        public ArtWorkImageVM ArtWorkImageVM { get; set; }
        public NotificationVM NotificationVM { get; set; }
    }


}
