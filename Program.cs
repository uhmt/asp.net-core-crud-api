var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var todos = new List<TodoItem>();
var todosLock = new object();

app.MapGet("/", () => "API using ASP.NET Core :)");

app.MapGet("/todos", () =>
{
    lock (todosLock)
    {
        if (!todos.Any())
        {
            return Results.NotFound("No todos found. Add some tasks to get started.");
        }
        else
        {
            return Results.Ok(todos);
        }
    }
});

app.MapGet("/todos/{id}", (int id) =>
{
    lock (todosLock)
    {
        if (todos.Any(todo => todo.Id == id))
        {
            var todo = todos.FirstOrDefault(t => t.Id == id);
            return Results.Ok(todo);
        }
        else
        {
            return Results.NotFound($"Todo with the id '{id}' does not exist");
        }
    }
});

app.MapPost("/todos", (TodoItem task) =>
{
    lock (todosLock)
    {
        if (task.DueDate < DateTime.Now)
        {
            return Results.BadRequest("The DueDate cannot be in the past.");
        }
        if (todos.Any(todo => todo.Id == task.Id))
        {
            return Results.Conflict($"Todo with ID '{task.Id}' already exists. Please use a different ID.");
        }
        else
        {
            todos.Add(task);
            return TypedResults.Created($"/todos/{task.Id}", task);
        }
    }
});

app.MapDelete("/todos/{id}", (int id) =>
{
    lock (todosLock)
    {
        var todo = todos.FirstOrDefault(TodoItem => TodoItem.Id == id);
        if (todo is not null)
        {
            todos.Remove(todo);
            return Results.NoContent();
        }
        else
        {
            return Results.NotFound($"Todo with ID '{id}' does not exist.");
        }
    }
});
app.Run();

public record TodoItem
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public DateTime DueDate { get; init; }
    public bool IsCompleted { get; init; }
}
