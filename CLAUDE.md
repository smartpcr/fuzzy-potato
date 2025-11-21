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

#### The TypeRegistry Pattern
TypeRegistry is the single source of truth for type resolution:
- **JSON**: Custom `PolymorphicJsonConverterFactory` automatically detects TypeRegistry-registered types
- **YAML**: Custom `PolymorphicYamlNodeDeserializer` resolves types at parse time
- Both use `$type` discriminator property for type identification

```csharp
// Type registration (required for YAML, automatic for JSON via custom converters)
TypeRegistry.Register<TextDocument>("text-document");

// Serialization - automatically adds $type discriminator
var json = serializer.Serialize(document);

// Deserialization - automatically resolves type from $type
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
**CRITICAL**: All polymorphic types must be registered in `[ClassInitialize]`:

```csharp
[ClassInitialize]
public static void ClassInitialize(TestContext context)
{
    TypeRegistry.Register<TextDocument>("text-document");
    TypeRegistry.Register<ImageDocument>("image-document");
    TypeRegistry.Register<CSharpNode>("csharp-node");
    // ... register all polymorphic types used in tests
}
```

### Test Patterns
- Use AAA pattern (Arrange, Act, Assert)
- FluentAssertions for readable assertions: `.Should().Be()`, `.Should().HaveCount()`
- MSTest attributes: `[TestClass]`, `[TestMethod]`, `[TestInitialize]`, `[ClassInitialize]`

## Common Development Scenarios

### Adding a New Polymorphic Type

1. Create the derived class (no attributes needed):
```csharp
public class CustomNode : NodeDefinition
{
    public string CustomProperty { get; set; }
}
```

2. Register in TypeRegistry where needed:
```csharp
TypeRegistry.Register<CustomNode>("custom-node");
```

3. The custom converters will automatically handle serialization/deserialization

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

1. Check TypeRegistry has all types registered
2. Verify `$type` property appears in serialized output
3. For YAML: Ensure `$type` is being filtered from buffered events
4. For JSON: Check `CanConvert()` isn't excluding the type
5. Add breakpoints in custom converter `Read()` and `Write()` methods

## Documentation References

Comprehensive developer documentation in `.claude/agents/`:
- **index.md** - Navigation hub for all documentation
- **architecture.md** - Build system, CI/CD, project structure
- **design.md** - SOLID principles, async patterns, error handling
- **patterns.md** - Repository, Factory, Strategy, Builder patterns
- **test-strategy.md** - Testing philosophy, AAA pattern, mocking
- **usage-guide.md** - IDE setup, development workflow, common tasks

Prompt history in `.claude/prompts/2025/11/2025-11-20.md` documents:
- Session 4: Complete architectural redesign to custom converters
- Why JsonPolymorphic attributes were abandoned
- Bug fixes (dictionary iteration, object type exclusion, delegation pattern)
- Lessons learned about cross-assembly polymorphism

## Build System Notes

- **Central Package Management (CPM)**: Versions in `Directory.Packages.props` only
- **Shared Build Config**: `Directory.Build.props` applies to all projects
- **GitVersioning**: Automatic semantic versioning via `version.json`
- Test projects auto-include MSTest/FluentAssertions/Moq via conditional ItemGroup

## Key Pitfalls to Avoid

1. **Never use JsonPolymorphic attributes** - Use TypeRegistry + custom converters
2. **Don't forget TypeRegistry.Register()** in test ClassInitialize
3. **YAML deserializers must delegate, not return false** when they can't handle a type
4. **Filter `$type` from YAML events** before replaying to prevent property errors
5. **Exclude `object` type from JSON converter** to handle Dictionary<string, object>
6. **Clear converters list when recursing** to prevent infinite loops
7. **Always use `this.` qualifier** for class members (EditorConfig enforced)
8. **Use 4-space indentation for C#**, 2-space for everything else
