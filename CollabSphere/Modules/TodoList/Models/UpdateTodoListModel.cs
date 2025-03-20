using CollabSphere.Common;

namespace CollabSphere.Modules.TodoList.Models;

public class UpdateTodoListModel
{
    public string Title { get; set; }
}

public class UpdateTodoListResponseModel : BaseResponseModel { }
