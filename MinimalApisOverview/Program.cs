using Microsoft.EntityFrameworkCore;
using MinimalApisOverview;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("TodoList"));

var app = builder.Build();

var todoItems = app.MapGroup("/todoitems");

todoItems.MapGet("/todoitems", GetAllTodos);

todoItems.MapGet("/todoitems/complete", GetCompleteTodos);

todoItems.MapGet("/todoitems/{id}", GetTodo);

todoItems.MapPost("/todoitems", CreateTodo);

todoItems.MapPut("/todoitems/{id}", UpdateTodo);

todoItems.MapDelete("/todoitems/{id}", DeleteTodo);

app.Run();

#region Route handler methods
static async Task<IResult> GetAllTodos(ApplicationDbContext db)
{
    return TypedResults.Ok(await db.Todos.ToArrayAsync());
}

static async Task<IResult> GetCompleteTodos(ApplicationDbContext db)
{
    return TypedResults.Ok(await db.Todos.Where(t => t.IsComplete).ToArrayAsync());
}

static async Task<IResult> GetTodo(int id, ApplicationDbContext db)
{
    return await db.Todos.FindAsync(id)
            is Todo todo
                ? TypedResults.Ok(todo)
                : TypedResults.NotFound();
}

static async Task<IResult> CreateTodo(Todo todo, ApplicationDbContext db)
{
    db.Todos.Add(todo);
    await db.SaveChangesAsync();

    return TypedResults.Created($"/todoitems/{todo.Id}", todo);
}

static async Task<IResult> UpdateTodo(int id, Todo inputTodo, ApplicationDbContext db)
{
    var todo = await db.Todos.FindAsync(id);

    if (todo is null) return TypedResults.NotFound();

    todo.Name = inputTodo.Name;
    todo.IsComplete = inputTodo.IsComplete;

    await db.SaveChangesAsync();

    return TypedResults.NoContent();
}

static async Task<IResult> DeleteTodo(int id, ApplicationDbContext db)
{
    if (await db.Todos.FindAsync(id) is Todo todo)
    {
        db.Todos.Remove(todo);
        await db.SaveChangesAsync();
        return TypedResults.NoContent();
    }

    return TypedResults.NotFound();
}
#endregion