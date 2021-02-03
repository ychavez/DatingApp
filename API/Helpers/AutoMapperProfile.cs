using System;
using System.Linq;
using API.DTO;
using API.Entities;
using API.Extensions;
using AutoMapper;

namespace API.Helpers
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<AppUser,MemberDTO>()
            .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src=> src.Photos.FirstOrDefault(x => x.IsMain).Url))
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
            CreateMap<Photo,PhotoDTO>();
            CreateMap<MemberUpdateDTO,AppUser>();
            CreateMap<RegisterDto,AppUser>();
            CreateMap<Message,MessageDTO>()
            .ForMember(dest => dest.SenderPhotoUrl, opt => opt.MapFrom(src => 
            src.Sender.Photos.FirstOrDefault(x => x.IsMain).Url))
             .ForMember(dest => dest.RecipientPhotoUrl, opt => opt.MapFrom(src => 
            src.Recipient.Photos.FirstOrDefault(x => x.IsMain).Url));
            CreateMap<DateTime,DateTime>().ConvertUsing(t => DateTime.SpecifyKind(t, DateTimeKind.Utc));
        }
    }
}