using Application.Activities.Queries;
using Application.Core;
using Application.Queries;
using Microsoft.EntityFrameworkCore;
using Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<GetActivityList.Handler>());

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDBContext>(opt
    => opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfiles)); 
builder.Services.AddCors();
var app = builder.Build();

app.UseCors
    (options=>options.AllowAnyHeader().AllowAnyMethod()
    .WithOrigins("http://localhost:3001","https://localhost:3001"));

// Configure the HTTP request pipeline.

//Redirects https requests to http
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

try
{
    var context = services.GetRequiredService<AppDBContext>();
    Console.WriteLine(context.Database.GetConnectionString());
    Console.WriteLine(Path.GetFullPath("reactivities.db"));
    await context.Database.MigrateAsync();
    await context.Database.ExecuteSqlRawAsync("PRAGMA journal_mode=DELETE;");
    await DbInitializer.SeedData(context);
}
catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred while seeding the database.");
}
app.Run();
