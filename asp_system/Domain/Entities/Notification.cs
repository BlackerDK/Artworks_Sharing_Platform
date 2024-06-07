using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Notification
    {
        //Id	NofiticationId	Title	Description	Date	IsRead
        [Key]
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty; 
        public DateTime Date { get; set; }
        public bool? IsRead { get; set; }
        public NotiStatus notiStatus { get; set; }
        public virtual ICollection<UserNofitication> UserNofitications { get; set;}
        
    }
}
