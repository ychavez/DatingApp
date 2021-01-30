using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AdminController : BaseApiController
    {
        private readonly UserManager<AppUser> _user;
        public AdminController(UserManager<AppUser> user)
        {
            _user = user;

        }
        [Authorize(Policy = "RequiredAminRole")]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetUsersWithRoles()
        {
            var users = await _user.Users
            .Include(r => r.UserRoles)
            .ThenInclude(r => r.Role)
            .OrderBy(u => u.UserName)
            .Select(u => new
            {

                u.Id,
                Username = u.UserName,
                Roles = u.UserRoles.Select(r => r.Role.Name).ToList()
            }).ToListAsync();

            return Ok(users);

        }

        [HttpPost("edit-roles/{username}")]
        public async Task<ActionResult> EditRoles(string username, [FromQuery] string roles)
        {
            var selectedRoles = roles.Split(",").ToArray();
            
            var user = await _user.FindByNameAsync(username);
            
            if(user ==  null) return NotFound("Could not fin user");

            var userRoles = await _user.GetRolesAsync(user);
            
            var result = await _user.AddToRolesAsync(user,selectedRoles.Except(userRoles));
            if(!result.Succeeded) return BadRequest("Failed to add to roles");

            result = await _user.RemoveFromRolesAsync(user,userRoles.Except(selectedRoles));

            if(!result.Succeeded) return BadRequest("Failed to remove from roles");

          return Ok( await _user.GetRolesAsync(user));
        }


        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photos-to-moderate")]
        public ActionResult GetPhotosForModeration()
        {
            return Ok("Only Admin or moderators can see this");
        }

    }
}