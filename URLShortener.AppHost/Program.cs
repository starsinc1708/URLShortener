var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
	.WithDataVolume()
	.WithPgAdmin()
	.AddDatabase("url-shortener");

var redis = builder.AddRedis("redis");

builder.AddProject<Projects.URLShortener_API>("urlshortener-api")
	.WithReference(postgres)
	.WithReference(redis)
	.WaitFor(postgres)
	.WaitFor(redis);

builder.Build().Run();
