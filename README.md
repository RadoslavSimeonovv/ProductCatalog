 ProductCatalog

  A full-stack e-commerce backend API built with .NET 10 and ASP.NET Core Minimal APIs, designed as a learning project targeting senior-level .NET engineering         patterns.
  A product catalog and order management system for a tech store — supporting product and category management, customer orders, and payment processing.

  Tech Stack
  .NET 10 
  ·ASP.NET Core Minimal APIs
  ·PostgreSQL 
  ·EF Core 10 
  ·MediatR
  ·FluentValidation 
  ·Keycloak 
  ·Redis 
  ·Serilog
  ·Quartz.NET 
  ·Docker

  Features & Patterns

  Architecture
  - Clean Architecture with strict layer separation (Domain → Application → Infrastructure → API)
  - Domain-Driven Design — rich aggregates (Product, Order, Payment), value objects (Money, Currency, Sku, CustomerId), domain events
  - CQRS via MediatR — separate command/query handlers per use case
  - Result pattern — Result<T> + typed errors instead of exceptions in domain/application layers
  - Repository pattern with optimistic concurrency (xmin rowversion on all aggregates)

  Infrastructure
  - JWT Bearer authentication via Keycloak (OIDC, role-based policies)
  - Transactional Outbox pattern — domain events persisted atomically with aggregate changes, processed by a Quartz.NET background job
  - Redis distributed caching with MediatR pipeline opt-in (ICachedQuery) and domain-event-driven cache invalidation
  - Structured logging with Serilog, Seq sink, correlation ID and user context enrichment
  - Health checks for PostgreSQL, Redis, and Keycloak
  - API versioning (/api/v1/)

  Domain
  - Product catalog — products, categories, features, lifecycle status
  - Orders — creation, submission for payment, cancellation, resource ownership enforcement
  - Payments — initiation, success/failure simulation, idempotency key support
