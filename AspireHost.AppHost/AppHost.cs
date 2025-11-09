var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache")
    .WithDataVolume(); // Persists a volume for a known resource

builder.AddProject<Projects.TodoApi>("api")
    .WithHttpHealthCheck(path: "/healthz", statusCode: 200)
    .WithReference(cache)
    .WithEnvironment("ConnectionStrings__Redis", cache.Resource.ConnectionStringExpression) // Overwrites the environment variable
    .WaitFor(cache);

builder.Build().Run();
