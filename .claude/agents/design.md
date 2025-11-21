# Design Principles

> **Navigation**: [← Index](./index.md) | [Architecture →](./architecture.md) | [Patterns →](./patterns.md)

**Related Documents**:
- [Architecture](./architecture.md) - System structure supporting these principles
- [Patterns](./patterns.md) - Concrete implementations of these principles
- [Test Strategy](./test-strategy.md) - Testing practices aligned with these principles

---

## Core Design Philosophy

This project adheres to proven software design principles to ensure maintainability, testability, and scalability.

### SOLID Principles

1. **Single Responsibility Principle (SRP)**
   - Each class should have one reason to change
   - Separate concerns into focused, cohesive units

2. **Open/Closed Principle (OCP)**
   - Open for extension, closed for modification
   - Use interfaces and abstractions for extensibility

3. **Liskov Substitution Principle (LSP)**
   - Derived classes must be substitutable for their base classes
   - Maintain behavioral compatibility in inheritance hierarchies

4. **Interface Segregation Principle (ISP)**
   - Clients should not depend on interfaces they don't use
   - Prefer small, focused interfaces over large, monolithic ones

5. **Dependency Inversion Principle (DIP)**
   - Depend on abstractions, not concretions
   - High-level modules should not depend on low-level modules

## Code Organization

### Namespace Strategy

- Root namespace: `FuzzyPotato`
- Sub-namespaces by feature/domain
- Test namespaces mirror source namespaces with `.Tests` suffix

### File Organization

- One public type per file
- File name matches type name
- Private/internal helper classes may share files with their primary class

## Coding Standards

### Enforced via .editorconfig

1. **Naming Conventions**
   - PascalCase: Public members, types, methods, properties, events
   - camelCase: Private instance fields, local variables, parameters
   - PascalCase: Static fields and const fields (no prefixes)

2. **Null Handling**
   - Nullable reference types enabled project-wide
   - Explicit null checks where nullability is expected
   - Use null-coalescing (`??`) and null-conditional (`?.`) operators

3. **var Usage**
   - Prefer `var` when type is obvious from right-hand side
   - Explicit types for clarity when type is not apparent

4. **Using Directives**
   - Inside namespace (enforced as warning)
   - System directives first
   - Separated groups: System, External, Internal

### Documentation

- XML documentation required for public APIs
- Warning CS1591 suppressed at project level (can be re-enabled)
- Document intent, not implementation
- Examples for complex APIs

## Error Handling

### Exception Strategy

- Use built-in exception types where appropriate
- Create custom exceptions for domain-specific errors
- Include meaningful error messages
- Preserve stack traces when rethrowing

### Validation

- Guard clauses at method entry points
- Fail fast with ArgumentException, ArgumentNullException
- Domain validation in entity/value object constructors

## Asynchronous Programming

### Async/Await Guidelines

- Async all the way (avoid sync-over-async)
- ConfigureAwait(false) in library code
- CancellationToken support for long-running operations
- Avoid async void (except event handlers)

## Dependency Injection

### DI Strategy

- Constructor injection preferred
- Explicit dependencies
- Register services with appropriate lifetimes:
  - Singleton: Stateless services, shared state
  - Scoped: Per-request services
  - Transient: Stateful services, lightweight

## Testing Design

### Test Structure

- **Arrange-Act-Assert (AAA)** pattern
- One logical assertion per test (FluentAssertions allows multiple related assertions)
- Descriptive test names: `MethodName_Scenario_ExpectedBehavior`

### Test Doubles

- **Mocks**: Verify interactions (Moq)
- **Stubs**: Provide canned responses
- **Fakes**: Working implementations for testing

### Test Coverage

- Unit tests: Individual class behavior
- Integration tests: Component interactions
- Focus on behavior, not implementation details

## Performance Considerations

### General Guidelines

- Measure before optimizing
- Use Span<T> and Memory<T> for high-performance scenarios
- ArrayPool<T> for reducing allocations
- Benchmark with BenchmarkDotNet

### Common Optimizations

- String concatenation: Use StringBuilder for multiple operations
- Collections: Choose appropriate collection types
- LINQ: Be aware of deferred execution and multiple enumeration

## Immutability

### When to Use

- Value objects
- Data transfer objects (DTOs)
- Configuration objects
- Thread-safe shared state

### Techniques

- `readonly` fields
- Init-only properties (`init` accessor)
- Record types for immutable data structures
- ImmutableCollections for collections

## Versioning Strategy

### Semantic Versioning (SemVer)

- MAJOR: Breaking changes
- MINOR: New features (backwards-compatible)
- PATCH: Bug fixes (backwards-compatible)

### Managed by Nerdbank.GitVersioning

- Automatic version bumping based on commit height
- Version tags in Git for releases
- AssemblyVersion vs. NuGet package version precision

---

> **Navigation**: [← Index](./index.md) | [Architecture →](./architecture.md) | [Patterns →](./patterns.md)

**See Also**:
- [Patterns](./patterns.md) for concrete pattern implementations
- [Architecture](./architecture.md) for build and versioning configuration
