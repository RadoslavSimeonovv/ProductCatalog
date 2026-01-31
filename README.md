# ProductCatalog

Domain layer - DDD-based Domain Layer with three main entities (Product, Order, Payment) that encapsulate all business rules through behavior methods returning Result/Result<T>, enforcing invariants and preventing invalid state transitions.
All meaningful state changes emit explicit domain events (creation, status changes, feature updates, payments) and use centralized domain error catalogs instead of exceptions for business failures.
Value Objects like Money ensure correctness and consistency across pricing, ordering, and payments, resulting in a clean, testable, and senior-level domain model.
