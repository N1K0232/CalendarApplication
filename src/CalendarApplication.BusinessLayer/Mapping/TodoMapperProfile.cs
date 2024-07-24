using AutoMapper;
using CalendarApplication.Shared.Models;
using CalendarApplication.Shared.Models.Requests;
using TinyHelpers.Extensions;
using Entities = CalendarApplication.DataAccessLayer.Entities;

namespace CalendarApplication.BusinessLayer.Mapping;

public class TodoMapperProfile : Profile
{
    public TodoMapperProfile()
    {
        CreateMap<Entities.Todo, Todo>();
        CreateMap<SaveTodoRequest, Entities.Todo>()
            .ForMember(t => t.StartDate, options => options.MapFrom(t => t.StartDate.ToDateOnly()))
            .ForMember(t => t.StartTime, options => options.MapFrom(t => t.StartDate.ToTimeOnly()))
            .ForMember(t => t.FinishDate, options => options.MapFrom(t => t.FinishDate.GetValueOrDefault().ToDateOnly()))
            .ForMember(t => t.FinishTime, options => options.MapFrom(t => t.FinishDate.GetValueOrDefault().ToTimeOnly()));
    }
}