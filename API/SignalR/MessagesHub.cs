using System;
using System.Linq;
using System.Threading.Tasks;
using API.DTO;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    public class MessagesHub : Hub
    {
        private readonly IMessageRepository messageRepository;
        private readonly IMapper mapper;
        private readonly IHubContext<PresenceHub> presenceHub;
        private readonly IUserRepository userRepository;
        private readonly PresenceTracker tracker;
        public MessagesHub(IMessageRepository messageRepository, IMapper mapper,
        IUserRepository userRepository,
        IHubContext<PresenceHub> presenceHub, PresenceTracker tracker)
        {
            this.tracker = tracker;
            this.presenceHub = presenceHub;
            this.userRepository = userRepository;
            this.mapper = mapper;
            this.messageRepository = messageRepository;

        }
        public override async Task OnConnectedAsync()
        {
            var httpcontext = Context.GetHttpContext();
            var otherUser = httpcontext.Request.Query["user"].ToString();
            var groupName = GetGroupName(Context.User.GetUsername(), otherUser);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await AddToGroup(Context, groupName);

            var messages = await messageRepository.GetMessageThread(Context.User.GetUsername(), otherUser);

            await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);

        }
        private string GetGroupName(string caller, string other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }
        public async Task SendMessage(CreateMessageDTO createMessageDTO)
        {
            var username = Context.User.GetUsername();

            if (username == createMessageDTO.RecipientUserName.ToLower())
            {
                throw new HubException("You cannot send messages to yourself");
            }

            var sender = await userRepository.GetUserByUsername(username);
            var recipient = await userRepository.GetUserByUsername(createMessageDTO.RecipientUserName);

            if (recipient == null) throw new HubException("Not found user");

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDTO.Content
            };

            var groupName = GetGroupName(sender.UserName, recipient.UserName);
            var group = await messageRepository.GetMessageGroup(groupName);

            if (group.Connections.Any(x => x.Username == recipient.UserName))
            {
                message.DateRead = DateTime.UtcNow;
            }
            else
            {
                var connections = await tracker.GetConnectionForUser(recipient.UserName);
                if (connections != null)
                {
                    await presenceHub.Clients.Clients(connections).SendAsync("NewMessageRecived", new { username = sender.UserName, KnownAs = sender.KnownAs });
                }
            }
            messageRepository.AddMessage(message);

            if (await messageRepository.SaveAllAsync())
            {

                await Clients.Group(groupName).SendAsync("NewMessage", mapper.Map<MessageDTO>(message));
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await RemoveFromMessageGroup(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        private async Task<bool> AddToGroup(HubCallerContext context, string groupName)
        {
            var group = await messageRepository.GetMessageGroup(groupName);
            var connection = new Connection(context.ConnectionId, Context.User.GetUsername());
            if (group == null)
            {
                group = new Group(groupName);
                messageRepository.AddGroup(group);
            }
            group.Connections.Add(connection);
            return await messageRepository.SaveAllAsync();
        }

        private async Task RemoveFromMessageGroup(string ConnectionId)
        {
            var connection = await messageRepository.GetConnection(ConnectionId);
            messageRepository.RemoveConnection(connection);
            await messageRepository.SaveAllAsync();
        }
        

    }
}