# Usage Guide

> **Navigation**: [← Index](./index.md) | [Architecture →](./architecture.md) | [Test Strategy →](./test-strategy.md)

**Related Documents**:
- [Architecture](./architecture.md) - Build and deployment details
- [Patterns](./patterns.md) - Implementation patterns used in examples
- [Test Strategy](./test-strategy.md) - How to test your usage
- [Design Principles](./design.md) - Best practices for API usage

---

## Getting Started

### Prerequisites

- .NET SDK 8.0 or later
- Git (for versioning)
- IDE: Visual Studio 2022, VS Code, or Rider

### Initial Setup

```bash
# Clone the repository
git clone <repository-url>
cd fuzzy-potato

# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run tests
dotnet test
```

## Development Workflow

### Creating a New Library Project

```bash
# Create the library
dotnet new classlib -n FuzzyPotato.NewFeature -o src/FuzzyPotato.NewFeature

# Create corresponding tests
dotnet new mstest -n FuzzyPotato.NewFeature.Tests -o src/FuzzyPotato.NewFeature.Tests

# Add to solution
dotnet sln add src/FuzzyPotato.NewFeature/FuzzyPotato.NewFeature.csproj
dotnet sln add src/FuzzyPotato.NewFeature.Tests/FuzzyPotato.NewFeature.Tests.csproj
```

### Configure the Project Files

**Library Project** (`src/FuzzyPotato.NewFeature/FuzzyPotato.NewFeature.csproj`):
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <IsPackable>true</IsPackable>
  </PropertyGroup>
</Project>
```

**Test Project** (`src/FuzzyPotato.NewFeature.Tests/FuzzyPotato.NewFeature.Tests.csproj`):
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <IsTestProject>true</IsTestProject>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\FuzzyPotato.NewFeature\FuzzyPotato.NewFeature.csproj" />
  </ItemGroup>

  <ItemGroup>
    <Using Include="Microsoft.VisualStudio.TestTools.UnitTesting" />
  </ItemGroup>
</Project>
```

Note: Remove redundant properties that are already in `Directory.Build.props`:
- TargetFramework(s)
- LangVersion
- Nullable
- ImplicitUsings (if you want it globally)

### Adding NuGet Packages

1. **Add package version** to `Directory.Packages.props`:
```xml
<ItemGroup>
  <PackageVersion Include="Newtonsoft.Json" Version="13.0.3" />
</ItemGroup>
```

2. **Reference in project** (no version attribute):
```xml
<ItemGroup>
  <PackageReference Include="Newtonsoft.Json" />
</ItemGroup>
```

3. **Restore and build**:
```bash
dotnet restore
dotnet build
```

### Updating Package Versions

Update version once in `Directory.Packages.props`:
```xml
<PackageVersion Include="Newtonsoft.Json" Version="13.0.4" />
```

All projects using this package will get the new version.

## Building

### Build Configurations

```bash
# Debug build (default)
dotnet build

# Release build
dotnet build --configuration Release

# Clean before build
dotnet clean && dotnet build
```

### Build for Specific Framework

```bash
# If project targets multiple frameworks
dotnet build --framework net8.0
```

## Testing

### Run All Tests

```bash
# Run all tests
dotnet test

# With detailed output
dotnet test --verbosity normal

# With code coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Run Specific Tests

```bash
# By test project
dotnet test src/FuzzyPotato.Core.Tests/FuzzyPotato.Core.Tests.csproj

# By filter (class name)
dotnet test --filter "FullyQualifiedName~UserServiceTests"

# By filter (test method)
dotnet test --filter "Name~GetUserAsync"

# By test category
dotnet test --filter "TestCategory=Integration"
```

### Watch Mode

```bash
# Auto-run tests on file changes
dotnet watch test --project src/FuzzyPotato.Core.Tests/FuzzyPotato.Core.Tests.csproj
```

## Packaging

### Create NuGet Package

```bash
# Pack all packable projects
dotnet pack --configuration Release

# Pack specific project
dotnet pack src/FuzzyPotato.Core/FuzzyPotato.Core.csproj --configuration Release

# Specify output directory
dotnet pack --configuration Release --output ./packages
```

### Package Versioning

Version is managed by Nerdbank.GitVersioning based on:
- `version.json` base version
- Git commit height
- Git tags

```bash
# Install nbgv tool
dotnet tool install -g nbgv

# Check current version
nbgv get-version

# Create version tag
git tag v1.0.0
git push origin v1.0.0
```

## CI/CD Usage

### Build Workflow

Triggered on:
- Push to `main` branch
- Pull requests
- Version tags (v*)

Runs:
- Restore dependencies
- Build (Debug & Release on Ubuntu & Windows)
- Test with auto-detection
- Pack NuGet packages (Release/Ubuntu)
- Publish to NuGet (if API key configured)

### Release Workflow

Manual trigger from GitHub Actions:
1. Go to Actions → Release
2. Click "Run workflow"
3. Optionally specify version (or use GitVersioning)
4. Optionally provide release notes
5. Workflow creates GitHub release with NuGet packages

### Setting up NuGet Publishing

Add `NUGET_API_KEY` secret in GitHub:
1. Go to repository Settings → Secrets → Actions
2. Add secret `NUGET_API_KEY` with your NuGet.org API key

Update `NUGET_SOURCE` in workflows if publishing to private feed.

## Code Quality

### EditorConfig

The project uses `.editorconfig` for consistent coding style:
- Automatically applies in Visual Studio, VS Code (with extension), Rider
- Enforces naming conventions, indentation, line endings
- Warnings for violations

### Analyzers

Microsoft.CodeAnalysis.NetAnalyzers is enabled for code quality checks.

View warnings/errors:
```bash
dotnet build /warnaserror
```

## Debugging

### Debug in IDE

**Visual Studio / Rider**: Open solution, F5 to debug

**VS Code**:
1. Install C# Dev Kit extension
2. Open folder
3. F5 or use Run and Debug panel

### Debug Tests

```bash
# VS Code: Click "Debug Test" CodeLens above test method
# Visual Studio: Right-click test → Debug Test(s)
# Rider: Click debug icon next to test
```

## Common Tasks

### Add New Class

```csharp
// src/FuzzyPotato.Core/Services/UserService.cs
namespace FuzzyPotato.Core.Services
{
    using FuzzyPotato.Core.Models;

    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository)
        {
            this._repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<User?> GetUserAsync(int id, CancellationToken cancellationToken = default)
        {
            return await this._repository.GetByIdAsync(id, cancellationToken);
        }
    }
}
```

### Add Tests for New Class

```csharp
// src/FuzzyPotato.Core.Tests/Services/UserServiceTests.cs
namespace FuzzyPotato.Core.Tests.Services
{
    using FuzzyPotato.Core.Models;
    using FuzzyPotato.Core.Services;
    using Moq;

    [TestClass]
    public class UserServiceTests
    {
        [TestMethod]
        public async Task GetUserAsync_ValidId_ReturnsUser()
        {
            // Arrange
            var expectedUser = new User { Id = 1, Email = "test@example.com" };
            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(expectedUser);
            var service = new UserService(mockRepo.Object);

            // Act
            var result = await service.GetUserAsync(1);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedUser);
        }
    }
}
```

### Add Dependency Injection Configuration

```csharp
// Startup.cs or Program.cs
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFuzzyPotatoCore(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configuration
        services.Configure<ServiceOptions>(
            configuration.GetSection(ServiceOptions.SectionName));

        // Services
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();

        return services;
    }
}
```

## Troubleshooting

### Restore Failures

```bash
# Clear NuGet caches
dotnet nuget locals all --clear

# Force restore
dotnet restore --force
```

### Build Errors

```bash
# Clean obj/bin folders
dotnet clean

# Rebuild
dotnet build --no-incremental
```

### Test Discovery Issues

```bash
# Clear test cache
dotnet test --no-build --nologo -- RunConfiguration.DisableAppDomain=true
```

### Version Conflicts

Check for package version conflicts:
```bash
# List all package references
dotnet list package

# Check for outdated packages
dotnet list package --outdated
```

## Best Practices

1. **Always run tests before committing**
   ```bash
   dotnet test && git commit
   ```

2. **Keep projects minimal**
   - Only project-specific properties in `.csproj`
   - Use centralized configuration

3. **Write tests alongside code**
   - Create test file when creating source file
   - Follow AAA pattern

4. **Use meaningful commit messages**
   - Affects versioning via GitVersioning
   - Helps with changelog generation

5. **Update documentation**
   - Keep README current
   - Document public APIs with XML comments

6. **Code review checklist**
   - Tests pass locally
   - Code follows .editorconfig
   - No compiler warnings
   - XML docs for public APIs
   - Added necessary package versions to Directory.Packages.props

## Performance Profiling

### BenchmarkDotNet

```csharp
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

public class PerformanceTests
{
    [Benchmark]
    public void Method1()
    {
        // Code to benchmark
    }

    [Benchmark]
    public void Method2()
    {
        // Alternative implementation
    }
}

// Run in Release mode
// dotnet run -c Release
```

## IDE-Specific Tips

### Visual Studio

- Use Test Explorer (Test → Test Explorer)
- Code coverage: Test → Analyze Code Coverage
- Live Unit Testing: Test → Live Unit Testing

### VS Code

- Install C# Dev Kit extension
- Install .NET Core Test Explorer
- Use CodeLens "Run Test" / "Debug Test" above test methods

### Rider

- Built-in test runner and coverage
- Right-click test/class → Run Tests / Cover Tests
- Use dotMemory/dotTrace for profiling

## Reference

- [.NET Documentation](https://learn.microsoft.com/en-us/dotnet/)
- [MSTest Documentation](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-with-mstest)
- [Moq Documentation](https://github.com/devlooped/moq)
- [FluentAssertions Documentation](https://fluentassertions.com/introduction)
- [Nerdbank.GitVersioning](https://github.com/dotnet/Nerdbank.GitVersioning)

---

> **Navigation**: [← Index](./index.md) | [Architecture →](./architecture.md) | [Test Strategy →](./test-strategy.md)

**Quick Links**:
- See [Architecture](./architecture.md) for build system details
- See [Patterns](./patterns.md) for design patterns used in examples
- See [Test Strategy](./test-strategy.md) for testing guidelines
- Check [Prompt History](../prompts/) for development timeline
