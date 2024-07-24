using Microsoft.AspNetCore.SignalR;
using BusinessLogic_Layer.Entity;
using BusinessLogic_Layer.Service;
using DataAccess_Layer.Helpers;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using MongoDB.Driver.Core.Connections;
using DataAccess_Layer.Interfaces;
using MongoDB.Bson;
using Nest;

public class ChatHub : Hub
{
    private readonly ChatService _chatService;
    private readonly IUnitOfWorkChat _unitOfWork;
    private static readonly Dictionary<string, string> userConnections = new Dictionary<string, string>();
    public ChatHub(ChatService chatService, IUnitOfWorkChat unitOfWork)
    {
        _chatService = chatService;
        _unitOfWork = unitOfWork;
    }
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst("Id")?.Value;
        if (userId != null)
        {
            lock (userConnections)
            {
                userConnections[userId.ToString()] = Context.ConnectionId;
            }
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var userId = Context.User?.FindFirst("Id")?.Value;
        if (userId != null)
        {
            lock (userConnections)
            {
                userConnections.Remove(userId.ToString());
            }
        }
        await base.OnDisconnectedAsync(exception);
    }
    public async Task SendMessage(ApiMessage apiMessage)
    {
        // Cập nhật tin nhắn vào cơ sở dữ liệu
        var updateResult = await _chatService.Update(apiMessage);

        // Lấy thông tin chat
        var objectId = new ObjectId(apiMessage.ChatId);
        var data = await _unitOfWork.ChatRepository.GetByIdAsync(objectId);

        if (data != null)
        {
            {
                // Tạo danh sách các kết nối hiện tại cho các thành viên của chat
                var connectedUserIds = data.Members
                    .Where(memberId => userConnections.ContainsKey(memberId))
                    .ToList();

                if (connectedUserIds?.Count > 0)
                {
                    // Gửi tin nhắn đến tất cả các kết nối của các thành viên
                    foreach (var userId in connectedUserIds)
                    {
                        var connectionId = userConnections[userId];
                        await Clients.Client(connectionId).SendAsync("ReceiveMessage", apiMessage);
                    }
                }
            }
        }
        else
        {
            Console.WriteLine($"Failed to retrieve chat");
        }
    }


}
