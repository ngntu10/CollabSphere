using CollabSphere.Common;

namespace CollabSphere.Modules.TodoList.Models;

public class CreateTodoListModel
{
    public string Title { get; set; }
}

public class CreateTodoListResponseModel : BaseResponseModel { }
