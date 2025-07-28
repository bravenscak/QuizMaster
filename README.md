# Quiz Master – Backend

This is the backend API for **Quiz Master**, a platform designed to streamline the organization and participation in pub quizzes. It provides endpoints for managing users, roles, quizzes, registrations, notifications, and categories.

## 🧱 Stack
- **ASP.NET Core** (.NET 8.0)
- **Entity Framework Core** (code-first)
- **PostgreSQL** (hosted via Neon)
- **JWT** authentication
- **BCrypt** for password hashing
- **AutoMapper** for DTO mapping
- **Swagger/OpenAPI** for documentation

## 📦 Features
- Role-based access (Admin, Organizer, Participant)
- CRUD for quizzes, registrations, and results
- Secure user authentication & authorization
- Notification system for quiz updates
- RESTful API design with clear route structure

## 🔧 Setup
1. Clone the repo
2. Configure `appsettings.json` (DB connection string, JWT secret)
3. Run migrations: `dotnet ef database update`
4. Start API: `dotnet run`
5. Access API docs: `https://localhost:<port>/swagger`

## 📍 Developed as part of the final thesis at Algebra University College, 2025.
