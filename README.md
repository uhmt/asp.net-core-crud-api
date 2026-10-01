# Task API · ASP.NET Core

A small C# API demonstrating task creation, retrieval, update and deletion with persistent SQLite storage through Entity Framework Core.

## Run

Install the .NET 8 SDK, then:

```sh
dotnet restore
dotnet run --urls http://localhost:5080
```

The SQLite file `todos.db` is created automatically. Override `ConnectionStrings__Todos` to use a different SQLite path. Database files are excluded from Git.

| Method | Route | Purpose |
| --- | --- | --- |
| GET | `/todos` | List tasks, including an empty array |
| GET | `/todos/{id}` | Fetch a task; 404 if missing |
| POST | `/todos` | Create; server assigns the ID |
| PUT | `/todos/{id}` | Update a task |
| DELETE | `/todos/{id}` | Delete; 204 on success |

Example request body for POST/PUT:
```json
{"name":"Review documentation","dueDate":"2099-12-31","isCompleted":false}
```

Names must contain 1–200 characters. New tasks cannot have a due date before today (UTC). Updates allow past dates so an overdue task can be completed. Data survives a server restart.

## Verification

```sh
dotnet build
python3 tests/smoke.py
```

The smoke test starts an isolated instance, creates/updates/deletes a task, checks invalid requests, and verifies persistence after restart. It requires `dotnet` on PATH.

## Scope

Learning project with no authentication or authorization. Run locally; do not expose it with private data. The initial schema uses `EnsureCreated`; schema evolution would require a migration strategy before production use.
