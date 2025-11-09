using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;
using TodoApi.Services;

namespace TodoApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodoController : ControllerBase
{
    private readonly ITodoService _todoService;
    private readonly ILogger<TodoController> _logger;

    public TodoController(ITodoService todoService, ILogger<TodoController> logger)
    {
        _todoService = todoService;
        _logger = logger;
    }

    /// <summary>
    /// Get a todo item by its ID
    /// </summary>
    /// <param name="id">The ID of the todo item</param>
    /// <returns>The todo item if found, otherwise 404</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TodoItem), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TodoItem>> GetTodoItem(string id)
    {
        _logger.LogInformation("Getting todo item with id: {Id}", id);
        
        var item = await _todoService.GetTodoItemAsync(id);
        
        if (item == null)
        {
            return NotFound(new { message = $"Todo item with id '{id}' not found" });
        }

        return Ok(item);
    }

    /// <summary>
    /// Add a new todo item
    /// </summary>
    /// <param name="item">The todo item to add</param>
    /// <returns>The created todo item with generated ID</returns>
    [HttpPost]
    [ProducesResponseType(typeof(TodoItem), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TodoItem>> AddTodoItem([FromBody] TodoItem item)
    {
        if (string.IsNullOrWhiteSpace(item.Title))
        {
            return BadRequest(new { message = "Title is required" });
        }

        _logger.LogInformation("Adding new todo item: {Title}", item.Title);
        
        var createdItem = await _todoService.AddTodoItemAsync(item);
        
        return CreatedAtAction(
            nameof(GetTodoItem), 
            new { id = createdItem.Id }, 
            createdItem);
    }
}

