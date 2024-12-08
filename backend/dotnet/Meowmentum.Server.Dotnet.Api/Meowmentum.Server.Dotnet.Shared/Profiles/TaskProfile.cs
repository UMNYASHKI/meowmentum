﻿using Meowmentum.Server.Dotnet.Core.Entities;
using Meowmentum.Server.Dotnet.Shared.Requests.Task;
using Meowmentum.Server.Dotnet.Shared.Responses.Task;
using Task = Meowmentum.Server.Dotnet.Core.Entities.Task;
using AutoMapper;

public class TaskProfile : Profile
{
    public TaskProfile()
    {
        CreateMap<TaskRequest, Task>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority))
            .ForMember(dest => dest.Deadline, opt => opt.MapFrom(src => src.Deadline))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.TaskTags, opt => opt.MapFrom(src =>
                src.TagIds != null
                ? src.TagIds.Where(tagId => tagId > 0).Select(tagId => new TaskTag { TagId = tagId }).ToList()
                : new List<TaskTag>()));

        CreateMap<Task, TaskResponse>()
            .ForMember(dest => dest.TimeSpent, opt => opt.MapFrom(src => src.TimeIntervals))
            .ForMember(dest => dest.TagIds, opt => opt.MapFrom(src => 
                src.TaskTags != null 
                ? src.TaskTags.Select(tt => tt.TagId).ToList() 
                : new List<long>()))
            .ForMember(dest => dest.TagNames, opt => opt.MapFrom(src => 
                src.TaskTags != null 
                ? src.TaskTags.Select(tt => tt.Tag.Name).ToList() 
                : new List<string>()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority))
            .ForMember(dest => dest.Deadline, opt => opt.MapFrom(src => src.Deadline))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description)) 
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title));

        CreateMap<Task, Task>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Deadline, opt => opt.MapFrom(src => src.Deadline))
            .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.TaskTags, opt => opt.MapFrom(src => src.TaskTags))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore());
    }
}
