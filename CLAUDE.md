# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**AcaHelpAPI** is an ASP.NET Core 8 REST API backend built with a layered architecture. It provides user authentication (JWT), user management, and question/answer functionality for an academy help platform.

## Technology Stack

- **Framework**: ASP.NET Core 8 (.NET 8.0)
- **Database**: SQL Server with Entity Framework Core 8
- **Authentication**: JWT Bearer tokens (configured in Program.cs)
- **Password Hashing**: BCrypt.Net-Next
- **API Documentation**: Swagger/OpenAPI (Swashbuckle)
- **Containerization**: Docker (Linux target)

## Architecture

### Layered Structure

1. **Controllers** (`/Controllers`) - HTTP request handlers
   - `AuthController` - Login/JWT token generation
   - `UsersController` - User CRUD operations
   - `QuestionsController` - Question/answer management
   - Model state validation returns standardized `ApiResponse<T>` with camelCase JSON

2. **Data Access** (`/Data`)
   - `MiDbContext` - Entity Framework DbContext with Users and Questions DbSets
   - Connection string: configured in appsettings.json (Development) and appsettings.Development.json

3. **Models** (`/Models`)
   - `User` - Entity with Id, Name, LastName, Email, HashedPassword, CreatedAt, UpdatedAt
   - `Question` - Entity for Q&A functionality

4. **DTOs** (`/DTOs`)
   - `CreateUserDTO` - User registration payload
   - `LoginDTO` - Email/password login
   - `CreateQuestionDTO` - Question creation payload
   - `LoginResponseDTO` - Login response with token and timestamp

5. **Helpers** (`/Helpers`)
   - `ApiResponse<T>` - Standardized response wrapper with Success, Message, MessageCode, Data, Errors, Timestamp
   - Factory methods: `SuccessResponse()` and `ErrorResponse()` with optional error details

### Key Configuration

- **Program.cs** - Configures services including:
  - DbContext with SQL Server
  - JWT authentication scheme with token validation (issuer, audience, expiry checks)
  - CORS policy "DevPolicy" allowing all origins (dev-only setting)
  - Controllers with camelCase JSON serialization
  - Custom model state error handling returning ApiResponse format
- **JWT Config** (appsettings.json): Issuer="AcaHelp-API", Audience="AcaHelp-Client", 60-minute token expiry
- **HTTPS Redirection** enabled
- **Swagger** available in Development environment

## Common Development Commands

### Build & Run
```bash
dotnet build
dotnet run
```

### Entity Framework Migrations
```bash
# Add migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update

# Revert last migration
dotnet ef migrations remove
```

### Open API/Swagger
When running locally, navigate to `https://localhost:5001/swagger/index.html` to view and test API endpoints.

### Environment Configuration
- **Development**: Uses appsettings.Development.json (local SQL Server Integrated Security)
- **Production**: Uses appsettings.json (ensure connection string and JWT key are configured)

## Important Notes

- JWT token expiry is set to 60 minutes (Program.cs, line 79)
- Password hashing uses BCrypt with verification in AuthController
- All API responses follow the `ApiResponse<T>` pattern for consistency
- CORS is permissive in dev ("DevPolicy" allows any origin/method/header) - restrict for production
- User secrets are configured (UserSecretsId: e1f255ce-563f-412c-80bc-eaabc51b77d0)
- JSON serialization uses camelCase naming policy for API responses
- HTTP status codes: 200 for success, 401 for invalid credentials, 500 for server errors
