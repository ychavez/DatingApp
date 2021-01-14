using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTO;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper mapper;

        public UserRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            this.mapper = mapper;
        }

        public async Task<MemberDTO> GetMemberAsync(string usrername)
        {
            return await _context.Users.Where(x => x.UserName == usrername)
            .ProjectTo<MemberDTO>(mapper.ConfigurationProvider)
            .SingleOrDefaultAsync();
        }


        public async Task<PagedList<MemberDTO>> GetMembersAsync(UserParams userParams)
        {
            var query =  _context.Users.ProjectTo<MemberDTO>(mapper.ConfigurationProvider).AsNoTracking();

            return await PagedList<MemberDTO>.CreateAsync(query, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<IEnumerable<AppUser>> GetUserAsync() => await _context.Users.Include(p => p.Photos).ToListAsync();
        public async Task<AppUser> GetUserById(int id) => await _context.Users.FindAsync(id);
        public async Task<AppUser> GetUserByUsername(string username) => await _context.Users.Include(p => p.Photos).SingleOrDefaultAsync(x => x.UserName == username);
        public async Task<bool> SaveAllAsync() => await _context.SaveChangesAsync() > 0;
        public void Update(AppUser user) => _context.Entry(user).State = EntityState.Modified;

    }
}