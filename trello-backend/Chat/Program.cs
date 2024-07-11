using Chat;

var builder = WebApplication.CreateBuilder(args);

ChatDependencyExtenstion.AddRepository(builder.Services); // Chỉ truyền một tham số ở đây
ChatDependencyExtenstion.ConfigureServices(builder);

var app = builder.Build();

// Cấu hình và khởi động ứng dụng
ChatDependencyExtenstion.ConfigureAppsAsync(app);
