using CollabSphere.Entities.Domain;

namespace CollabSphere.Infrastructures.Specifications.Impl;

public static class TodoItemSpecification
{
    public static BaseSpecification<TodoItem> GetTodoItemByIdSpec(Guid id)
    {
        return new BaseSpecification<TodoItem>(x => x.Id == id);
    }

    public static BaseSpecification<TodoItem> GetTodoItemsByListIdSpec(Guid id)
    {
        return new BaseSpecification<TodoItem>(x => x.List.Id == id);
    }
}
