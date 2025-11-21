# Serialization Design

> **Navigation**: [← Index](./index.md) | [Architecture →](./architecture.md)

## Core Philosophy

FuzzyPotato is a **flexible, type-agnostic serialization library**. It works with any type and doesn't impose constraints on your data models.

## Architecture

### 1. PolymorphicBase - Optional Base Class

```csharp
public abstract class PolymorphicBase
{
    public string Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

**Purpose**: Convenience base class with common properties.
**Not Required**: Your types don't need to inherit from it.

### 2. TypeRegistry - Runtime Type Mapping

```csharp
TypeRegistry.Register<TextDocument>("text-document");
TypeRegistry.Register<ImageDocument>("image-document");
```

**Purpose**: Maps type discriminators to actual types for polymorphic serialization.
**Required For**: YAML polymorphic serialization.
**Optional For**: JSON (can use JsonPolymorphic attributes instead).

### 3. FuzzyJsonSerializer - Clean, Simple API

```csharp
public class FuzzyJsonSerializer
{
    // Single set of methods that work with ANY type
    public string Serialize<T>(T value)
    public string SerializeCollection<T>(IEnumerable<T> values)
    public T? Deserialize<T>(string json)
    public IEnumerable<T>? DeserializeCollection<T>(string json)
    public async Task SerializeToFileAsync<T>(...)
    public async Task<T?> DeserializeFromFileAsync<T>(...)
}
```

**Key Points**:
- No type constraints
- Works with primitives, POCOs, polymorphic hierarchies
- Supports System.Text.Json's JsonPolymorphic attributes
- Can use TypeRegistry with custom converter

### 4. FuzzyYamlSerializer - Polymorphic Support

```csharp
public class FuzzyYamlSerializer
{
    // Same clean API, with optional type discriminator support
    public string Serialize<T>(T value, bool useTypeDiscriminator = true)
    public string SerializeCollection<T>(IEnumerable<T> values, bool useTypeDiscriminator = true)
    public T? Deserialize<T>(string yaml, bool useTypeDiscriminator = true)
    public IEnumerable<T>? DeserializeCollection<T>(string yaml, bool useTypeDiscriminator = true)
    public async Task SerializeToFileAsync<T>(...)
    public async Task<T?> DeserializeFromFileAsync<T>(...)
}
```

**Key Points**:
- Uses TypeRegistry for polymorphic types
- Optional type discriminator wrapping
- Falls back to direct serialization if types not registered

## Usage Patterns

### Pattern 1: Simple POCO Serialization

```csharp
public class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
}

var serializer = new FuzzyJsonSerializer();
var json = serializer.Serialize(new Person { Name = "Alice", Age = 30 });
var person = serializer.Deserialize<Person>(json);
```

### Pattern 2: JSON Polymorphic (Using Attributes)

```csharp
[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(TextDocument), typeDiscriminator: "text-document")]
[JsonDerivedType(typeof(ImageDocument), typeDiscriminator: "image-document")]
public abstract class DocumentBase : PolymorphicBase
{
}

public class TextDocument : DocumentBase
{
    public string Content { get; set; }
}

// Serialization
var serializer = new FuzzyJsonSerializer();
DocumentBase doc = new TextDocument { Content = "Hello" };
var json = serializer.Serialize(doc); // Includes $type discriminator
var deserialized = serializer.Deserialize<DocumentBase>(json); // Correct type restored
```

###Pattern 3: YAML Polymorphic (Using TypeRegistry)

```csharp
// Register types
TypeRegistry.Register<TextDocument>("text-document");
TypeRegistry.Register<ImageDocument>("image-document");

// Serialization
var serializer = new FuzzyYamlSerializer();
var doc = new TextDocument { Content = "Hello" };
var yaml = serializer.Serialize(doc); // Wraps with type discriminator
var deserialized = serializer.Deserialize<TextDocument>(yaml); // Uses TypeRegistry
```

YAML Output:
```yaml
type: text-document
data:
  id: ''
  name: ''
  createdAt: 2025-11-20T18:00:00Z
  content: Hello
```

### Pattern 4: Collections

```csharp
// JSON
var docs = new List<DocumentBase> { textDoc, imageDoc };
var json = serializer.SerializeCollection(docs);
var deserialized = serializer.DeserializeCollection<DocumentBase>(json);

// YAML
TypeRegistry.Register<TextDocument>("text-document");
TypeRegistry.Register<ImageDocument>("image-document");
var yaml = serializer.SerializeCollection(docs);
var deserialized = serializer.DeserializeCollection<DocumentBase>(yaml);
```

### Pattern 5: Non-Polymorphic Objects

```csharp
// WorkflowDefinition contains a polymorphic List<NodeDefinition>
// but itself isn't polymorphic
var workflow = new WorkflowDefinition
{
    Nodes = new List<NodeDefinition> { /* various node types */ }
};

// JSON - NodeDefinition has JsonPolymorphic attributes
var json = serializer.Serialize(workflow);
var deserialized = serializer.Deserialize<WorkflowDefinition>(json);

// YAML - Register node types first
TypeRegistry.Register<CSharpNode>("csharp-node");
TypeRegistry.Register<IfElseNode>("if-else-node");
var yaml = serializer.Serialize(workflow, useTypeDiscriminator: false); // WorkflowDefinition isn't polymorphic
// But nested nodes ARE polymorphic via YamlDotNet's default serialization
```

## Design Principles

### 1. No Constraints

**Before (Bad)**:
```csharp
public string Serialize<T>(T value) where T : PolymorphicBase
```

**After (Good)**:
```csharp
public string Serialize<T>(T value)
```

### 2. Single Responsibility

- `PolymorphicBase`: Optional base class with common properties
- `TypeRegistry`: Runtime type mapping
- `FuzzyJsonSerializer`: JSON serialization
- `FuzzyYamlSerializer`: YAML serialization with TypeRegistry integration

### 3. Flexibility

- Use JsonPolymorphic attributes (compile-time) OR TypeRegistry (runtime)
- Optional type discriminators
- Works with any type hierarchy

### 4. Consistency

Both serializers have the same method signatures and behavior patterns.

## Migration Guide

### Old API → New API

| Old Method | New Method | Notes |
|------------|------------|-------|
| `Serialize<T>(T value) where T : PolymorphicBase` | `Serialize<T>(T value)` | No constraint |
| `SerializeCollection(IEnumerable<PolymorphicBase>)` | `SerializeCollection<T>(IEnumerable<T>)` | Generic base type |
| `DeserializePolymorphic(string)` | `Deserialize<TBase>(string)` | Specify base type |
| `DeserializeCollection(string)` | `DeserializeCollection<TBase>(string)` | Specify base type |
| `SerializeObject<T>(T)` | `Serialize<T>(T)` | Methods unified |
| `DeserializeObject<T>(string)` | `Deserialize<T>(string)` | Methods unified |
| `SerializeObjectToFileAsync<T>(...)` | `SerializeToFileAsync<T>(...)` | Methods unified |
| `DeserializeObjectFromFileAsync<T>(...)` | `DeserializeFromFileAsync<T>(...)` | Methods unified |

### Test Updates

**Before**:
```csharp
var json = serializer.SerializeCollection(documents); // Implicit PolymorphicBase
var deserialized = serializer.DeserializeCollection(json); // Returns IEnumerable<PolymorphicBase>
```

**After**:
```csharp
var json = serializer.SerializeCollection(documents); // T inferred from documents type
var deserialized = serializer.DeserializeCollection<DocumentBase>(json); // Explicit base type
```

## Benefits of New Design

1. **Truly Flexible**: Works with any type, not just PolymorphicBase derivatives
2. **Clean API**: Single set of methods instead of duplicates
3. **Type Safety**: Generic constraints ensure correct types
4. **No Breaking Changes for Consumers**: They define their own polymorphic hierarchies
5. **Runtime Flexibility**: TypeRegistry allows dynamic type registration
6. **Standard Compliance**: Supports System.Text.Json's JsonPolymorphic pattern

## See Also

- [Patterns](./patterns.md) - Factory pattern for runtime instantiation
- [Architecture](./architecture.md) - Project structure
- [Test Strategy](./test-strategy.md) - Testing polymorphic serialization
