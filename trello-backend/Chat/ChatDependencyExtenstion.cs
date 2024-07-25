using BusinessLogic_Layer.Mapping;
using BusinessLogic_Layer.Service;
using DataAccess_Layer.DTOs;
using DataAccess_Layer.Interfaces;
using DataAccess_Layer.UnitOfWorks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BusinessLogic_LayerDataAccess_Layer.Common;

namespace Chat
{
    public static class ChatDependencyExtenstion
    {
        public static void AddRepository(this IServiceCollection services)
        {
            services.AddTransient<IUnitOfWorkChat, UnitOfWorkChat>();
            services.AddLogging();
            services.AddAutoMapper(typeof(MappingChatContainer));
            services.AddScoped<CallApi>();
            services.AddHttpClient();
            services.AddTransient<ChatService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        public static void ConfigureServices(WebApplicationBuilder builder)
        {
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.Configure<MongoDBSetting>(builder.Configuration.GetSection("MongoDatabase"));
            builder.Services.AddLocalization();
            builder.Services.AddMemoryCache();
            builder.Services.AddOptions();
            builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));
            var key = Encoding.ASCII.GetBytes(builder.Configuration.GetSection("JwtConfig:Secret").Value);
            var tokenValidationParameter = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidAudience = builder.Configuration["JwtConfig:ValidAudience"],
                ValidIssuer = builder.Configuration["JwtConfig:ValidIssuer"],
                RequireExpirationTime = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(jwt =>
            {
                jwt.SaveToken = true;
                jwt.TokenValidationParameters = tokenValidationParameter;
                jwt.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chatHub"))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });
            builder.Services.AddAuthorization();
            builder.Services.AddSingleton(tokenValidationParameter);
            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder.WithOrigins("http://localhost:5173")
                           .AllowAnyHeader()
                           .AllowAnyMethod()
                           .AllowCredentials();
                });
            });

            builder.Services.AddSignalR();
        }

        public static async void ConfigureAppsAsync(WebApplication app)
        {
            app.UseRouting();
            app.UseHttpsRedirection();
            app.MapControllers();
            app.UseAuthentication();
            app.UseAuthorization();
            var supportedCultures = new[] { "en", "vi" };
            var localizationOptions = new RequestLocalizationOptions().SetDefaultCulture("en")
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);

            localizationOptions.ApplyCurrentCultureToResponseHeaders = true;
            app.MapHub<ChatHub>("/chatHub");
            app.UseCors("CorsPolicy");
            app.UseRequestLocalization(localizationOptions);
            app.Run();
        }
    }
}
