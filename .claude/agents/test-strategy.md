# Test Strategy

## Testing Philosophy

This project follows a pragmatic testing approach balancing thoroughness with maintainability.

### Test Pyramid

```
        /\
       /UI\         ← Few: Critical user journeys
      /────\
     /Integ.\       ← Some: Component integration
    /────────\
   /  Unit    \     ← Many: Business logic, algorithms
  /────────────\
```

## Test Levels

### Unit Tests

**Purpose**: Test individual components in isolation

**Characteristics**:
- Fast execution (< 100ms per test)
- No external dependencies (database, network, file system)
- Deterministic and repeatable
- Test one logical concept per test

**Framework**: MSTest + FluentAssertions + Moq

**Example**:
```csharp
[TestClass]
public class CalculatorTests
{
    [TestMethod]
    public void Add_TwoPositiveNumbers_ReturnsSum()
    {
        // Arrange
        var calculator = new Calculator();

        // Act
        var result = calculator.Add(2, 3);

        // Assert
        result.Should().Be(5);
    }
}
```

### Integration Tests

**Purpose**: Test component interactions

**Characteristics**:
- Slower than unit tests
- May use real dependencies (in-memory database, test containers)
- Test realistic scenarios
- Validate component contracts

**Setup**:
- Use TestContainers for databases
- In-memory implementations for external services
- Separate test project if needed

**Example**:
```csharp
[TestClass]
public class UserRepositoryIntegrationTests
{
    private DbContext _context = null!;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
    }

    [TestMethod]
    public async Task GetByIdAsync_ExistingUser_ReturnsUser()
    {
        // Arrange
        var user = new User { Id = 1, Email = "test@example.com" };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var repository = new UserRepository(_context);

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be("test@example.com");
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Dispose();
    }
}
```

### End-to-End Tests

**Purpose**: Test complete user workflows

**When**: Critical business scenarios only

## Test Organization

### File Structure

```
src/
├── FuzzyPotato.Core/
│   ├── Services/
│   │   └── UserService.cs
│   └── FuzzyPotato.Core.csproj
└── FuzzyPotato.Core.Tests/
    ├── Services/
    │   └── UserServiceTests.cs
    └── FuzzyPotato.Core.Tests.csproj
```

### Naming Conventions

**Test Classes**: `{ClassUnderTest}Tests`
- Example: `UserServiceTests`

**Test Methods**: `{MethodName}_{Scenario}_{ExpectedBehavior}`
- Example: `GetUserAsync_ValidId_ReturnsUser`
- Example: `CreateUserAsync_DuplicateEmail_ThrowsArgumentException`

**Test Projects**: `{ProjectName}.Tests`
- Example: `FuzzyPotato.Core.Tests`

## Testing Practices

### AAA Pattern (Arrange-Act-Assert)

```csharp
[TestMethod]
public async Task ProcessOrderAsync_ValidOrder_UpdatesInventory()
{
    // Arrange
    var mockInventory = new Mock<IInventoryService>();
    var service = new OrderService(mockInventory.Object);
    var order = new Order { ProductId = 1, Quantity = 5 };

    // Act
    await service.ProcessOrderAsync(order);

    // Assert
    mockInventory.Verify(x => x.UpdateStockAsync(1, -5, It.IsAny<CancellationToken>()), Times.Once);
}
```

### Test Data Management

#### Use Builders for Complex Objects

```csharp
public class OrderBuilder
{
    private int _id = 1;
    private List<OrderItem> _items = new();

    public OrderBuilder WithId(int id)
    {
        _id = id;
        return this;
    }

    public OrderBuilder WithItem(string productId, int quantity)
    {
        _items.Add(new OrderItem(productId, quantity));
        return this;
    }

    public Order Build() => new() { Id = _id, Items = _items };
}

// Usage
var order = new OrderBuilder()
    .WithId(123)
    .WithItem("PROD-1", 2)
    .WithItem("PROD-2", 1)
    .Build();
```

#### Use AutoFixture/Bogus for Test Data

```csharp
// Using Bogus
var faker = new Faker<User>()
    .RuleFor(u => u.Id, f => f.Random.Int(1, 1000))
    .RuleFor(u => u.Email, f => f.Internet.Email())
    .RuleFor(u => u.Name, f => f.Name.FullName());

var testUsers = faker.Generate(10);
```

### Mocking Strategy

#### When to Mock

- External dependencies (HTTP clients, databases)
- Slow operations
- Non-deterministic behavior (time, random)

#### When NOT to Mock

- Value objects
- DTOs
- Simple data structures
- Logic you're testing

#### Mock Setup Examples

```csharp
// Return value
mockRepository
    .Setup(x => x.GetByIdAsync(123, It.IsAny<CancellationToken>()))
    .ReturnsAsync(new User { Id = 123, Name = "Test" });

// Throw exception
mockValidator
    .Setup(x => x.ValidateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
    .ThrowsAsync(new ValidationException("Invalid order"));

// Verify interaction
mockLogger.Verify(
    x => x.Log(
        LogLevel.Error,
        It.IsAny<EventId>(),
        It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error")),
        It.IsAny<Exception>(),
        It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
    Times.Once);
```

## Assertions

### FluentAssertions Best Practices

```csharp
// Value assertions
result.Should().Be(expected);
result.Should().BeGreaterThan(0);
result.Should().NotBeNull();

// String assertions
message.Should().StartWith("Error");
message.Should().Contain("user not found", "because user ID was invalid");

// Collection assertions
users.Should().HaveCount(3);
users.Should().Contain(u => u.Email == "test@example.com");
users.Should().BeInAscendingOrder(u => u.Name);

// Exception assertions
var act = async () => await service.GetUserAsync(-1);
await act.Should().ThrowAsync<ArgumentOutOfRangeException>()
    .WithMessage("*must be positive*");

// Object comparison
result.Should().BeEquivalentTo(expected, options => options
    .Excluding(u => u.Id)
    .Excluding(u => u.CreatedAt));
```

## Test Coverage

### Coverage Goals

- **Critical Paths**: 100% coverage
- **Business Logic**: 80%+ coverage
- **Infrastructure**: 60%+ coverage
- **UI/Presentation**: Focus on integration tests

### Measuring Coverage

```bash
# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Generate report (using ReportGenerator)
reportgenerator -reports:**/coverage.cobertura.xml -targetdir:coverage-report -reporttypes:Html
```

### Coverage in CI

- Build workflow runs coverage collection
- Coverage reports uploaded as artifacts
- Can integrate with Codecov/Coveralls if desired

## Test Performance

### Parallel Execution

MSTest runs tests in parallel by default. Control with:

```csharp
[TestClass]
[DoNotParallelize]  // Disable parallelization for this class
public class DatabaseTests
{
}
```

### Test Fixtures for Expensive Setup

```csharp
[TestClass]
public class ServiceTests
{
    private static ServiceFixture _fixture = null!;

    [ClassInitialize]
    public static void ClassSetup(TestContext context)
    {
        _fixture = new ServiceFixture();  // Expensive setup once per class
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        _fixture.Dispose();
    }

    [TestMethod]
    public void Test1()
    {
        // Use _fixture
    }
}
```

## Continuous Testing

### Local Development

```bash
# Watch mode - re-run tests on file change
dotnet watch test

# Filter tests
dotnet test --filter "FullyQualifiedName~UserService"
```

### CI Pipeline

1. All tests run on every PR
2. Build matrix tests multiple OS platforms
3. Auto-detect test projects
4. Fail fast on test failures

## Test Documentation

### Documenting Test Intent

```csharp
/// <summary>
/// Verifies that the service correctly handles concurrent requests
/// without data corruption or race conditions.
/// </summary>
[TestMethod]
public async Task ProcessAsync_ConcurrentRequests_NoDataCorruption()
{
    // Test implementation
}
```

### Test Categories

```csharp
[TestClass]
public class UserServiceTests
{
    [TestMethod]
    [TestCategory("Unit")]
    public void ValidateEmail_InvalidFormat_ReturnsFalse()
    {
    }

    [TestMethod]
    [TestCategory("Integration")]
    public async Task SaveUserAsync_ValidUser_PersistsToDatabase()
    {
    }
}

// Run specific category
// dotnet test --filter TestCategory=Unit
```

## Best Practices Summary

1. **Write tests first** (TDD) or alongside code
2. **One assertion per test** (logical concept)
3. **Descriptive test names** - explain what's being tested
4. **Fast tests** - keep unit tests under 100ms
5. **Isolated tests** - no shared state between tests
6. **Deterministic** - same input = same output always
7. **Readable** - tests are documentation
8. **Maintainable** - refactor tests as code evolves
9. **Coverage ≠ Quality** - focus on meaningful tests
10. **Test behavior, not implementation** - avoid brittle tests
