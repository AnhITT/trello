using BusinessLogic_Layer.Mapping;
using BusinessLogic_Layer.Service;
using BusinessLogic_LayerDataAccess_Layer.Common;
using DataAccess_Layer.Common;
using DataAccess_Layer.Data;
using DataAccess_Layer.DTOs;
using DataAccess_Layer.Interfaces;
using DataAccess_Layer.UnitOfWorks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace UploadFile
{
    public static class FileDependencyExtenstion
    {
        public static void AddRepository(this IServiceCollection services)
        {
            services.AddTransient<IUnitOfWorkUpload,UnitOfWorkUpload>();
            services.AddTransient<UploadService>();
            services.AddLogging();
            services.AddAutoMapper(typeof(MappingUploadContainer));
            services.AddScoped<CallApi>();
            services.AddScoped<Auth>();

            services.AddHttpClient();
        }

        public static void ConfigureServices(WebApplicationBuilder builder)
        {
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddDbContext<UploadDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("UploadFile")));
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
            });
            builder.Services.AddAuthorization();
            builder.Services.AddSingleton(tokenValidationParameter);
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
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

            app.UseRequestLocalization(localizationOptions);
            app.Run();
        }
    }
}
