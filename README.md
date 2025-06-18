
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

## Testing

- Swagger UI available at `https://localhost:<port>/swagger` for interactive API testing.
- Use the **Authorize** button in Swagger UI to set your JWT token.
- Login to obtain token, then test protected endpoints.
- Redis and RabbitMQ must be running for full functionality.

---

## Security & Code Quality

- Token-based authentication applied with JWT.
- Input validation via Data Annotations and model binding.
- Global exception handling returns standardized error responses with correlation IDs.
- Use of Entity Framework Core ORM to prevent SQL injection.
- Logging with Serilog including request correlation ID for traceability.
- HTTPS enforcement and CORS configured securely.
- Separation of concerns with layered architecture and DI.
- Async programming patterns applied consistently.

---

## Code Quality & Design Patterns

- Clean, modular layered architecture separating API, application, infrastructure, and persistence concerns.
- Service Manager pattern used for centralized service coordination.
- Repository pattern for data access abstraction.
- Dependency Injection used throughout.
- Async/Await used for I/O-bound operations.
- Logging and error handling implemented consistently.

---

## Contributing

Contributions are welcome! Please open issues or submit pull requests with clear descriptions.

---

## License

This project is licensed under the MIT License.

---

## Contact

For questions or support, contact [Your Name] at your.email@example.com
