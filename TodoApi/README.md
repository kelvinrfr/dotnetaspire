# Todo API with Redis

A .NET 8 Web API application that demonstrates Redis integration for persisting and retrieving todo items.

## Features

- **Add Todo Items**: POST endpoint to create and store todo items in Redis
- **Get Todo Items**: GET endpoint to retrieve todo items from Redis by ID
- **Redis Integration**: Uses StackExchange.Redis for distributed caching

## Prerequisites

- .NET 8 SDK
- Redis server running (default: localhost:6379)

## Getting Started

### 1. Start Redis Server

If you don't have Redis installed, you can use Docker:

```bash
docker run -d -p 6379:6379 redis:latest
```

Or install Redis locally and start the service.

### 2. Configure Redis Connection

Update the `appsettings.json` file with your Redis connection string:

```json
{
  "ConnectionStrings": {
    "Redis": "localhost:6379"
  }
}
```

### 3. Run the Application

```bash
dotnet run
```

The API will be available at `http://localhost:5000` (or the port configured in `launchSettings.json`).

### 4. Access Swagger UI

Navigate to `http://localhost:5000/swagger` to view and test the API endpoints.

## API Endpoints

### POST /api/todo

Add a new todo item to Redis.

**Request Body:**
```json
{
  "title": "Complete project",
  "description": "Finish the todo API project",
  "isCompleted": false
}
```

**Response:** 201 Created
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "title": "Complete project",
  "description": "Finish the todo API project",
  "isCompleted": false,
  "createdAt": "2024-01-15T10:30:00Z"
}
```

### GET /api/todo/{id}

Retrieve a todo item from Redis by its ID.

**Response:** 200 OK
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "title": "Complete project",
  "description": "Finish the todo API project",
  "isCompleted": false,
  "createdAt": "2024-01-15T10:30:00Z"
}
```

**Response:** 404 Not Found (if item doesn't exist)
```json
{
  "message": "Todo item with id 'xxx' not found"
}
```

## Example Usage

### Using cURL

**Add a todo item:**
```bash
curl -X POST http://localhost:5000/api/todo \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Learn Redis",
    "description": "Understand Redis caching",
    "isCompleted": false
  }'
```

**Get a todo item:**
```bash
curl http://localhost:5000/api/todo/{id}
```

### Using HTTP Client

The project includes a `TodoApi.http` file that you can use with REST Client extensions in VS Code or similar tools.

## Project Structure

```
TodoApi/
├── Controllers/
│   └── TodoController.cs      # API endpoints
├── Models/
│   └── TodoItem.cs            # Todo item model
├── Services/
│   ├── ITodoService.cs        # Service interface
│   └── TodoService.cs         # Redis service implementation
├── Program.cs                  # Application configuration
├── appsettings.json           # Configuration file
└── README.md                  # This file
```

## Notes

- Todo items are stored in Redis with a 24-hour expiration time
- Items are serialized as JSON before storing in Redis
- The ID is auto-generated if not provided when creating a new item
- CreatedAt timestamp is automatically set if not provided

