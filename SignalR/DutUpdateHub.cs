using Microsoft.AspNetCore.SignalR;

namespace Onyx.SignalR
{
    public sealed class DutUpdateHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var clientId = Context.GetHttpContext().Request.Query["clientId"].ToString();
            var lineId = Context.GetHttpContext().Request.Query["lineId"].ToString();
            //var typeId = Context.GetHttpContext().Request.Query["typeId"].ToString();
            var group = $"G-{lineId}";

            if (!clientId.StartsWith("IE50"))
            {
                await Clients.Client(Context.ConnectionId).SendAsync("CloseConnection", "Connection not allowed for this machine.");
                return;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, group);
            await Clients.Client(Context.ConnectionId).SendAsync("AddedToGroup", $"Successfully added to the group {group}");
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            var lineId = Context.GetHttpContext().Request.Query["lineId"].ToString();
            //var typeId = Context.GetHttpContext().Request.Query["typeId"].ToString();
            //var group = $"G-{lineId}-{typeId}";
            var group = $"G-{lineId}";

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, group);
            Console.WriteLine($"Removed from group {group}");

            await base.OnDisconnectedAsync(ex);
        }

        public async Task SendMessageToGroup(string method, string group, string message)
        {
            await Clients.Group(group).SendAsync(method, message);
        }
    }
}
