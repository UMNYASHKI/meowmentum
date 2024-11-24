using Meowmentum.Server.Dotnet.Core.Entities;
using Meowmentum.Server.Dotnet.Shared.Requests.Task;
using Meowmentum.Server.Dotnet.Shared.Responses.Task;
using Task = Meowmentum.Server.Dotnet.Core.Entities.Task;
using AutoMapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CreateTaskRequest, Task>()
    .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
    .ForMember(dest => dest.TagId, opt => opt.MapFrom(src => src.TagId == 0 ? (long?)null : src.TagId))
    .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
    .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority))
    .ForMember(dest => dest.Deadline, opt => opt.MapFrom(src => src.Deadline))
    .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
    .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title));

        CreateMap<Task, TaskResponse>()
            .ForMember(dest => dest.TagId, opt => opt.MapFrom(src => src.TagId))
            .ForMember(dest => dest.TimeSpent, opt => opt.MapFrom(src => src.TimeIntervals))
            .ForMember(dest => dest.TagId, opt => opt.MapFrom(src => src.TagId))
            .ForMember(dest => dest.TagName, opt => opt.MapFrom(src => src.Tag != null ? src.Tag.Name : null))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority))
            .ForMember(dest => dest.Deadline, opt => opt.MapFrom(src => src.Deadline))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description)) 
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title));
    }

    public class TimeInterval
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Description { get; set; }
    }
}
