using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Data;
using API.DTO;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{

    [Authorize]
    public class UsersController : BaseApiController
    {

        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public UsersController(IUserRepository repository, IMapper mapper, IPhotoService photoService)
        {
            _repository = repository;
            _mapper = mapper;
            _photoService = photoService;
        }

        // api/user
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUsers() => Ok(await _repository.GetMembersAsync());

        // api/user/3
        [HttpGet("{username}", Name="GetUser")]
        public async Task<ActionResult<MemberDTO>> GetUser(string username) => Ok(await _repository.GetMemberAsync(username));

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDTO memberUpdateDTO)
        {
            var username = User.GetUsername();
            var user = await _repository.GetUserByUsername(username);
            _mapper.Map(memberUpdateDTO, user);

            _repository.Update(user);

            if (await _repository.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to update!");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDTO>> AddPhoto(IFormFile file)
        {
            var user = await _repository.GetUserByUsername(User.GetUsername());
            var result = await _photoService.AddPhotoAsync(file);

            if (result.Error != null)
            {
                return BadRequest(result.Error.Message);
            }
            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId,
            };
            if (user.Photos.Count == 0)
            {
                photo.IsMain = true;
            }

            user.Photos.Add(photo);
            if (await _repository.SaveAllAsync())
            {

                // return _mapper.Map<PhotoDTO>(photo);
                return CreatedAtRoute("GetUser",new {username = user.UserName} ,_mapper.Map<PhotoDTO>(photo));

            }

            return BadRequest("Error adding photo");


        }

    }
}