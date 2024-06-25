using UploadFile;

var builder = WebApplication.CreateBuilder(args);

FileDependencyExtenstion.AddRepository(builder.Services);
FileDependencyExtenstion.ConfigureServices(builder);

var app = builder.Build();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

FileDependencyExtenstion.ConfigureAppsAsync(app);