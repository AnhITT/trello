using Microsoft.AspNetCore.SignalR;
using BusinessLogic_Layer.Entity;
using BusinessLogic_Layer.Service;
using DataAccess_Layer.Helpers;
using System.Security.Claims;

public class ChatHub : Hub
{
    private readonly ChatService _chatService;
    private static readonly Dictionary<string, string> userConnections = new Dictionary<string, string>();

    public ChatHub(ChatService chatService)
    {
        _chatService = chatService;
    }

    /*public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst("Id")?.Value;
        if (userId != null)
        {
            lock (userConnections)
            {
                userConnections[userId] = Context.ConnectionId;
            }
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId != null)
        {
            lock (userConnections)
            {
                userConnections.Remove(userId);
            }
        }
        await base.OnDisconnectedAsync(exception);
    }*/

    public async Task SendMessage(string receiverUserId, ApiMessage apiMessage)
    {
        var result = await _chatService.Update(apiMessage);

        if (result.Success)
        {
            string receiverConnectionId;
            lock (userConnections)
            {
                userConnections.TryGetValue(receiverUserId, out receiverConnectionId);
            }

            if (receiverConnectionId != null)
            {
                await Clients.Client(receiverConnectionId).SendAsync("ReceiveMessage", apiMessage);
            }
        }
    }

    public string GetConnectionId() => Context.ConnectionId;
}
