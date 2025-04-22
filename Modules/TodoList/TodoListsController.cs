using CollabSphere.Common;
using CollabSphere.Modules.TodoItem.Models;
using CollabSphere.Modules.TodoItem.Services;
using CollabSphere.Modules.TodoList.Models;
using CollabSphere.Modules.TodoList.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollabSphere.Modules.TodoList;

[Authorize]
public class TodoListsController : ApiController
{
    private readonly ITodoItemService _todoItemService;
    private readonly ITodoListService _todoListService;

    public TodoListsController(ITodoListService todoListService, ITodoItemService todoItemService)
    {
        _todoListService = todoListService;
        _todoItemService = todoItemService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(ApiResponse<IEnumerable<TodoListResponseModel>>.Success(StatusCodes.Status200OK,
            await _todoListService.GetAllAsync(), ""));
    }

    [HttpGet("{id:guid}/todoItems")]
    public async Task<IActionResult> GetAllTodoItemsAsync(Guid id)
    {
        return Ok(ApiResponse<IEnumerable<TodoItemResponseModel>>.Success(StatusCodes.Status200OK,
            await _todoItemService.GetAllByListIdAsync(id), ""));
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateTodoListModel createTodoListModel)
    {
        return Ok(ApiResponse<CreateTodoListResponseModel>.Success(StatusCodes.Status201Created,
            await _todoListService.CreateAsync(createTodoListModel), ""));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, UpdateTodoListModel updateTodoListModel)
    {
        return Ok(ApiResponse<UpdateTodoListResponseModel>.Success(StatusCodes.Status200OK,
            await _todoListService.UpdateAsync(id, updateTodoListModel), ""));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        return Ok(ApiResponse<BaseResponseModel>.Success(StatusCodes.Status200OK,
            await _todoListService.DeleteAsync(id), ""));
    }
}
