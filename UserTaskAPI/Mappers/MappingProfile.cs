using AutoMapper;
using UserTaskAPI.DTO;
using UserTaskAPI.Models;

namespace UserTaskAPI.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<User, GetUsersDto>().ReverseMap();
            CreateMap<UserTask, UserTaskDto>().ReverseMap();
            CreateMap<UserTask, TaskDto>().ReverseMap();
        }
    }
}
