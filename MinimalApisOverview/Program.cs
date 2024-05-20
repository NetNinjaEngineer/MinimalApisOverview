using Microsoft.EntityFrameworkCore;
using MinimalApisOverview;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("TodoList"));

var app = builder.Build();

var todoItems = app.MapGroup("/todoitems");

todoItems.MapGet("/todoitems", async (ApplicationDbContext db) =>
    await db.Todos.ToListAsync());

todoItems.MapGet("/todoitems/complete", async (ApplicationDbContext db) =>
    await db.Todos.Where(t => t.IsComplete).ToListAsync());

todoItems.MapGet("/todoitems/{id}", async (int id, ApplicationDbContext db) =>
    await db.Todos.FindAsync(id)
        is Todo todo
            ? Results.Ok(todo)
            : Results.NotFound());

todoItems.MapPost("/todoitems", async (Todo todo, ApplicationDbContext db) =>
{
    db.Todos.Add(todo);
    await db.SaveChangesAsync();

    return Results.Created($"/todoitems/{todo.Id}", todo);
});

todoItems.MapPut("/todoitems/{id}", async (int id, Todo inputTodo, ApplicationDbContext db) =>
{
    var todo = await db.Todos.FindAsync(id);

    if (todo is null) return Results.NotFound();

    todo.Name = inputTodo.Name;
    todo.IsComplete = inputTodo.IsComplete;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

todoItems.MapDelete("/todoitems/{id}", async (int id, ApplicationDbContext db) =>
{
    if (await db.Todos.FindAsync(id) is Todo todo)
    {
        db.Todos.Remove(todo);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    return Results.NotFound();
});

app.Run();