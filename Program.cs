using HairSalon.Data;
using HairSalon.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using HairSalon.Models;

namespace HairSalon
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<HairSalonContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<IMasterService, MasterService>();
            builder.Services.AddScoped<IReceptionService, ReceptionService>();
            builder.Services.AddScoped<IServiceService, ServiceService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All));
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddControllers();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<HairSalonContext>();
                var userService = scope.ServiceProvider.GetRequiredService<IUserService>();                               
            }

            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<HairSalonContext>();
                try
                {
                    context.Database.EnsureCreated();
                }
                catch (Exception ex)
                {
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Ошибка при миграции");
                }
            }

            app.Use(async (context, next) =>
            {
                context.Response.Headers.Append("Content-Type", "text/html; charset=utf-8");
                await next();
            });

            app.UseExceptionHandler("/Error");

            if (!app.Environment.IsDevelopment())
            {
                app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseRouting();

            app.MapBlazorHub();
            app.MapControllers();
            app.MapFallbackToPage("/_Host");

            app.Run();
        }
    }
}