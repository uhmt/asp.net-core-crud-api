using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TodoDb>(options => options.UseSqlite(builder.Configuration.GetConnectionString("Todos") ?? "Data Source=todos.db"));
var app = builder.Build();
using (var scope = app.Services.CreateScope()) scope.ServiceProvider.GetRequiredService<TodoDb>().Database.EnsureCreated();
app.MapGet("/", () => "Task API: /todos");
app.MapGet("/todos", async (TodoDb db) => Results.Ok(await db.Todos.AsNoTracking().ToListAsync()));
app.MapGet("/todos/{id:int}", async (int id, TodoDb db) => await db.Todos.FindAsync(id) is TodoItem item ? Results.Ok(item) : Results.NotFound());
app.MapPost("/todos", async (TodoInput input, TodoDb db) =>
{
    if (!Valid(input)) return Results.BadRequest("A name and a due date today or later are required.");
    var item = new TodoItem { Name = input.Name.Trim(), DueDate = input.DueDate, IsCompleted = input.IsCompleted };
    db.Todos.Add(item); await db.SaveChangesAsync(); return Results.Created($"/todos/{item.Id}", item);
});
app.MapPut("/todos/{id:int}", async (int id, TodoInput input, TodoDb db) =>
{
    var item = await db.Todos.FindAsync(id); if (item is null) return Results.NotFound();
    if (string.IsNullOrWhiteSpace(input.Name) || input.Name.Length > 200 || input.DueDate == default) return Results.BadRequest("A name (1-200 characters) and a due date are required.");
    item.Name = input.Name.Trim(); item.DueDate = input.DueDate; item.IsCompleted = input.IsCompleted;
    await db.SaveChangesAsync(); return Results.Ok(item);
});
app.MapDelete("/todos/{id:int}", async (int id, TodoDb db) =>
{
    var item = await db.Todos.FindAsync(id); if (item is null) return Results.NotFound();
    db.Todos.Remove(item); await db.SaveChangesAsync(); return Results.NoContent();
});
app.Run();
static bool Valid(TodoInput input) => !string.IsNullOrWhiteSpace(input.Name) && input.Name.Length <= 200 && input.DueDate.Date >= DateTime.UtcNow.Date;
public record TodoInput(string Name, DateTime DueDate, bool IsCompleted);
public class TodoItem { public int Id { get; set; } public string Name { get; set; } = ""; public DateTime DueDate { get; set; } public bool IsCompleted { get; set; } }
public class TodoDb(DbContextOptions<TodoDb> options) : DbContext(options) { public DbSet<TodoItem> Todos => Set<TodoItem>(); }
