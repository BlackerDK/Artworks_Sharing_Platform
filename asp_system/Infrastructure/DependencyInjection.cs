using Application.Interfaces;
using Application.Interfaces.Services;
using Infrastructure.Data;
using Infrastructure.Persistence.Services;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Domain.Entities;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IArtworkService, ArtworkService>();
        services.AddScoped<IArtworkImageService, ArtworkImageService>();
        services.AddScoped<IUserServices, UserService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IUserNotificationService, UserNotificationService>();
        services.AddScoped<ICatalogyService, CatalogyService>();
        services.AddScoped<IPackageService, PackageService>();
        services.AddScoped<IPosterService, PosterService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<ILikeService, LikeService>();
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<ICartService, CartService>();
		return services;
    }
}
