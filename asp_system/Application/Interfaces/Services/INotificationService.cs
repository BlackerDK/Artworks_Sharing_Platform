using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface INotificationService
    {
        Task<NotificationDTO> GetNotificationById(int id);
        Task<IEnumerable<NotificationDTO>> GetAllNotification();
        Task<ResponseDTO> CreateNotification(CreateNotificationDTO noti);
        Task<ResponseDTO> RemoveNotification(int id);
        Task<ResponseDTO> MarkReadNoti(int id);

        Task<ResponseDTO> UpdateStatusNoti(UpdateNotiStatusDTO dto);
        
    }
}
