using Microsoft.EntityFrameworkCore;

namespace MinimalApisOverview;

public static class TodoEndpoints
{
    public static void MapTodoEndpoints(this WebApplication app)
    {
        var todoItems = app.MapGroup("/todoitems");

        todoItems.MapGet("/", GetAllTodos);

        todoItems.MapGet("/complete", GetCompleteTodos);

        todoItems.MapGet("/{id}", GetTodo);

        todoItems.MapPost("/", CreateTodo);

        todoItems.MapPut("/{id}", UpdateTodo);

        todoItems.MapDelete("/{id}", DeleteTodo);
    }

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
}
