using DeliveryApp.Infraestrutura.Orm;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DeliveryApp.Infraestrutura;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddDataProtection();
        services.AddIdentityCore<IdentityUser<Guid>>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = false;
            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;
        })
        .AddRoles<IdentityRole<Guid>>()
        .AddEntityFrameworkStores<DeliveryAppDbContext>()
        .AddSignInManager()
        .AddDefaultTokenProviders();

        services.AddDbContext<DeliveryAppDbContext>(options =>
        {
            if (configuration["Infra:DatabaseProvider"] == "InMemory")
            {
                options.UseInMemoryDatabase("DeliveryApp");
            }
            else
            {
                string? connectionString = configuration.GetConnectionString("PostgresEF");

                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    throw new InvalidOperationException(
                        $"A connection string \"PostgresEF\" não foi encontrada."
                    );
                }

                options.UseNpgsql(connectionString, opt =>
                {
                    opt.EnableRetryOnFailure(3);
                });
            }
        });
    }
}
