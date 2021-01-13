using AutoMapper;
using messageqServer.Entities;
using messageqServer.Models;

namespace messageqServer.Helpers
{
    public class AutoMapperProfile : Profile
    {
        // mappings between model and entity objects
        public AutoMapperProfile()
        {
           
            CreateMap<CreateEventRequest, EventProcess>();

            CreateMap<EventProcess, EventResponse>();

            CreateMap<EventProcess, CreateEventResponse>();
        }
    }
}