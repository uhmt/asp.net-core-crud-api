var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var todos = new List<TodoItem>();

app.MapGet("/", () => "HelloWorld");

app.MapGet("/todos", () => 
{
    if (!todos.Any())
    {
        return Results.NotFound("Aun no has agregado tareas o ya no quedan");
    }
    else 
    {
        return Results.Ok(todos);
    }
});

app.MapGet("/todos/{id}", (int id) =>
{
    if (todos.Any(todo => todo.Id == id))
    {
        return Results.NotFound($"La tarea con el id '{id}' no existe");
    }
    else
    {
        var todo = todos.FirstOrDefault(t => t.Id == id);
        return Results.Ok(todo);
    }
    
});

app.MapPost("/newTodo", (TodoItem task) =>
{
    if (todos.Any(todo => todo.Id == task.Id))
    {
        return Results.Conflict("La tarea que estas agregando no puede tener un id existente, prueba con un id diferente");
    }
    else
    {
        todos.Add(task);
        return TypedResults.Created($"/todos/{task.Id}", task);
    }
    
});

app.MapDelete("/todos/{id}", (int id) =>
{
    var todo = todos.FirstOrDefault(TodoItem => TodoItem.Id == id);
    if (todo is not null)
    {
        todos.Remove(todo);
        return Results.NoContent();
    }
    return Results.NotFound();
});
app.Run();

public record TodoItem
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public DateTime DueDate { get; init; }
    public bool IsCompleted { get; init; }
}
