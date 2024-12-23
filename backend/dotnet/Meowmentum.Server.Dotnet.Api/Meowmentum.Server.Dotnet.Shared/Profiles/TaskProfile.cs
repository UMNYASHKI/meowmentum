﻿using Meowmentum.Server.Dotnet.Core.Entities;
using Meowmentum.Server.Dotnet.Shared.Requests.Task;
using Meowmentum.Server.Dotnet.Shared.Responses.Task;
using Task = Meowmentum.Server.Dotnet.Core.Entities.Task;
using AutoMapper;
using Meowmentum.Server.Dotnet.Shared.Responses;

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
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src =>
                src.TaskTags != null
                ? src.TaskTags.Select(tt => new TagResponse
                {
                    Id = tt.TagId,
                    Name = tt.Tag.Name,
                    CreatedDate = tt.Tag.CreatedDate,
                    UpdatedDate = tt.Tag.UpdatedDate
                }).ToList()
                : new List<TagResponse>()))
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.Deadline, opt => opt.MapFrom(src => src.Deadline))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority))
            .ForMember(dest => dest.TimeIntervals, opt => opt.MapFrom(src => src.TimeIntervals));

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
