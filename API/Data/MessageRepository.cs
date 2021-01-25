using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTO;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext context;
        private readonly IMapper _mapper;

        public MessageRepository(DataContext context, IMapper mapper)
        {
            this.context = context;
             _mapper = mapper;
        }
        public void AddMessage(Message message)
        {
            context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
           
            context.Messages.Remove(message);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await context.Messages.FindAsync(id);
        }

        public async Task<PagedList<MessageDTO>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = context.Messages
            .OrderByDescending(m => m.MessageSent)
            .AsQueryable();

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(u => u.Recipient.UserName == messageParams.Username),
                "Outbox" => query.Where(u => u.Sender.UserName == messageParams.Username),
                _ => query.Where(u => u.Recipient.UserName ==
                 messageParams.Username && u.DateRead == null)
            };

            var messages = query.ProjectTo<MessageDTO>(_mapper.ConfigurationProvider);

            return await PagedList<MessageDTO>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);


        }

        public Task<IEnumerable<MessageDTO>> GetMessageThread(string CurrentUsername, string recipientUsername)
        {
            throw new System.NotImplementedException();
        }

   

        public async Task<bool> SaveAllAsync()
        {
            return await context.SaveChangesAsync() > 0;
        }
    }
}