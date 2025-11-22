# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

FuzzyPotato is a .NET library for JSON/YAML serialization with polymorphic type support. The library uses a custom converter architecture with TypeRegistry for runtime type resolution, enabling serialization of complex object hierarchies with base and derived types across assembly boundaries.



## Essential Commands

### Build and Test
```bash
# Restore dependencies
dotnet restore

# Build all projects
dotnet build

# Build without restore (faster for iterative development)
dotnet build --no-restore

# Run all tests
dotnet test

# Run tests without rebuilding
dotnet test --no-build

# Run specific test class
dotnet test --filter "FullyQualifiedName~FuzzyJsonSerializerTests"

# Run single test method
dotnet test --filter "FullyQualifiedName~FuzzyJsonSerializerTests.Serialize_TextDocument_ContainsTypeDiscriminator"

# Clean build artifacts
dotnet clean
```

### Package Creation
```bash
# Create NuGet package
dotnet pack --configuration Release
```

## Critical Architecture Concepts

### Polymorphic Serialization Architecture

**DO NOT use `JsonPolymorphic` attributes** - The library uses a custom converter approach instead.

#### The TypeName Property Pattern
Each polymorphic type declares its discriminator via the abstract `TypeName` property:
- **JSON**: Custom `PolymorphicJsonConverterFactory` automatically detects PolymorphicBase-derived types
- **YAML**: Custom `PolymorphicYamlNodeDeserializer` resolves types by scanning assemblies
- Both use `$type` discriminator property for type identification
- Discriminator value comes from instance's `TypeName` property

```csharp
// Define polymorphic type with TypeName property
public class TextDocument : DocumentBase
{
    public override string TypeName => "text-document";
    // ... properties
}

// Optional: Register for faster lookup (type is auto-discovered from TypeName)
TypeRegistry.Register<TextDocument>();

// Serialization - automatically adds $type discriminator from TypeName
var json = serializer.Serialize(document);

// Deserialization - automatically resolves type from $type via assembly scanning
var result = serializer.Deserialize<DocumentBase>(json);
```

#### Custom Converter Implementation (JSON)

**`PolymorphicJsonConverterFactory`** (`src/FuzzyPotato.Core/Serialization/PolymorphicJsonConverter.cs`):
- Registered in `FuzzyJsonSerializer` constructor
- Intercepts serialization for types in TypeRegistry
- Creates `PolymorphicJsonConverter<T>` instances dynamically
- **Critical**: Excludes `object` type to prevent `Dictionary<string, object>` issues
- Handles nested polymorphic collections recursively

**`PolymorphicJsonConverter<TBase>`**:
- Writes `$type` property during serialization
- Reads `$type` and resolves actual type during deserialization
- Clears converters list when recursing to prevent infinite loops

#### Custom Deserializer Implementation (YAML)

**`PolymorphicYamlNodeDeserializer`** (`src/FuzzyPotato.Core/Serialization/PolymorphicYamlTypeInspector.cs`):
- Registered via `InsteadOf<ObjectNodeDeserializer>` in `FuzzyYamlSerializer`
- **Buffers YAML parsing events** to look ahead for `$type` discriminator
- Filters out `$type` from buffered events before replaying (prevents "property not found" errors)
- **Critical**: Must delegate to `_originalDeserializer` for non-polymorphic types (not return `false`)

**`PolymorphicYamlSerializingTypeInspector`**:
- Adds synthetic `$type` property descriptor during serialization
- Uses `TypeDiscriminatorPropertyDescriptor` as read-only property

### Nested Polymorphic Collections

The library handles complex scenarios like:
```csharp
public class WorkflowDefinition
{
    public List<NodeDefinition> Nodes { get; set; } // NodeDefinition is abstract
}
```

**How it works**:
1. When serializing `Nodes` list, System.Text.Json/YamlDotNet processes each element
2. Custom converters detect each `NodeDefinition` is polymorphic
3. Each concrete type (CSharpNode, HttpRequestNode, etc.) gets `$type` added
4. On deserialization, `$type` is read first to determine concrete type
5. Object is deserialized to correct type, not abstract base

## Code Style Requirements

### EditorConfig Enforcement
The project has strict `.editorconfig` rules:

**C# Files (`*.cs`)**:
- Indent: 4 spaces
- Private fields: `_camelCase` (underscore prefix)
- Static fields: `PascalCase`
- Const fields: `PascalCase`
- Using directives: Inside namespace (enforced as warning)
- `this.` qualifier required for all members (enforced as warning)

**Other Files**:
- Indent: 2 spaces
- End of line: CRLF (except shell scripts which use LF)

### File Headers
All C# files must include this header:
```csharp
// -----------------------------------------------------------------------
// <copyright file="{fileName}" company="FuzzyPotato">
//     Copyright (c) FuzzyPotato. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
```

## Testing Strategy

### Test Organization
- Tests live in `src/FuzzyPotato.Core.Tests/` alongside the library
- Example implementations in `src/FuzzyPotato.Core.Tests/Examples/` (Document, Workflow, WorkflowRuntime)
- Test namespace: `FuzzyPotato.Core.Tests.*`

### Type Registration in Tests
**OPTIONAL**: Types are auto-discovered, but you can pre-register for faster lookup in `[ClassInitialize]`:

```csharp
[ClassInitialize]
public static void ClassInitialize(TestContext context)
{
    // Optional: Pre-register types for faster first access
    TypeRegistry.Register<TextDocument>();
    TypeRegistry.Register<ImageDocument>();
    TypeRegistry.Register<CSharpNode>();
    // Discriminators are automatically retrieved from TypeName property
}
```

### Test Patterns
- Use AAA pattern (Arrange, Act, Assert)
- FluentAssertions for readable assertions: `.Should().Be()`, `.Should().HaveCount()`
- MSTest attributes: `[TestClass]`, `[TestMethod]`, `[TestInitialize]`, `[ClassInitialize]`

## Common Development Scenarios

### Adding a New Polymorphic Type

1. Create the derived class with TypeName property (no attributes needed):
```csharp
public class CustomNode : NodeDefinition
{
    public override string TypeName => "custom-node";

    public string CustomProperty { get; set; }
}
```

2. The custom converters will automatically discover and handle serialization/deserialization

3. (Optional) Pre-register for faster first access:
```csharp
TypeRegistry.Register<CustomNode>();  // Automatically uses "custom-node" from TypeName
```

### Modifying Serializers

**When modifying `FuzzyJsonSerializer` or `FuzzyYamlSerializer`**:
- Keep API simple - just delegate to System.Text.Json/YamlDotNet
- Don't add manual type handling - converters handle it automatically
- Ensure `JsonSerializerOptions` includes custom converters in constructor

**When modifying custom converters**:
- Test with nested polymorphic collections
- Test with `Dictionary<string, object>` containing primitives
- Verify `$type` is filtered correctly (YAML) or written first (JSON)
- Ensure proper delegation to original deserializers

### Debugging Serialization Issues

1. Verify TypeName property is implemented correctly (returns kebab-case discriminator)
2. Verify `$type` property appears in serialized output
3. For YAML: Ensure `$type` is being filtered from buffered events
4. For JSON: Check `CanConvert()` isn't excluding the type
5. Add breakpoints in custom converter `Read()` and `Write()` methods

## Detailed Documentation

**IMPORTANT**: The following documentation files contain in-depth technical details. Read them when working on related tasks:

### Core Architecture & Design
- **[`.claude/agents/index.md`](.claude/agents/index.md)** - Navigation hub and documentation overview
  - Read this first when exploring the documentation system

- **[`.claude/agents/architecture.md`](.claude/agents/architecture.md)** - Build system, CI/CD, project structure
  - **Read when**: Modifying build configuration, adding projects, changing dependencies, setting up CI/CD

- **[`.claude/agents/design.md`](.claude/agents/design.md)** - SOLID principles, async patterns, error handling guidelines
  - **Read when**: Implementing new features, refactoring code, establishing coding standards

- **[`.claude/agents/serialization-design.md`](.claude/agents/serialization-design.md)** - Deep dive into polymorphic serialization architecture
  - **Read when**: Debugging serialization issues, modifying converters, understanding type resolution

### Patterns & Best Practices
- **[`.claude/agents/patterns.md`](.claude/agents/patterns.md)** - Repository, Factory, Strategy, Builder patterns with examples
  - **Read when**: Adding new design patterns, refactoring to use patterns, architectural decisions

- **[`.claude/agents/test-strategy.md`](.claude/agents/test-strategy.md)** - Testing philosophy, AAA pattern, mocking strategies
  - **Read when**: Writing tests, setting up test infrastructure, debugging test failures

- **[`.claude/agents/usage-guide.md`](.claude/agents/usage-guide.md)** - IDE setup, development workflow, common tasks
  - **Read when**: Setting up development environment, troubleshooting tooling issues

### Historical Context
Prompt history in `.claude/prompts/2025/11/` documents development sessions:
- **[`.claude/prompts/2025/11/2025-11-20.md`](.claude/prompts/2025/11/2025-11-20.md)** - Session 4: Architectural redesign to custom converters
  - Why JsonPolymorphic attributes were abandoned
  - Bug fixes (dictionary iteration, object type exclusion, delegation pattern)
  - Lessons learned about cross-assembly polymorphism

- **[`.claude/prompts/2025/11/2025-11-21.md`](.claude/prompts/2025/11/2025-11-21.md)** - Session 5: TypeName-based discriminators
  - Moved from hard-coded discriminators to TypeName property pattern
  - Assembly scanning for automatic type discovery
  - Simplified TypeRegistry.Register() API

**Note**: When encountering complex issues or making architectural decisions, consult these documents to understand the rationale behind current design choices and avoid repeating past mistakes.

## Build System Notes

- **Central Package Management (CPM)**: Versions in `Directory.Packages.props` only
- **Shared Build Config**: `Directory.Build.props` applies to all projects
- **GitVersioning**: Automatic semantic versioning via `version.json`
- Test projects auto-include MSTest/FluentAssertions/Moq via conditional ItemGroup

## Key Pitfalls to Avoid

1. **Never use JsonPolymorphic attributes** - Use TypeName property + custom converters
2. **Always implement TypeName property** in polymorphic types (abstract enforces this)
3. **TypeName must return kebab-case string** (e.g., "text-document", not "TextDocument")
4. **Filter TypeName property from serialization** - both "TypeName" and "typeName" (camelCase)
5. **YAML deserializers must delegate, not return false** when they can't handle a type
6. **Filter `$type` from YAML events** before replaying to prevent property errors
7. **Exclude `object` type from JSON converter** to handle Dictionary<string, object>
8. **Clear converters list when recursing** to prevent infinite loops
9. **Always use `this.` qualifier** for class members (EditorConfig enforced)
10. **Use 4-space indentation for C#**, 2-space for everything else
