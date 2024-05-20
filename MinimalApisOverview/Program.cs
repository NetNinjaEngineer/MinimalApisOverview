using Microsoft.EntityFrameworkCore;
using MinimalApisOverview;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("TodoList"));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();