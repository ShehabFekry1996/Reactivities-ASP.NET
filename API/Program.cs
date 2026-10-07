using API.Middleware;
using Application.Activities.Commands;
using Application.Activities.Queries;
using Application.Activities.Validators;
using Application.Core;
using Application.Interfaces;
using Application.Queries;
using Domain;
using FluentValidation;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(opt =>
{
    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    opt.Filters.Add(new AuthorizeFilter(policy));
});

builder.Services.AddMediatR
    (cfg =>
    {
        cfg.RegisterServicesFromAssemblyContaining<GetActivityList.Handler>();
        cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
    });

builder.Services.AddControllers();
builder.Services.AddScoped<IUserAccessor, UserAccessor>();
builder.Services.AddDbContext<AppDBContext>(opt
    => opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfiles));
builder.Services.AddValidatorsFromAssemblyContaining<CreateActivityValidator>();
builder.Services.AddTransient<ExceptionMiddleware>(); // estansiated when it's needed 
builder.Services.AddIdentityApiEndpoints<User>(opt =>
{
    opt.User.RequireUniqueEmail = true;

}).AddRoles<IdentityRole>().AddEntityFrameworkStores<AppDBContext>();

builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("IsActivityHost", policy =>
    {
        policy.Requirements.Add(new IsHostRequirement());
    });
});
builder.Services.AddTransient<IAuthorizationHandler,IsHostRequirementHandler>();
builder.Services.AddCors();
var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();// should be above everything else 
app.UseCors
    (options=>options.AllowAnyHeader().AllowAnyMethod().AllowCredentials()
    .WithOrigins("http://localhost:3001","https://localhost:3001"));

// Configure the HTTP request pipeline.

//Redirects https requests to http
app.UseHttpsRedirection();

app.UseAuthentication(); // must be above Authorization 
app.UseAuthorization();

app.MapControllers();
app.MapGroup("api").MapIdentityApi<User>(); //api/login

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

try
{
    var context = services.GetRequiredService<AppDBContext>();
    var userManager = services.GetRequiredService<UserManager<User>>();
    Console.WriteLine(context.Database.GetConnectionString());
    Console.WriteLine(Path.GetFullPath("reactivities.db"));
    await context.Database.MigrateAsync();
    await context.Database.ExecuteSqlRawAsync("PRAGMA journal_mode=DELETE;");
    await DbInitializer.SeedData(context, userManager);
}
catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred while seeding the database.");
}
app.Run();
