using Microsoft.EntityFrameworkCore;
using MinimalApisOverview;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("TodoList"));

var app = builder.Build();

app.MapGet("/", (LinkGenerator linkGenerator, HttpContext context) =>
    linkGenerator.GetUriByName(context, "getAllTodos", values: null));

app.MapTodoEndpoints();

app.Run();