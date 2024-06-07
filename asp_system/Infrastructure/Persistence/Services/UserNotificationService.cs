using API.Helper;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Model;
using Firebase.Auth;
using Infrastructure.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Services
{
    public class UserNotificationService : IUserNotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        public UserNotificationService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<ResponseDTO> CreateUserNotification(CreateUserNotificationDTO noti)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(noti.userId);

                if (user == null)
                {
                    return await Task.FromResult(new ResponseDTO { IsSuccess = false, Message = "User not found" });
                }

                var newUserNotification = new UserNofitication
                {
                    
                    ArtworkId = noti.ArtworkId,
                    NotificationId = noti.NotificationId,
                    User = user,
                    UserIdFor = noti.UserIdFor// Assuming User property is related to ApplicationUser in UserNotification
                };

                _unitOfWork.Repository<UserNofitication>().AddAsync(newUserNotification);
                _unitOfWork.Save();  // Assuming SaveAsync is an asynchronous method

                return await Task.FromResult(new ResponseDTO { IsSuccess = true, Message = "Notification added successfully", Data = noti });
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new ResponseDTO { IsSuccess = false, Message = ex.Message });
            }
        }


        public async Task<ResponseDTO> RemoveAllUserNotificationsByUserId(string userId)
        {
            try
            {
                var userNotifications = await _unitOfWork.Repository<UserNofitication>()
                    .GetQueryable()
                    .Where(noti => noti.User.Id == userId)
                    .ToListAsync();

                if (userNotifications == null || !userNotifications.Any())
                {
                    return new ResponseDTO { IsSuccess = false, Message = "No UserNotifications found for the specified UserId" };
                }

                foreach (var userNotification in userNotifications)
                {
                    _unitOfWork.Repository<UserNofitication>().DeleteAsync(userNotification);
                }
                _unitOfWork.Save();

                return new ResponseDTO { IsSuccess = true, Message = "All UserNotifications deleted successfully" };
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                return new ResponseDTO { IsSuccess = false, Message = ex.Message };
            }
        }

        public async Task<IEnumerable<GetUserNotificationDTO>> GetNotificationByUserId(string userId)
        {
            var userNotifications = await _unitOfWork.Repository<UserNofitication>()
            .GetQueryable()
            .Where(noti => noti.User.Id == userId)
            .Include(x=> x.User).ThenInclude(x=>x.Orders)
            .Include(x => x.Artwork).ThenInclude(x=>x.ArtworkImages)
            .Include(x => x.Notification)
            .ToListAsync();
            var userNotificationDTOs = userNotifications.Select(notification => new GetUserNotificationDTO
            {
                Id = notification.Id,
                ArtworkTitle = notification.Artwork?.Title,
                NotificationTitle = notification.Notification?.Title,
                NotificationDescription = notification.Notification?.Description,
                isRead = notification.Notification.IsRead,
                nameUser = notification.User.LastName + " " + notification.User.FirstName,
                dateTime = notification.Notification.Date,
                notiStatus = notification.Notification.notiStatus,
                artwordUrl = notification.Artwork.ArtworkImages.FirstOrDefault().Image,
                artworkId = notification.ArtworkId.Value,
                notificationId = notification.NotificationId.Value
            });
            return userNotificationDTOs;
        }



        public async Task<ResponseDTO> RemoveUserNotification(int id)
        {          
            try
            {
                var userNotification = await _unitOfWork.Repository<UserNofitication>().GetByIdAsync(id);

                if (userNotification == null)
                {
                    return new ResponseDTO { IsSuccess = false, Message = "UserNotification not found" };
                }

                await _unitOfWork.Repository<UserNofitication>().DeleteAsync(userNotification);
                _unitOfWork.Save(); // Assuming SaveAsync is an asynchronous method, if not, use Save()

                return new ResponseDTO { IsSuccess = true, Message = "UserNotification deleted successfully" };
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                return new ResponseDTO { IsSuccess = false, Message = ex.Message };
            }
        }

        public async Task<ResponseDTO> AddNoticationForAdmin(CreateAdminNotificationDTO noti)
        {
            var userIdAdmin = await _userManager.GetUsersInRoleAsync(AppRole.Admin);
                foreach (var item in userIdAdmin)
                {
                    var user = await _userManager.FindByIdAsync(item.Id);
                    var newUserNotification = new UserNofitication
                    {
                        ArtworkId = noti.ArtworkId,
                        NotificationId = noti.NotificationId,
                        User = user  // Assuming User property is related to ApplicationUser in UserNotification
                    };

                _unitOfWork.Repository<UserNofitication>().AddAsync(newUserNotification);
                _unitOfWork.Save();  // Assuming SaveAsync is an asynchronous method

                }

            return await Task.FromResult(new ResponseDTO { IsSuccess = true, Message = "Notification added successfully"});
            
            
        }

        public Task<List<GetUserNotificationDTO1>> GetNotiSortResultAsync(string userid, DefaultSearch defaultSearch)
        {
            var userNotifications = _unitOfWork.Repository<UserNofitication>()
            .GetQueryable()
            .Where(noti => noti.User.Id == userid)
            .Include(x => x.User)
            .Include(x=>x.Artwork).ThenInclude(x => x.User)
            .Include(x => x.Artwork).ThenInclude(x => x.ArtworkImages)
            .Include(x => x.Notification);

            var totalNotifi = userNotifications.Count();
            var result =  userNotifications.Select(_ => new GetUserNotificationDTO1
            {
                Date = _.Notification.Date,
                UserIdFor = _.UserIdFor,
                ArtWorkVM = _mapper.Map<Artwork, ArtWorkVM>(_.Artwork),
                UserVM = _mapper.Map<ApplicationUser, UserVM>(_.User),
                ArtWorkImageVM = _mapper.Map<ArtworkImage, ArtWorkImageVM>(_.Artwork.ArtworkImages.FirstOrDefault()),
                NotificationVM = _mapper.Map<Notification, NotificationVM>(_.Notification),
            }).Sort(string.IsNullOrEmpty(defaultSearch.sortBy) ? "Date" : defaultSearch.sortBy
                      , defaultSearch.isAscending)
                      .ToPageList(defaultSearch.currentPage, defaultSearch.perPage).AsNoTracking().ToListAsync();
            return result;
        }

        public int totalGetNotiUserSortResult(string userid)
        {
            var userNotifications = _unitOfWork.Repository<UserNofitication>()
           .GetQueryable()
           .Where(noti => noti.User.Id == userid)
           .Include(x => x.User)
           .Include(x => x.Artwork).ThenInclude(x => x.ArtworkImages)
           .Include(x => x.Notification);

            var totalNotifi = userNotifications.Count();
            return totalNotifi;
        }
    }
}
