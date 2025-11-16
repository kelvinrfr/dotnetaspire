using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using TodoApi.Models;

namespace TodoApi.Services;

public class TodoService : ITodoService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<TodoService> _logger;
    private const string CacheKeyPrefix = "todo:";

    public TodoService(IDistributedCache cache, ILogger<TodoService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<TodoItem?> GetTodoItemAsync(string id)
    {
        try
        {
            var cacheKey = $"{CacheKeyPrefix}{id}";
            var cachedValue = await _cache.GetStringAsync(cacheKey);

            if (string.IsNullOrEmpty(cachedValue))
            {
                _logger.LogInformation("Todo item with id {Id} not found in cache", id);
                return null;
            }

            var item = JsonSerializer.Deserialize<TodoItem>(cachedValue);
            _logger.LogInformation("Retrieved todo item {Id} from Redis cache", id);
            return item;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving todo item {Id} from Redis", id);
            throw;
        }
    }

    public async Task<TodoItem> AddTodoItemAsync(TodoItem item)
    {
        try
        {
            if (string.IsNullOrEmpty(item.Id))
            {
                item.Id = Guid.NewGuid().ToString();
            }

            if (item.CreatedAt == default)
            {
                item.CreatedAt = DateTime.UtcNow;
            }

            var cacheKey = $"{CacheKeyPrefix}{item.Id}";
            var serializedItem = JsonSerializer.Serialize(item);

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24) // Items expire after 24 hours
            };

            await _cache.SetStringAsync(cacheKey, serializedItem, options);
            _logger.LogInformation("Added todo item {Id} to Redis cache", item.Id);

            return item;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding todo item to Redis");
            throw;
        }
    }
}

