using Application;
using Asp.Versioning;
using Asp.Versioning.Conventions;
using Infrastructure;
using Infrastructure.Data;
using WebApi;
using WebApi.Middleware;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddOpenApi();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
}).AddMvc(options => { options.Conventions.Add(new VersionByNamespaceConvention()); }).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'V";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy<string>("fixed", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromSeconds(10),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 2
            }));
});

var app = builder.Build();

// DB init (seed)
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    // dbContext.Database.Migrate(); // todo
    ApplicationDbInitializer.Initialize(dbContext);
}

app.UseCustomExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();

// CORS 
app.UseCors(policy => policy
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();

app.MapControllers()
    .RequireRateLimiting("fixed");

app.Run();