using Presentation_Layer;

var builder = WebApplication.CreateBuilder(args);

PresentationDependencyExtenstion.AddRepository(builder.Services, builder.Configuration);
PresentationDependencyExtenstion.ConfigureServices(builder);

var app = builder.Build();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

PresentationDependencyExtenstion.ConfigureAppsAsync(app);