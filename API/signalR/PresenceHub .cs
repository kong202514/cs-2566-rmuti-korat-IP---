



using API.Entities;
using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.signalR;



[Authorize]
public class PresenceHub : Hub
{
    private readonly PresenceTracker _presenceTracker;

    public PresenceHub(PresenceTracker presenceTracker)
    {
        _presenceTracker = presenceTracker;
    }



    public override async Task OnConnectedAsync()
    {
        var username = Context?.User?.GetUsername();
        if (username is null || Context is null) return;
        await _presenceTracker.UserConnected(username, Context.ConnectionId);
        await Clients.Others.SendAsync("UserOnline", username);


        var onlineUsers = await _presenceTracker.GetOnlineUsers();
        await Clients.All.SendAsync("OnlineUsers", onlineUsers);
    }
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var username = Context?.User?.GetUsername();
        if (username is null || Context is null) return;
        await _presenceTracker.UserDisconnected(username, Context.ConnectionId);
        await Clients.Others.SendAsync("UserOffline", username);
        var onlineUsers = await _presenceTracker.GetOnlineUsers();
        await Clients.All.SendAsync("OnlineUsers", onlineUsers);
        await base.OnDisconnectedAsync(exception);
    }


    private async Task<bool> addToMessageGroup(string groupName)
    {
        if (Context is null || Context.User is null) return false;
        var connection = new Connection(Context.ConnectionId, Context.User.GetUsername());

        var group = await _messageRepository.GetMessageGroup(groupName);
        if (group is null)
        {
            group = new MessageGroup(groupName);
            _messageRepository.AddGroup(group);
        }

        group.Connections.Add(connection);
        return await _messageRepository.SaveAllAsync();
    }

    private async Task removeFromMessageGroup()
    {
        if (Context is null) return;
        var connection = await _messageRepository.GetConnection(Context.ConnectionId);
        if (connection is null) return;
        _messageRepository.RemoveConnection(connection);
        await _messageRepository.SaveAllAsync();

    }
}