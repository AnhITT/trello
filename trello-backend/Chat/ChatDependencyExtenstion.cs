using BusinessLogic_Layer.Mapping;
using BusinessLogic_Layer.Service;
using Chat.Hubs;
using DataAccess_Layer.DTOs;
using DataAccess_Layer.Interfaces;
using DataAccess_Layer.UnitOfWorks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Chat
{
    public static class ChatDependencyExtenstion
    {
        public static void AddRepository(this IServiceCollection services)
        {
            services.AddTransient<IUnitOfWorkChat, UnitOfWorkChat>();
            services.AddLogging();
            services.AddAutoMapper(typeof(MappingChatContainer));
            services.AddHttpClient();
        }

        public static void ConfigureServices(WebApplicationBuilder builder)
        {
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.Configure<MongoDBSetting>(builder.Configuration.GetSection("MongoDatabase"));
            builder.Services.AddLocalization();
            builder.Services.AddMemoryCache();
            builder.Services.AddOptions();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSingleton<GroupChatService>();
            builder.Services.AddSingleton<MessageService>();
            builder.Services.AddSignalR();
        }

        public static async void ConfigureAppsAsync(WebApplication app)
        {
            app.UseRouting();
            app.UseHttpsRedirection();
            app.MapControllers();
            var supportedCultures = new[] { "en", "vi" };
            var localizationOptions = new RequestLocalizationOptions().SetDefaultCulture("en")
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);

            localizationOptions.ApplyCurrentCultureToResponseHeaders = true;
            app.MapHub<ChatHub>("/chatHub");
            app.UseRequestLocalization(localizationOptions);
            app.Run();
        }
    }
}
