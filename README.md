Product Catalog System

A .NET backend application built with Clean Architecture, Domain-Driven Design (DDD), CQRS, and MediatR, demonstrating a modular and maintainable service design.
The system manages products, orders, and payments, enforcing business rules through rich domain models and explicit state transitions.

Architecture
The solution is structured into four layers:

Domain
Contains the core business model with aggregates (Product, Order, Payment), value objects (Money, Currency, Sku), domain events, and centralized domain error definitions. All business rules and invariants are enforced here through behavior methods returning Result/Result<T>.

Application
Implements use cases using CQRS with MediatR, separating commands and queries, applying validation, and orchestrating domain operations.

Infrastructure
Provides technical implementations such as Entity Framework Core persistence, repository implementations, optimistic concurrency control, domain event dispatching, and PostgreSQL integration.

API
Exposes the system through ASP.NET Core Minimal APIs, organized by feature with standardized HTTP responses, global exception handling, and Dockerized deployment.

Key Features
Clean Architecture with strict layer boundaries
Rich domain model with entities and value objects
CQRS pattern using MediatR
Domain events for side-effect orchestration
Result pattern for predictable error handling
FluentValidation for request validation
EF Core with optimistic concurrency
PostgreSQL persistence
Containerized runtime with Docker
Consistent API error handling using ProblemDetails
