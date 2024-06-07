using API.Helper;
using Domain.Entities;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IUserNotificationService
    {
        Task<IEnumerable<GetUserNotificationDTO>> GetNotificationByUserId(string userid);
        Task<ResponseDTO> CreateUserNotification(CreateUserNotificationDTO noti);
        Task<ResponseDTO> RemoveUserNotification(int id);
        Task<ResponseDTO> RemoveAllUserNotificationsByUserId(string userId);
        Task<ResponseDTO> AddNoticationForAdmin(CreateAdminNotificationDTO noti);
        Task<List<GetUserNotificationDTO1>> GetNotiSortResultAsync(string userId, DefaultSearch defaultSearch);
        int totalGetNotiUserSortResult(string userId);

    }
}
