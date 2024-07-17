using ChatRoom;

var builder = WebApplication.CreateBuilder(args);

ChatRoomDependencyExtenstion.AddRepository(builder.Services); // Chỉ truyền một tham số ở đây
ChatRoomDependencyExtenstion.ConfigureServices(builder);

var app = builder.Build();

ChatRoomDependencyExtenstion.ConfigureAppsAsync(app);
