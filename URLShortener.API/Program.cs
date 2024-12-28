using URLShortener.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddNpgsqlDataSource("url-shortener");

builder.AddRedisDistributedCache("redis");

builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics => metrics.AddMeter("UrlShortener.API"));

#pragma warning disable EXTEXP0018 // “ип предназначен только дл€ оценки и может быть изменен или удален в будущих обновлени€х. „тобы продолжить, скройте эту диагностику.
builder.Services.AddHybridCache();
#pragma warning restore EXTEXP0018 // “ип предназначен только дл€ оценки и может быть изменен или удален в будущих обновлени€х. „тобы продолжить, скройте эту диагностику.

builder.Services.AddOpenApi();

builder.Services.AddHostedService<PostgresDatabaseInitializer>();
builder.Services.AddScoped<UrlShorteningService>();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "OpenAPI V1");
    });
}

app.MapPost("shorten", async (string url, UrlShorteningService urlShorteningService) =>
{
    if (!Uri.TryCreate(url, UriKind.Absolute, out _))
    {
        return Results.BadRequest("Invalid URL Format.");
    }

    var shortCode = await urlShorteningService.ShortenUrl(url);

    return Results.Ok(new { shortCode });
});

app.MapGet("{shortCode}", async (string shortCode, UrlShorteningService urlShorteningService) =>
{
    var originalUrl = await urlShorteningService.GetOriginalUrl(shortCode);

    return originalUrl is null ? Results.NotFound() : Results.Redirect(originalUrl);
});

app.MapGet("urls", async (UrlShorteningService urlShorteningService) =>
{
    return Results.Ok(await urlShorteningService.GetAllUrls());
});

app.Run();