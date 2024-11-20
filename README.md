# ASP.NET Core CRUD API

A simple CRUD API built with ASP.NET Core for managing tasks. This API allows you to create, read, update, and delete tasks with basic validation.

## Features

- Add new tasks with unique IDs.
- Fetch all tasks or a specific task by ID.
- Update task details.
- Delete tasks.
- Friendly error handling.

## Technologies

- **ASP.NET Core 7.0**
- **C#**
- **In-memory list** for storing tasks (no database yet).

## Endpoints

### GET `/todos`

- Retrieves all tasks.
- If no tasks are available, returns a friendly message.

### GET `/todos/{id}`

- Fetches a task by its `id`.
- Returns `404 Not Found` if the task does not exist.

### POST `/newTodo`

- Adds a new task.
- Requires the following JSON format:
  ```json
  {
    "id": 1,
    "name": "Example Task",
    "dueDate": "2024-11-20",
    "isCompleted": false
  }
  ```
