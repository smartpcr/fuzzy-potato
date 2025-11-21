# FuzzyPotato

**A powerful .NET library for JSON/YAML serialization with polymorphic type support.**

FuzzyPotato provides seamless serialization and deserialization of polymorphic data structures - base classes with common properties and derived classes with specific properties - in both JSON and YAML formats.

## Features

- **Polymorphic Serialization**: Automatically handles base and derived types
- **JSON Support**: Built on System.Text.Json with type discriminators
- **YAML Support**: YamlDotNet integration with type registry
- **Type Safety**: Strong typing with compile-time checks
- **Async File I/O**: Async methods for file operations
- **Extensible**: Easy to add custom derived types

## Quick Start

### Installation

```bash
dotnet add package FuzzyPotato.Core
```

### Basic Usage

```csharp
using FuzzyPotato.Core.Models.Examples;
using FuzzyPotato.Core.Serialization;

// Create polymorphic objects
var documents = new List<PolymorphicBase>
{
    new TextDocument
    {
        Id = "1",
        Name = "My Document",
        Content = "Hello World!",
        WordCount = 2
    },
    new ImageDocument
    {
        Id = "2",
        Name = "My Image",
        ImageUrl = "https://example.com/image.png",
        Width = 1920,
        Height = 1080
    }
};

// JSON Serialization
var jsonSerializer = new FuzzyJsonSerializer();
var json = jsonSerializer.SerializeCollection(documents);
Console.WriteLine(json);

// Deserialize back
var deserializedDocs = jsonSerializer.DeserializeCollection(json);

// YAML Serialization
TypeRegistry.Register<TextDocument>("text-document");
TypeRegistry.Register<ImageDocument>("image-document");

var yamlSerializer = new FuzzyYamlSerializer();
var yaml = yamlSerializer.SerializeCollection(documents);
Console.WriteLine(yaml);
```

### JSON Output Example

```json
[
  {
    "$type": "text-document",
    "id": "1",
    "name": "My Document",
    "createdAt": "2025-11-20T17:00:00Z",
    "content": "Hello World!",
    "wordCount": 2,
    "language": "en"
  },
  {
    "$type": "image-document",
    "id": "2",
    "name": "My Image",
    "createdAt": "2025-11-20T17:00:00Z",
    "imageUrl": "https://example.com/image.png",
    "width": 1920,
    "height": 1080,
    "format": "png"
  }
]
```

### YAML Output Example

```yaml
- type: text-document
  data:
    id: '1'
    name: My Document
    createdAt: 2025-11-20T17:00:00Z
    content: Hello World!
    wordCount: 2
    language: en
- type: image-document
  data:
    id: '2'
    name: My Image
    createdAt: 2025-11-20T17:00:00Z
    imageUrl: https://example.com/image.png
    width: 1920
    height: 1080
    format: png
```

## Creating Custom Types

### Define Your Type

```csharp
using System.Text.Json.Serialization;
using FuzzyPotato.Core.Models;

[JsonDerivedType(typeof(CustomDocument), typeDiscriminator: "custom-document")]
public class CustomDocument : PolymorphicBase
{
    public string CustomProperty { get; set; } = string.Empty;
    public int CustomValue { get; set; }
}
```

### Register for YAML (JSON handled by attribute)

```csharp
TypeRegistry.Register<CustomDocument>("custom-document");
```

### Use It

```csharp
var custom = new CustomDocument
{
    Id = "custom-1",
    Name = "Custom",
    CustomProperty = "Value",
    CustomValue = 42
};

var json = jsonSerializer.Serialize(custom);
var yaml = yamlSerializer.Serialize(custom);
```

## API Reference

### FuzzyJsonSerializer

| Method | Description |
|--------|-------------|
| `Serialize<T>(T value)` | Serializes object to JSON string |
| `SerializeCollection(IEnumerable<PolymorphicBase>)` | Serializes collection to JSON |
| `Deserialize<T>(string json)` | Deserializes JSON to typed object |
| `DeserializePolymorphic(string json)` | Deserializes JSON to base type |
| `DeserializeCollection(string json)` | Deserializes JSON to collection |
| `SerializeToFileAsync<T>(string, T, CancellationToken)` | Serializes to file |
| `DeserializeFromFileAsync<T>(string, CancellationToken)` | Deserializes from file |

### FuzzyYamlSerializer

| Method | Description |
|--------|-------------|
| `Serialize<T>(T value)` | Serializes object to YAML string |
| `SerializeCollection(IEnumerable<PolymorphicBase>)` | Serializes collection to YAML |
| `Deserialize<T>(string yaml)` | Deserializes YAML to typed object |
| `DeserializePolymorphic(string yaml)` | Deserializes YAML to base type |
| `DeserializeCollection(string yaml)` | Deserializes YAML to collection |
| `SerializeToFileAsync<T>(string, T, CancellationToken)` | Serializes to file |
| `DeserializeFromFileAsync<T>(string, CancellationToken)` | Deserializes from file |

### TypeRegistry

| Method | Description |
|--------|-------------|
| `Register<T>(string discriminator)` | Registers type for YAML serialization |
| `GetType(string discriminator)` | Gets type for discriminator |
| `GetDiscriminator(Type type)` | Gets discriminator for type |
| `GetAllTypes()` | Gets all registered types |
| `Clear()` | Clears all registrations |

## Built-in Examples

FuzzyPotato includes example document types:

- **TextDocument**: Text content with word count and language
- **ImageDocument**: Images with URL, dimensions, and format
- **VideoDocument**: Videos with URL, duration, resolution, and codec

These serve as examples and can be used in your applications.

## Advanced Scenarios

### Custom JSON Options

```csharp
var jsonSerializer = new FuzzyJsonSerializer(options =>
{
    options.WriteIndented = false; // Compact JSON
    options.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
});
```

### Custom YAML Configuration

```csharp
var yamlSerializer = new FuzzyYamlSerializer(
    configureSerializer: builder =>
    {
        builder.WithIndentedSequences();
    },
    configureDeserializer: builder =>
    {
        builder.IgnoreUnmatchedProperties();
    }
);
```

### File Operations

```csharp
// Save to file
await jsonSerializer.SerializeToFileAsync("data.json", document);
await yamlSerializer.SerializeToFileAsync("data.yaml", document);

// Load from file
var doc1 = await jsonSerializer.DeserializeFromFileAsync<TextDocument>("data.json");
var doc2 = await yamlSerializer.DeserializeFromFileAsync<TextDocument>("data.yaml");
```

## Development

### Prerequisites
- .NET SDK 8.0 or later

### Building
```bash
git clone https://github.com/crp/fuzzy-potato.git
cd fuzzy-potato
dotnet restore
dotnet build
```

### Running Tests
```bash
dotnet test
```

### Creating Package
```bash
dotnet pack --configuration Release
```

## Project Structure

```
fuzzy-potato/
├── src/
│   ├── FuzzyPotato.Core/          # Core library
│   │   ├── Models/                 # Base and example models
│   │   └── Serialization/          # JSON/YAML serializers
│   └── FuzzyPotato.Core.Tests/    # Unit tests
├── .github/workflows/              # CI/CD pipelines
├── Directory.Build.props           # Shared build configuration
├── Directory.Packages.props        # Central package management
├── global.json                     # SDK version
└── version.json                    # Nerdbank.GitVersioning

```

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

MIT

## Acknowledgments

- Built on [System.Text.Json](https://learn.microsoft.com/en-us/dotnet/api/system.text.json) for JSON serialization
- Uses [YamlDotNet](https://github.com/aaubry/YamlDotNet) for YAML serialization
- Template based on [csharp-template](https://github.com/smartpcr/csharp-template)
