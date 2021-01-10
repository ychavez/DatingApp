using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUsers(DataContext context)
        {
            if (await context.Users.AnyAsync()) return;

            var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);


            foreach (var user in users)
            {
                using var hmac = new HMACSHA512();
                var passwordSalt = hmac.Key;
                var passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Holaaaa"));
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();

            }




        }
    }
}