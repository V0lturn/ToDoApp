# ToDoApp — Task Management System

A full-stack task management application built using .NET Core Web API and Angular. The project implements a 4-layer architecture on the backend and a responsive user interface based on Bootstrap.

---

## Features

* Task Management (CRUD): Create, view, edit, and delete personal tasks.
* Category Management: Organize tasks into custom categories with duplicate prevention per user.
* Server-side Pagination, Search, and Filtering.
* User Authentication with JWT tokens and password hashing using BCrypt.

---

## Technology Stack

### Backend

* .NET 8 / ASP.NET Core Web API
* Entity Framework Core
* Microsoft SQL Server
* JWT Bearer Authentication
* BCrypt.Net

### Frontend

* Angular (Standalone Components)
* Reactive Forms
* Bootstrap 5
* Bootstrap Icons

---

## Architecture (4-Layer Pattern)

The backend follows a 4-layer architecture:

1. **Controllers** – Handle HTTP requests and delegate execution to the business layer.
2. **Services** – Contain business logic, validation, and DTO mapping.
3. **Interfaces** – Define contracts for services and repositories.
4. **Infrastructure / Data Access** – Handle database interaction through EF Core and repositories.

---

## Local Database Setup

The project uses Entity Framework Core Code-First migrations.

Configure the connection string in `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_LOCAL_SERVER;Database=ToDoAppDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

Run migrations:

```powershell
Update-Database
```

---
