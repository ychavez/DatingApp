using API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace API.Data
{
    public class DataContext : DbContext
    {
    
        public DataContext( DbContextOptions options) : base(options)
        {
        }
        public DbSet<AppUser> Users {get;set;}
        
    }
}