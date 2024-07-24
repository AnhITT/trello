using Microsoft.AspNetCore.SignalR;
using BusinessLogic_Layer.Service;
using DataAccess_Layer.Helpers;
using System.Security.Claims;

public class BoardHub : Hub
{
    private static readonly Dictionary<string, string> userConnections = new Dictionary<string, string>();
    public BoardHub()
    {
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

    /*    public async Task SendMessage(ApiMessage apiMessage)
        {
            var result = await _chatService.Update(apiMessage);

            userConnections.TryGetValue(apiMessage.receverid, connectionid);



            if (result.Success)
            {
                if (receiverUserId != null)
                {
                    await Clients.Client(conectionid).SendAsync("ReceiveMessage", apiMessage);
                }
            }
        }
    */


}
