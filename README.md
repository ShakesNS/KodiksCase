
# KodiksCase Project

## Overview

KodiksCase is a layered .NET 8 application designed for managing orders, users, and products with secure JWT authentication, RabbitMQ-based order queuing, Redis caching, and background processing using a worker service. The project emphasizes clean architecture, modularity, and best practices in security and code quality.

---

## Architecture

- **API Layer (`KodiksCase.Api`)**  
  Handles HTTP requests and responses, exposes RESTful endpoints for users, products, and orders. Implements JWT authentication and input validation.

- **Application Layer (`KodiksCase.Application`)**  
  Contains business logic and service interfaces. Implements service manager pattern to coordinate business services.

- **Infrastructure Layer (`KodiksCase.Infrastructure`)**  
  Manages external integrations such as RabbitMQ messaging, Redis caching, and middleware implementations.

- **Persistence Layer (`KodiksCase.Persistence`)**  
  Handles data access using Entity Framework Core, repository patterns, and database migrations.

- **Worker Service (`KodiksCase.Worker`)**  
  Background service that listens to RabbitMQ queues, processes orders asynchronously, writes processing logs to Redis, and handles notification simulation.

---

## Key Components

### Service Manager

- Centralized service aggregator that exposes user, order, and product services via a single interface.
- Simplifies dependency injection and controller/service interaction.
- Registered in DI container and injected into API controllers.

### Authentication

- JWT token-based authentication securing API endpoints.
- Login and Register endpoints are open (`AllowAnonymous`), all others require authorization.
- Tokens signed with a strong secret key and validated on each request.

### Messaging & Background Processing

- RabbitMQ used for order queuing.
- Worker service consumes messages from RabbitMQ, processes orders, and logs status in Redis.
- Redis caching used for storing processing logs and caching user orders.

### Logging

- Serilog integrated for structured logging with console and rolling file sinks.
- Correlation ID included in logs for traceability across distributed components.

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- SQL Server database
- RabbitMQ server running
- Redis server running
### Technologies and Libraries Used

- **.NET 8** with ASP.NET Core Web API  
- **Entity Framework Core** for ORM and database access  
- **RabbitMQ.Client** for messaging  
- **StackExchange.Redis** for caching  
- **Serilog** for logging  
- **Swagger** for API documentation  
- **Postman** for API testing  
- **Microsoft.Extensions.Logging** for structured logging  
- **Microsoft.AspNetCore.Authentication.JwtBearer** for JWT authentication

### Configuration

Set up your `appsettings.json` with the following keys:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "your-db-connection-string",
    "Redis": "localhost:6379"
  },
  "JwtSettings": {
    "Key": "your-secure-secret-key-of-minimum-256-bits",
    "Issuer": "KodiksCaseApi",
    "Audience": "KodiksCaseClient",
    "ExpiresInMinutes": 60
  },
  "RabbitMQ": {
    "HostName": "localhost",
    "UserName": "guest",
    "Password": "guest",
    "QueueName": "order-placed"
  }
}
```

---

## Running the Project

1. **Database Migration**

   Run EF Core migrations to create the database schema:

   ```bash
   dotnet ef database update --project KodiksCase.Persistence
   ```

2. **Start API**

   Run the API project:

   ```bash
   dotnet run --project KodiksCase.Api
   ```

3. **Start Worker**

   Run the background worker service:

   ```bash
   dotnet run --project KodiksCase.Worker
   ```

---

## Usage & Testing

- Use Swagger UI at `https://localhost:<port>/swagger` for interactive API testing.
- Use the **Authorize** button in Swagger UI to set your JWT token.
- To obtain a token:
  - Use the `/api/user/login` endpoint with valid credentials.
  - Copy the returned JWT token.
  - Paste it into Swagger's **Authorize** dialog or Postman's authorization header.
- Redis and RabbitMQ must be running for full functionality.
- The Postman collection JSON file is located inside the **Postman** folder within the **API project** (`KodiksCase.Api`).  
  To use it:
  1. Open Postman.  
  2. Click the **Import** button.  
  3. Select the JSON file at `KodiksCase.Api/Postman/KodiksCase.postman_collection.json`.  
  4. The collection will be imported and ready for testing the API endpoints.

### Test User Credentials

| Full Name       | Email             | Password  |
|-----------------|-------------------|-----------|
| Kodiks Test     | test@kodiks.com   | 123456    |

- Use these credentials to log in via the `/api/user/login` endpoint and get a JWT token for testing.

### Example: Create Order Request

Endpoint: `POST /api/orders/create`

Request body example:

```json
{
  "userId": "046a3a78-5ebb-489f-b7b8-08a6839fcab2",
  "productId": "d0696b26-44b8-420c-855b-eac593582258",
  "quantity": 650,
  "paymentMethod": "BankTransfer"
}
```

---

## API Response & Error Handling

The API returns responses in a consistent JSON format with the following structure:

### Successful Response Example

```json
{
  "success": true,
  "message": "Operation completed successfully.",
  "data": {
  }
}
```
### Error  Response Example

```json
{
  "success": false,
  "message": "Validation failed.",
  "errors": [
  ]
}
```

### Explanation

- **Success Response**  
  - `success`: `true` indicates the operation succeeded.  
  - `message`: Descriptive success message.  
  - `data`: Contains returned data, can be any object or null.

- **Error Response**  
  - `success`: `false` indicates operation failure.  
  - `message`: General error message.  
  - `errors`: List of detailed error messages (validation or other).

---

## Security & Code Quality

- Token-based authentication applied with JWT.
- Input validation via Data Annotations and model binding.
- Global exception handling returns standardized error responses with correlation IDs.
- Use of Entity Framework Core ORM to prevent SQL injection.
- Logging with Serilog including request correlation ID for traceability.
- Separation of concerns with layered architecture and DI.
- Async programming patterns applied consistently.

---

## Software Architecture and Design Patterns

- Clean, modular layered architecture separating API, application, infrastructure, and persistence concerns.
- Service Manager pattern used for centralized service coordination.
- Repository pattern for data access abstraction.
- Dependency Injection used throughout.
- Async/Await used for I/O-bound operations.
- Logging and error handling implemented consistently.
