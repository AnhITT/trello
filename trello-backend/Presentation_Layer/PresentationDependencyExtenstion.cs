using BusinessLogic_Layer.Common;
using BusinessLogic_Layer.Mapping;
using BusinessLogic_Layer.Service;
using DataAccess_Layer.Common;
using DataAccess_Layer.Data;
using DataAccess_Layer.DTOs;
using DataAccess_Layer.Interfaces;
using DataAccess_Layer.UnitOfWorks;
using DataAccess_Layer.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Elasticsearch.Net;
using Nest;
using BusinessLogic_LayerDataAccess_Layer.Common;

namespace Presentation_Layer
{
    public static class PresentationDependencyExtenstion
    {
        public static void AddRepository(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<AuthService>();
            services.AddTransient<MailService>();
            services.AddTransient<UserService>();
            services.AddTransient<BoardService>();
            services.AddTransient<TaskCardService>();
            services.AddTransient<WorkflowService>();
            services.AddTransient<WorkspaceService>();
            services.AddTransient<CommentService>();
            services.AddTransient<ElasticsearchService>();

            services.AddScoped<Generate>();
            services.AddScoped<CallApi>();
            services.AddScoped<DeleteChild>();

            services.AddLogging();
            services.AddAutoMapper(typeof(MappingContainer));
            services.AddHttpClient();

            var elasticConfig = configuration.GetSection("Elasticsearch");
            var apiKey = elasticConfig["ApiKey"];
            var cloudId = elasticConfig["CloudId"];
            var settings = new ConnectionSettings(cloudId, new ApiKeyAuthenticationCredentials(apiKey));
            var client = new ElasticClient(settings);
            
            services.AddSingleton<IElasticClient>(client);
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSignalR();
        }

        public static void ConfigureServices(WebApplicationBuilder builder)
        {
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddDbContext<MainDBContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("Sampleconn")));
            builder.Services.AddLocalization();
            builder.Services.AddMemoryCache();
            builder.Services.AddOptions();
            builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("EmailSettings"));
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

        public static async void ConfigureAppsAsync(WebApplication app, bool isUploadFile = false, bool isSignal = false, bool isSendNotify = false, string connStr = null)
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
            app.UseCors("CorsPolicy");
            app.UseRequestLocalization(localizationOptions);
            app.Run();
        }
    }
}
