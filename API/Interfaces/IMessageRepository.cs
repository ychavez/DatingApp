using System.Collections.Generic;
using System.Threading.Tasks;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface IMessageRepository
    {
        void AddMessage(Message message);
        void DeleteMessage(Message message);
        Task<Message> GetMessage(int id);

        Task<PagedList<MessageDTO>> GetMessagesForUser();

        Task<IEnumerable<MessageDTO>> GetMessageThread(int CurrentUserId, int recipientId);
        Task<bool> SaveAllAsync();
    }
}