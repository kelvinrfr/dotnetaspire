var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("redis")
    .WithDataVolume(); // Persists a volume for a known resource

var mysqlPassword =builder.AddParameter("mysqlRootPassword", secret: true);

// The MySql version used by Aspire is controlled by the .WithImage() call,
// which defaults to "library/mysql" with tag "9.5" unless overridden.
// To explicitly set the MySQL version, use WithImage and specify the desired tag.
// Expose MySQL on host port 33306 so you can connect with tools like DBeaver
var mysql = builder.AddMySql("mysql", password: mysqlPassword, port: 33322)
    .WithImage("library/mysql", "9.5") // Specify MySQL version
    // Set MySQL container lifetime to Persistent.
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume()
    .WaitFor(mysqlPassword);

builder
    .AddDockerfile("flyway", ".", "Flyway.Database-Migrations.dockerfile")
    .WithImageTag("1.0.0")
    .WithBindMount("../Migrations", "/flyway/sql")
    .WaitFor(mysql)
    .WithReference(mysql);
    
var pubsubEmulator = builder
    .AddDockerfile("pubsubEmulator", ".", dockerfilePath: "Gcp-Pubsub.Emulator.dockerfile")
    .WithImageTag("1.0.0")
    .WithEndpoint(name: "pubsub-emulator", port:8681, targetPort:8085);

builder
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
    .WaitFor(pubsubEmulator)
    .WithReference(mysql)
    .WaitFor(mysql);

builder.Build().Run();
