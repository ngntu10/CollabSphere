using AutoMapper;

using CollabSphere.Modules.TodoList.Models;

namespace CollabSphere.Modules.TodoList.MappingProfiles;

public class TodoListProfile : Profile
{
    public TodoListProfile()
    {
        CreateMap<CreateTodoListModel, Entities.Domain.TodoList>();

        CreateMap<Entities.Domain.TodoList, TodoListResponseModel>();
    }
}
