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
using FuzzyPotato.Core.Models;
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

## Example Implementations

The test project (`src/FuzzyPotato.Core.Tests/Examples/`) includes reference implementations showing how to use FuzzyPotato:

- **Document.cs**: Example document types (TextDocument, ImageDocument, VideoDocument)
- **Workflow.cs**: Workflow node definitions demonstrating complex polymorphic hierarchies
- **WorkflowRuntime.cs**: NodeFactory pattern and runtime execution examples

These examples demonstrate best practices and can be used as templates for your own implementations. See the test files for complete usage examples.

## Workflow Serialization Example

FuzzyPotato is perfect for complex scenarios like workflow engines where you need to serialize/deserialize polymorphic node hierarchies.

> **Full Implementation**: See `src/FuzzyPotato.Core.Tests/Examples/Workflow.cs` and `WorkflowRuntime.cs` for the complete working implementation.

### Workflow Node Types

```csharp
// Base class for all workflow nodes
public abstract class NodeDefinition : PolymorphicBase
{
    public string Description { get; set; }
    public double PositionX { get; set; }
    public double PositionY { get; set; }
    public bool Enabled { get; set; }
}

// Specific node types
public class CSharpNode : NodeDefinition
{
    public string Code { get; set; }
    public List<string> Usings { get; set; }
    public int TimeoutMs { get; set; }
}

public class PowerShellScriptNode : NodeDefinition
{
    public string Script { get; set; }
    public Dictionary<string, object> Parameters { get; set; }
}

public class IfElseNode : NodeDefinition
{
    public string Condition { get; set; }
    public string TrueNodeId { get; set; }
    public string FalseNodeId { get; set; }
}

public class WhileLoopNode : NodeDefinition
{
    public string Condition { get; set; }
    public int MaxIterations { get; set; }
    public string LoopBodyStartNodeId { get; set; }
}
```

### Workflow Definition

```csharp
public class WorkflowDefinition
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public List<NodeDefinition> Nodes { get; set; } // Polymorphic!
    public List<WorkflowConnection> Connections { get; set; }
    public string StartNodeId { get; set; }
    public Dictionary<string, object> Variables { get; set; }
}
```

### Creating and Serializing a Workflow

```csharp
// Register node types for YAML
TypeRegistry.Register<CSharpNode>("csharp-node");
TypeRegistry.Register<PowerShellScriptNode>("powershell-node");
TypeRegistry.Register<IfElseNode>("if-else-node");
TypeRegistry.Register<WhileLoopNode>("while-loop-node");
TypeRegistry.Register<HttpRequestNode>("http-request-node");
TypeRegistry.Register<DelayNode>("delay-node");

// Create a workflow with different node types
var workflow = new WorkflowDefinition
{
    Id = "data-pipeline",
    Name = "Data Processing Pipeline",
    StartNodeId = "fetch-data",
    Nodes = new List<NodeDefinition>
    {
        new HttpRequestNode
        {
            Id = "fetch-data",
            Name = "Fetch from API",
            Method = "GET",
            Url = "https://api.example.com/data",
            Headers = new Dictionary<string, string>
            {
                ["Authorization"] = "Bearer token"
            }
        },
        new IfElseNode
        {
            Id = "check-response",
            Name = "Validate Response",
            Condition = "response.StatusCode == 200",
            TrueNodeId = "process-data",
            FalseNodeId = "error-handler"
        },
        new CSharpNode
        {
            Id = "process-data",
            Name = "Transform Data",
            Code = "var result = data.Select(x => x.Transform());",
            Usings = new List<string> { "System", "System.Linq" }
        },
        new WhileLoopNode
        {
            Id = "batch-loop",
            Name = "Process Batches",
            Condition = "hasMoreBatches",
            MaxIterations = 100,
            LoopBodyStartNodeId = "process-batch"
        }
    },
    Connections = new List<WorkflowConnection>
    {
        new() { SourceNodeId = "fetch-data", TargetNodeId = "check-response" },
        new() { SourceNodeId = "check-response", TargetNodeId = "process-data", Label = "success" },
        new() { SourceNodeId = "check-response", TargetNodeId = "error-handler", Label = "error" }
    }
};

// Serialize to JSON (preserves all polymorphic types!)
var jsonSerializer = new FuzzyJsonSerializer();
var json = jsonSerializer.SerializeObject(workflow);
await jsonSerializer.SerializeObjectToFileAsync("workflow.json", workflow);

// Serialize to YAML
var yamlSerializer = new FuzzyYamlSerializer();
var yaml = yamlSerializer.SerializeObject(workflow);
await yamlSerializer.SerializeObjectToFileAsync("workflow.yaml", workflow);

// Deserialize - all node types are correctly restored!
var loadedWorkflow = await jsonSerializer.DeserializeObjectFromFileAsync<WorkflowDefinition>("workflow.json");

// Each node retains its specific type
var httpNode = loadedWorkflow.Nodes[0] as HttpRequestNode;
var ifElseNode = loadedWorkflow.Nodes[1] as IfElseNode;
var csharpNode = loadedWorkflow.Nodes[2] as CSharpNode;
```

### NodeFactory Pattern

```csharp
// Factory creates executable instances from definitions
public class NodeFactory
{
    public IExecutableNode CreateNode(NodeDefinition definition)
    {
        // Uses TypeRegistry to determine the correct runtime type
        return definition switch
        {
            CSharpNode n => new CSharpNodeExecutor(n),
            PowerShellScriptNode n => new PowerShellNodeExecutor(n),
            IfElseNode n => new IfElseNodeExecutor(n),
            // ... other types
            _ => throw new NotSupportedException()
        };
    }
}

// Execute nodes from deserialized workflow
var factory = new NodeFactory();
var executableNodes = loadedWorkflow.Nodes
    .Select(def => factory.CreateNode(def))
    .ToList();

// Run the workflow
var context = new WorkflowExecutionContext();
foreach (var node in executableNodes)
{
    var result = await node.ExecuteAsync(context);
    if (!result.Success) break;
}
```

### JSON Output Example

```json
{
  "id": "data-pipeline",
  "name": "Data Processing Pipeline",
  "nodes": [
    {
      "$type": "http-request-node",
      "id": "fetch-data",
      "name": "Fetch from API",
      "method": "GET",
      "url": "https://api.example.com/data",
      "headers": {
        "Authorization": "Bearer token"
      }
    },
    {
      "$type": "if-else-node",
      "id": "check-response",
      "name": "Validate Response",
      "condition": "response.StatusCode == 200",
      "trueNodeId": "process-data",
      "falseNodeId": "error-handler"
    },
    {
      "$type": "csharp-node",
      "id": "process-data",
      "name": "Transform Data",
      "code": "var result = data.Select(x => x.Transform());",
      "usings": ["System", "System.Linq"]
    }
  ]
}
```

### Key Benefits for Workflow Systems

1. **Type Safety**: All node types are correctly deserialized to their specific types
2. **Extensibility**: Easy to add new node types without changing serialization code
3. **Clean Separation**: Definition (data) separate from execution (runtime)
4. **Multiple Formats**: Save workflows as JSON or YAML
5. **Factory Pattern**: Clean instantiation of runtime executors from definitions

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

> **📚 Developer Documentation**: Comprehensive guides in [`.claude/agents/`](.claude/agents/index.md)
> - [Architecture](.claude/agents/architecture.md) - Project structure, build system, CI/CD
> - [Design Principles](.claude/agents/design.md) - SOLID principles, coding standards
> - [Patterns](.claude/agents/patterns.md) - Implementation patterns ⚠️ **Must read EditorConfig**
> - [Test Strategy](.claude/agents/test-strategy.md) - Testing approach, coverage targets
> - [Usage Guide](.claude/agents/usage-guide.md) - Development workflows, IDE setup
> - [Prompt History](.claude/prompts/) - Development timeline and decisions

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
│   │   ├── Models/                 # PolymorphicBase, TypeRegistry
│   │   └── Serialization/          # JSON/YAML serializers
│   └── FuzzyPotato.Core.Tests/    # Unit tests
│       ├── Examples/               # Example implementations (Document, Workflow)
│       └── Serialization/          # Serialization tests
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
