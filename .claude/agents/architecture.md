# Architecture Context

> **Navigation**: [← Index](./index.md) | [Design Principles →](./design.md) | [Patterns →](./patterns.md)

**Related Documents**:
- [Design Principles](./design.md) - Coding standards that align with this architecture
- [Patterns](./patterns.md) - Implementation patterns for this architecture
- [Test Strategy](./test-strategy.md) - Testing approach within this architecture

---

## Project Structure

This project follows a clean, modular architecture based on the [csharp-template](https://github.com/smartpcr/csharp-template).

### Directory Organization

```
fuzzy-potato/
├── src/                    # All source code and test projects
│   ├── FuzzyPotato.Core/   # Core library projects
│   └── FuzzyPotato.Core.Tests/  # Test projects alongside their targets
├── docs/                   # Project documentation
└── .github/workflows/      # CI/CD automation
```

### Key Architectural Decisions

1. **Single Source Folder**: All projects (libraries and tests) live under `src/` to keep the structure flat and maintainable.

2. **Central Package Management (CPM)**: All NuGet package versions are managed centrally in `Directory.Packages.props`:
   - Ensures consistent versioning across all projects
   - Simplifies dependency updates
   - Projects reference packages without version attributes

3. **Centralized Build Properties**: `Directory.Build.props` defines shared configuration:
   - Target frameworks (net8.0)
   - Language version (latest C#)
   - Nullable reference types (enabled)
   - Documentation generation
   - Debug/Release configurations
   - Test project dependencies

4. **GitVersioning**: Uses Nerdbank.GitVersioning for automatic semantic versioning based on Git history.

## Build System

### MSBuild Hierarchy

1. `Directory.Build.props` - Imported at the start of every project build
2. Individual `.csproj` files - Project-specific settings only
3. Test projects automatically get test dependencies via conditional ItemGroup

### Target Frameworks

- Primary: .NET 8.0
- Extensible via `TargetFrameworks` property in `Directory.Build.props`

## Testing Architecture

### Framework

- **MSTest** - Primary test framework
- **FluentAssertions** - Assertion library for readable test code
- **Moq** - Mocking framework for dependencies
- **coverlet.collector** - Code coverage collection

### Test Organization

- Tests live alongside the code they test in `src/`
- Test projects marked with `<IsTestProject>true</IsTestProject>`
- Automatic test discovery in CI/CD

## CI/CD Architecture

### Build Workflow

Multi-matrix builds across:
- **OS**: Ubuntu & Windows
- **Configuration**: Debug & Release

Stages:
1. Checkout with full Git history (for versioning)
2. Setup .NET SDK
3. Restore dependencies
4. Build
5. Auto-detect and run tests
6. Package (Release/Ubuntu only)
7. Publish to NuGet (if secrets configured)

### Release Workflow

Manual trigger with:
- Optional version override
- Optional release notes
- Automatic version from GitVersioning
- GitHub release creation with artifacts

## Configuration Management

### Settings Hierarchy

1. **Solution-level**: `global.json` (SDK version)
2. **Build-level**: `Directory.Build.props` (shared properties)
3. **Package-level**: `Directory.Packages.props` (dependency versions)
4. **Project-level**: Individual `.csproj` files (minimal, project-specific only)

### Code Style

- `.editorconfig` enforces consistent style across IDEs
- Naming conventions for fields, properties, methods
- C# language features and patterns
- File headers with copyright notice

## Extensibility Points

### Adding New Projects

1. Create project in `src/` folder
2. Add project-specific properties (IsPackable, IsTestProject, etc.)
3. Add to solution
4. Centralized dependencies apply automatically

### Adding New Dependencies

1. Add `<PackageVersion>` to `Directory.Packages.props`
2. Reference in projects without version: `<PackageReference Include="PackageName" />`
3. Version managed centrally

### Customization

Update these files to customize for your needs:
- `Directory.Build.props`: Product name, author, company, repository URL
- `version.json`: Initial version, versioning strategy
- `.github/workflows/`: CI/CD behavior, triggers, matrix configurations

---

> **Navigation**: [← Index](./index.md) | [Design Principles →](./design.md) | [Patterns →](./patterns.md)
