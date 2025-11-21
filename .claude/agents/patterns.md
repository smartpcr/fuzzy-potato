# Design Patterns & Best Practices

## ⚠️ CRITICAL: EditorConfig Compliance

**ALWAYS follow the coding patterns defined in `.editorconfig`**

Before writing ANY code, review and strictly adhere to:

1. **File Headers**: Every C# file MUST start with the copyright header template:
   ```csharp
   // -----------------------------------------------------------------------
   // <copyright file="{fileName}" company="FuzzyPotato">
   //     Copyright (c) FuzzyPotato. All rights reserved.
   // </copyright>
   // -----------------------------------------------------------------------
   ```

2. **Indentation**:
   - C# files: 4 spaces (tab width 4)
   - Other files: 2 spaces (tab width 2)

3. **Naming Conventions**:
   - Static fields: PascalCase
   - Const fields: PascalCase
   - Private instance fields: camelCase
   - Always use `this.` qualifier for fields, properties, methods, events

4. **Using Directives**:
   - Place inside namespace (enforced as warning)
   - System directives first
   - No separation between groups

5. **Code Style**:
   - Prefer `var` when type is apparent
   - End of line: CRLF (except .sh, .bash which use LF)
   - Trim trailing whitespace

**This is non-negotiable. Code that doesn't follow .editorconfig will be rejected.**

## Common Patterns

### Repository Pattern

**When to Use**: Data access layer abstraction

```csharp
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
```

### Unit of Work Pattern

**When to Use**: Coordinating multiple repositories in a transaction

```csharp
public interface IUnitOfWork : IDisposable
{
    IRepository<TEntity> Repository<TEntity>() where TEntity : class;
    Task<int> CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);
}
```

### Factory Pattern

**When to Use**: Complex object creation logic

```csharp
public interface IServiceFactory<TService>
{
    TService Create(string serviceType);
}

public class ServiceFactory<TService> : IServiceFactory<TService>
{
    private readonly IServiceProvider _serviceProvider;

    public ServiceFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public TService Create(string serviceType)
    {
        // Factory logic
    }
}
```

### Strategy Pattern

**When to Use**: Runtime algorithm selection

```csharp
public interface IProcessingStrategy
{
    Task<ProcessingResult> ProcessAsync(ProcessingContext context, CancellationToken cancellationToken);
}

public class ProcessingOrchestrator
{
    private readonly Dictionary<ProcessingType, IProcessingStrategy> _strategies;

    public ProcessingOrchestrator(IEnumerable<IProcessingStrategy> strategies)
    {
        _strategies = strategies.ToDictionary(s => s.ProcessingType, s => s);
    }

    public Task<ProcessingResult> ProcessAsync(ProcessingType type, ProcessingContext context, CancellationToken cancellationToken)
    {
        if (!_strategies.TryGetValue(type, out var strategy))
        {
            throw new NotSupportedException($"Processing type {type} is not supported.");
        }

        return strategy.ProcessAsync(context, cancellationToken);
    }
}
```

### Builder Pattern

**When to Use**: Complex object construction with many optional parameters

```csharp
public class ConfigurationBuilder
{
    private string? _connectionString;
    private int _timeout = 30;
    private bool _enableRetry;

    public ConfigurationBuilder WithConnectionString(string connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        return this;
    }

    public ConfigurationBuilder WithTimeout(int timeout)
    {
        if (timeout <= 0) throw new ArgumentOutOfRangeException(nameof(timeout));
        _timeout = timeout;
        return this;
    }

    public ConfigurationBuilder EnableRetry()
    {
        _enableRetry = true;
        return this;
    }

    public Configuration Build()
    {
        if (string.IsNullOrEmpty(_connectionString))
        {
            throw new InvalidOperationException("Connection string is required.");
        }

        return new Configuration(_connectionString, _timeout, _enableRetry);
    }
}
```

### Options Pattern

**When to Use**: Configuration settings (Microsoft.Extensions.Options)

```csharp
public class ServiceOptions
{
    public const string SectionName = "ServiceSettings";

    public string ApiKey { get; set; } = string.Empty;
    public int MaxRetries { get; set; } = 3;
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
}

// Registration
services.Configure<ServiceOptions>(configuration.GetSection(ServiceOptions.SectionName));

// Usage
public class MyService
{
    private readonly ServiceOptions _options;

    public MyService(IOptions<ServiceOptions> options)
    {
        _options = options.Value;
    }
}
```

### Decorator Pattern

**When to Use**: Adding behavior to objects dynamically

```csharp
public interface IDataProcessor
{
    Task<ProcessedData> ProcessAsync(RawData data, CancellationToken cancellationToken);
}

public class LoggingDataProcessorDecorator : IDataProcessor
{
    private readonly IDataProcessor _inner;
    private readonly ILogger<LoggingDataProcessorDecorator> _logger;

    public LoggingDataProcessorDecorator(IDataProcessor inner, ILogger<LoggingDataProcessorDecorator> logger)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ProcessedData> ProcessAsync(RawData data, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing data: {DataId}", data.Id);
        var result = await _inner.ProcessAsync(data, cancellationToken);
        _logger.LogInformation("Completed processing data: {DataId}", data.Id);
        return result;
    }
}
```

## Functional Programming Patterns

### Result Type (Railway-Oriented Programming)

**When to Use**: Error handling without exceptions

```csharp
public readonly record struct Result<T>
{
    public T? Value { get; init; }
    public string? Error { get; init; }
    public bool IsSuccess { get; init; }

    public static Result<T> Success(T value) => new() { Value = value, IsSuccess = true };
    public static Result<T> Failure(string error) => new() { Error = error, IsSuccess = false };
}

public class ValidationService
{
    public Result<User> ValidateUser(UserInput input)
    {
        if (string.IsNullOrEmpty(input.Email))
        {
            return Result<User>.Failure("Email is required");
        }

        if (!IsValidEmail(input.Email))
        {
            return Result<User>.Failure("Invalid email format");
        }

        return Result<User>.Success(new User(input.Email));
    }
}
```

### Option/Maybe Type

**When to Use**: Representing optional values

```csharp
public readonly record struct Option<T>
{
    private readonly T? _value;
    public bool HasValue { get; }

    private Option(T value)
    {
        _value = value;
        HasValue = true;
    }

    public static Option<T> Some(T value) => new(value);
    public static Option<T> None() => default;

    public TResult Match<TResult>(Func<T, TResult> some, Func<TResult> none) =>
        HasValue ? some(_value!) : none();
}
```

## Anti-Patterns to Avoid

### God Class
- **Problem**: Class with too many responsibilities
- **Solution**: Apply SRP, break into focused classes

### Primitive Obsession
- **Problem**: Using primitives instead of value objects
- **Solution**: Create domain-specific types (Email, CustomerId, etc.)

### Leaky Abstractions
- **Problem**: Implementation details bleeding through abstractions
- **Solution**: Design interfaces around client needs, not implementation

### Service Locator
- **Problem**: Hidden dependencies, difficult testing
- **Solution**: Use constructor injection

### Anemic Domain Model
- **Problem**: Domain objects with no behavior (just data)
- **Solution**: Rich domain models with encapsulated business logic

## Async Patterns

### Async Iterator Pattern

```csharp
public async IAsyncEnumerable<T> GetItemsAsync<T>([EnumeratorCancellation] CancellationToken cancellationToken = default)
{
    await foreach (var item in _repository.GetAllAsync(cancellationToken))
    {
        yield return item;
    }
}
```

### Parallel Processing with Channels

```csharp
public async Task ProcessInParallelAsync<T>(
    IEnumerable<T> items,
    Func<T, CancellationToken, Task> processor,
    int maxDegreeOfParallelism,
    CancellationToken cancellationToken)
{
    var channel = Channel.CreateUnbounded<T>();

    // Producer
    var producer = Task.Run(async () =>
    {
        foreach (var item in items)
        {
            await channel.Writer.WriteAsync(item, cancellationToken);
        }
        channel.Writer.Complete();
    }, cancellationToken);

    // Consumers
    var consumers = Enumerable.Range(0, maxDegreeOfParallelism)
        .Select(async _ =>
        {
            await foreach (var item in channel.Reader.ReadAllAsync(cancellationToken))
            {
                await processor(item, cancellationToken);
            }
        })
        .ToArray();

    await Task.WhenAll(producer);
    await Task.WhenAll(consumers);
}
```

## Testing Patterns

### Object Mother Pattern

**When to Use**: Creating test data

```csharp
public static class UserMother
{
    public static User CreateDefault() => new()
    {
        Id = 1,
        Email = "test@example.com",
        Name = "Test User"
    };

    public static User CreateWithEmail(string email) => CreateDefault() with { Email = email };
}
```

### Test Data Builder Pattern

```csharp
public class UserBuilder
{
    private int _id = 1;
    private string _email = "test@example.com";
    private string _name = "Test User";

    public UserBuilder WithId(int id)
    {
        _id = id;
        return this;
    }

    public UserBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public User Build() => new() { Id = _id, Email = _email, Name = _name };
}
```
