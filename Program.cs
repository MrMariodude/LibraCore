using LiberaryManagmentSystem.Models;
using LiberaryManagmentSystem.Repository;
using LiberaryManagmentSystem.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LiberaryManagmentSystem
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //? Create connection with MS SQL Server
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            //? Add Identity services
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;

                // Optional: Stronger password rules for better security
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            //? Configure Cookie settings for Identity
            builder.Services.ConfigureApplicationCookie(options =>
            {
                //! Path for login
                options.LoginPath = "/Account/AccessDenied";
                //! Path for access denied
                options.AccessDeniedPath = "/Account/AccessDenied";          

                
                // Cookie settings
                //options.Cookie.HttpOnly = true;  // Secure cookie
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;  // Enforce HTTPS in production
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);  // Expiration time of the authentication cookie
                options.SlidingExpiration = true;  // Renew cookie if user is active
                options.Cookie.SameSite = SameSiteMode.Strict;  // Adjust SameSite based on requirements
            });

            //? Register services for dependency injection
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IBookService, BookService>();
            builder.Services.AddScoped<ICheckoutService, CheckoutService>();
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped<AccountRepository>();
            builder.Services.AddScoped<RoleManager<IdentityRole>>();

            // Add controllers and views
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Seed roles into the database
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    await SeedRolesAsync(services);  // Ensure roles are seeded
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding roles.");
                }
            }

            // Configure the HTTP request pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            else{
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();  // Ensures requests are authenticated
            app.UseAuthorization();   // Ensures users are authorized

            // Map default route
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }

        // Seed roles into the database
        private static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var roles = new[] { "Admin", "User", "Librarian" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}
