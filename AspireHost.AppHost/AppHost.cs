var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache")
    .WithDataVolume(); // Persists a volume for a known resource

var pubsubEmulator = builder
    .AddDockerfile("pubsubEmulator", ".", dockerfilePath: "Gcp-Pubsub.Emulator.dockerfile")
    .WithImageTag("1.0.0")
    .WithEndpoint(name: "pubsub-emulator", port:8681, targetPort:8085);

var pubsubBrowser = builder
    .AddDockerfile("pubsubBrowser", ".", dockerfilePath: "Gcp-Pubsub.Browser.dockerfile")
    .WithImageTag("1.0.0")
    .WithEndpoint(port:57974, targetPort:80, scheme:"http")
    .WithReference(pubsubEmulator.GetEndpoint("pubsub-emulator")) // Solved CORS issue (container discovery)
    .WaitFor(pubsubEmulator);

builder.AddProject<Projects.TodoApi>("api")
    .WithHttpHealthCheck(path: "/healthz", statusCode: 200)
    .WithReference(cache)
    .WithEnvironment("ConnectionStrings__Redis", cache.Resource.ConnectionStringExpression) // Overwrites the environment variable
    .WaitFor(cache)
    .WaitFor(pubsubEmulator);

builder.Build().Run();
