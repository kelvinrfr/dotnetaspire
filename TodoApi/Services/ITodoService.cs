using TodoApi.Models;

namespace TodoApi.Services;

public interface ITodoService
{
    Task<TodoItem?> GetTodoItemAsync(string id);
    Task<TodoItem> AddTodoItemAsync(TodoItem item);
}

