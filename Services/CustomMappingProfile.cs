using AutoMapper;
using Portal.Data.Entities;
using Portal.Models;

namespace Portal.Services
{
    public class CustomMappingProfile: Profile
    {
        public CustomMappingProfile() 
        {
            CreateMap<TaskEntity, TaskItemModel>().ReverseMap();
        }
    }
}
