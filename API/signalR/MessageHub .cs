


using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;


namespace API.signalR;


[Authorize]
public class MessageHub : Hub
{
    private readonly IMessageRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IMessageRepository _messageRepository;

    public MessageHub(IMessageRepository messageRepository,
                        IUserRepository userRepository,
                        IMapper mapper)

    {
        _messageRepository = messageRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }


    public override async Task OnConnectedAsync()
    {
        var callerUsername = Context?.User?.GetUsername();
        var httpContext = Context?.GetHttpContext();
        var otherUsername = httpContext?.Request?.Query["user"].ToString();
        if (Context is null || callerUsername is null || httpContext is null || otherUsername is null) return;

        var groupName = getGroupName(callerUsername, otherUsername);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        var messages = await _messageRepository.GetMessageThread(callerUsername, otherUsername);
        await Clients.Group(groupName).SendAsync("MessageThread", messages);
    }






    private string getGroupName(string caller, string? other)
    {
        var stringCompare = string.CompareOrdinal(caller, other) < 0;
        return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
    }



    public async Task SendMessage(CreateMessageDto createMessageDto)
    { //code ดัดแปลงมาจาก MessagesController.cs -> CreateMessage
        if (createMessageDto.RecipientUsername is null || Context.User is null)
            throw new HubException("not found");

        var username = Context.User.GetUsername();
        if (username is null) return;
        if (username == createMessageDto?.RecipientUsername?.ToLower())
            throw new HubException("can't send to yourself!");

        var sender = await _userRepository.GetUserByUserNameAsync(username);
        var recipient = await _userRepository.GetUserByUserNameAsync(createMessageDto!.RecipientUsername);

        if (recipient is null || sender?.UserName is null) throw new HubException("not found");
        var message = new Message
        {
            Sender = sender,
            Recipient = recipient,
            Content = createMessageDto.Content,
            SenderUsername = sender.UserName,
            RecipientUsername = recipient.UserName
        };
        _messageRepository.AddMessage(message);
        var msdto = _mapper.Map<MessageDto>(message);

        if (await _messageRepository.SaveAllAsync())
        {
            var groupName = getGroupName(sender.UserName, recipient.UserName);
            await Clients.Group(groupName).SendAsync("NewMessage", msdto);
        }
    }
    public override Task OnDisconnectedAsync(Exception? exception)
    {
        return base.OnDisconnectedAsync(exception);
    }
}