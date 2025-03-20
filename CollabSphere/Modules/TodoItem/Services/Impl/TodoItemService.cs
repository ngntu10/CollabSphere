using AutoMapper;

using CollabSphere.Common;
using CollabSphere.Infrastructures.Repositories;
using CollabSphere.Infrastructures.Specifications.Impl;
using CollabSphere.Modules.TodoItem.Models;

namespace CollabSphere.Modules.TodoItem.Services.Impl;

public class TodoItemService : ITodoItemService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public TodoItemService(IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TodoItemResponseModel>> GetAllByListIdAsync(Guid id,
        CancellationToken cancellationToken = default)
    {
        var todoItemsSpec = TodoItemSpecification.GetTodoItemsByListIdSpec(id);
        var todoItems = await _unitOfWork.Repository<Entities.Domain.TodoItem>().GetAllAsync(todoItemsSpec);

        return _mapper.Map<IEnumerable<TodoItemResponseModel>>(todoItems);
    }

    public async Task<CreateTodoItemResponseModel> CreateAsync(CreateTodoItemModel createTodoItemModel,
        CancellationToken cancellationToken = default)
    {
        var todoListSpec = TodoListSpecification.GetTodoListByIdSpec(createTodoItemModel.TodoListId);
        var todoList = await _unitOfWork.Repository<Entities.Domain.TodoList>().GetFirstOrThrowAsync(todoListSpec);
        var todoItem = _mapper.Map<Entities.Domain.TodoItem>(createTodoItemModel);

        todoItem.List = todoList;
        var addedTodoItem = await _unitOfWork.Repository<Entities.Domain.TodoItem>().AddAsync(todoItem);

        await _unitOfWork.SaveChangesAsync();

        return new CreateTodoItemResponseModel
        {
            Id = addedTodoItem.Id
        };
    }

    public async Task<UpdateTodoItemResponseModel> UpdateAsync(Guid id, UpdateTodoItemModel updateTodoItemModel,
        CancellationToken cancellationToken = default)
    {
        var todoItemSpec = TodoItemSpecification.GetTodoItemByIdSpec(id);
        var todoItem = await _unitOfWork.Repository<Entities.Domain.TodoItem>().GetFirstOrThrowAsync(todoItemSpec);

        _mapper.Map(updateTodoItemModel, todoItem);

        var updatedTodoItem = await _unitOfWork.Repository<Entities.Domain.TodoItem>().UpdateAsync(todoItem);

        await _unitOfWork.SaveChangesAsync();

        return new UpdateTodoItemResponseModel
        {
            Id = updatedTodoItem.Id
        };
    }

    public async Task<BaseResponseModel> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var todoItemSpec = TodoItemSpecification.GetTodoItemByIdSpec(id);
        var todoItem = await _unitOfWork.Repository<Entities.Domain.TodoItem>().GetFirstOrThrowAsync(todoItemSpec);

        var deletedTodoItem = await _unitOfWork.Repository<Entities.Domain.TodoItem>().DeleteAsync(todoItem);

        await _unitOfWork.SaveChangesAsync();

        return new BaseResponseModel
        {
            Id = deletedTodoItem.Id
        };
    }
}
