using Microsoft.AspNetCore.SignalR;

namespace Onyx.SignalR
{
    public sealed class DutUpdateHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var clientId = Context.GetHttpContext().Request.Query["clientId"].ToString();

            if (!clientId.StartsWith("IE50"))
            {
                await Clients.Client(Context.ConnectionId).SendAsync("CloseConnection", "Connection not allowed for this machine.");
            }

            await Clients.All.SendAsync("ReceiveMessage", $"[{Context.ConnectionId}] subscribed");
        }
    }
}
