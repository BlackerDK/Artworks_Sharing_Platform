using AutoMapper;
using Domain.Entities;
using Domain.Model;

namespace API.Helper
{
    public class ApplicationMapper : Profile
    {
        public ApplicationMapper()
        {
            CreateMap<Artwork, ArtworkDTO>().ReverseMap();
            CreateMap<Artwork, ArtworkAddDTO>().ReverseMap();
            CreateMap<Artwork, ArtworkUpdateDTO>().ReverseMap();
            CreateMap<Notification, NotificationDTO>().ReverseMap();
            CreateMap<Notification, CreateNotificationDTO>().ReverseMap();
            CreateMap<UserNofitication, UserNotificationDTO>().ReverseMap();
            CreateMap<UserNofitication, CreateUserNotificationDTO>().ReverseMap();
            CreateMap<Category, CatalogyDTO>().ReverseMap();
            CreateMap<Category, CatalogyAddDTO>().ReverseMap();
            CreateMap<Package, PackageDTO>().ReverseMap();
            CreateMap<Package, PackageAddDTO>().ReverseMap();
            CreateMap<Poster, PosterDTO>().ReverseMap();
            CreateMap<Poster, PosterAddDTO>().ReverseMap();
            CreateMap<Artwork, ArtworkDTO>().ReverseMap();
            CreateMap<Artwork, ArtworkAddDTO>().ReverseMap();
            CreateMap<Artwork, ArtworkUpdateDTO>().ReverseMap();
            CreateMap<ArtworkImage, ArtworkImageDTO>().ReverseMap();
            CreateMap<Order, OrderDTO>().ReverseMap();
            CreateMap<Order, OrderCreateDTO>().ReverseMap();
            CreateMap<Order, OrderUpdateDTO>().ReverseMap();
            CreateMap<Order, OrderDeleteDTO>().ReverseMap();
            CreateMap<Like, LikeDTO>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User.Id))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ArtworkId, opt => opt.MapFrom(src => src.Artwork.ArtworkId));

			CreateMap<Like, LikeCreateDTO>().ReverseMap();
            CreateMap<Comment, CommentDTO>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User.Id))
				.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
				.ForMember(dest => dest.ArtworkId, opt => opt.MapFrom(src => src.Artwork.ArtworkId));
			CreateMap<Comment, CommentAddDTO>().ReverseMap();
            CreateMap<Comment, CommentUpdateDTO>().ReverseMap();

            CreateMap<ApplicationUser, UserRoles>().ReverseMap();
            CreateMap<UserRoles, UserRolesVM>().ReverseMap();

            CreateMap<UserNofitication, UserNotificationDTO>().ReverseMap();
            CreateMap<UserNotificationDTO, GetUserNotificationDTO>().ReverseMap();
            CreateMap<Notification, GetUserNotificationDTO>()
                .ForMember(dest => dest.dateTime, opt => opt.MapFrom(src => src.Date))
                .ForMember(dest => dest.notificationId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.NotificationTitle, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.NotificationDescription, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.isRead, opt => opt.MapFrom(src => src.IsRead));

            CreateMap<Artwork, GetUserNotificationDTO>()
                .ForMember(dest => dest.ArtworkTitle, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.artworkId, opt => opt.MapFrom(src => src.ArtworkId));

            CreateMap<ApplicationUser, GetUserNotificationDTO>()
             .ForMember(dest => dest.nameUser, opt => opt.MapFrom(src => src.LastName));

            CreateMap<ArtworkImage, GetUserNotificationDTO>()
            .ForMember(dest => dest.artwordUrl, opt => opt.MapFrom(src => src.Image));


            CreateMap<ApplicationUser, UserVM>().ReverseMap();
            CreateMap<Artwork, ArtWorkVM>().ReverseMap();
            CreateMap<ArtworkImage, ArtWorkImageVM> ().ReverseMap();
            CreateMap<Notification, NotificationVM> ().ReverseMap();


			CreateMap<Cart, CartDTO>().ReverseMap();
		}
    }
}
