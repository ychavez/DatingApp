using System.Collections.Generic;
using System.Threading.Tasks;
using API.Entities;
using API.Helpers;
using API.Interfaces;

namespace API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext context;

        public MessageRepository(DataContext context)
        {
            this.context = context;
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

        public Task<PagedList<MessageDTO>> GetMessagesForUser()
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<MessageDTO>> GetMessageThread(int CurrentUserId, int recipientId)
        {
            throw new System.NotImplementedException();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await context.SaveChangesAsync() > 0;
        }
    }
}