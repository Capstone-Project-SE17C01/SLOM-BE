# Clean Structured API Project - ASP.NET Core

This template is for a clean structured ASP.NET Core API project, following the RESTful principles, Clean Architecture principles, SOLID design principles, implementing the Dependency Injection, Repository, and Unit of Work design pattern, and utilizing Entity Framework Core for data access. It provides a standardized structure and organization for building robust and maintainable ASP.NET Core API applications with complete CRUD (Create, Read, Update, Delete) operations.

## Project Structure

The project structure is designed to promote separation of concerns and modularity, making it easier to understand, test, and maintain the application.

```
├── src
│   ├── Core                    # Contains the core business logic and domain models, view models, etc.
│   ├── Infrastructure          # Contains infrastructure concerns such as data access, external services, etc.
│   └── API                     # Contains the API layer, including controllers, extensions, etc.
└── README.md                   # Project documentation (you are here!)
```

## Coding Conventions

- **Namespace**: Use PascalCase (e.g., `MyApp.Domain`).
- **Class/Method Names**: PascalCase (e.g., `GetUserById`).
- **Variable Names**: camelCase (e.g., `userId`).
- **Interface Names**: Prefix with "I" (e.g., `IUserRepository`).
- **Async Methods**: End with "Async" (e.g., `GetUserAsync`).
- **Braces**: Use K&R style (`{}` on the same line as the statement).
- **Indentation**: 4 spaces, no tabs.
- **File Naming**: Match class name (e.g., `UserService.cs`).

## Git and Creating Pull Request & Rebase

1. **Create branch**
	- Create branch with name like feature/{taskId}_{nameof(Task)}
2. **Commit changes with message like**
   feature/{nameof(Task)}: do something here
3. **Rebase your branch with the latest changes from develop**
4. **Squash multiple commits into a single one**
5. **Push your changes to the remote branch**
6. **Create a Pull Request into develop**

## Usage

The [project template](https://binarybytez.com/clean-structured-api-project/) provides a starting point for building RESTful APIs using ASP.NET Core. You can modify and extend the existing code to suit your specific application requirements. Here's an overview of the key components involved in building RESTful APIs:

1. **Models**: The `Core` project contains the domain models representing the entities you want to perform CRUD operations on. Update the models or add new ones according to your domain.
2. **Repositories**: The `Infrastructure` project contains repository implementations that handle data access operations using Entity Framework Core. Modify the repositories or create new ones to match your entity models and database structure.
3. **Services**: The `Core` project contains services that encapsulate the business logic and orchestrate the operations on repositories. Update or create new services to handle CRUD operations on your entities.
4. **Controllers**: The `API` project contains controllers that handle HTTP requests and responses. Update or create new controllers to expose the CRUD endpoints for your entities. Implement the appropriate HTTP methods (GET, POST, PUT, DELETE) and perform actions on the core services accordingly.

Make sure to update the routes, validation, and error-handling logic to align with your application requirements and [best practices](https://binarybytez.com/performance-optimization-and-monitoring-in-asp-net-core/).

