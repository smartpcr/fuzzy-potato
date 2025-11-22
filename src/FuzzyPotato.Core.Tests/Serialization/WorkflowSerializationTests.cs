// -----------------------------------------------------------------------
// <copyright file="WorkflowSerializationTests.cs" company="FuzzyPotato">
//     Copyright (c) FuzzyPotato. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Tests.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;
    using FluentAssertions;
    using FuzzyPotato.Core.Models;
    using FuzzyPotato.Core.Tests.Examples.Documents;
    using FuzzyPotato.Core.Tests.Examples.Nodes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.NamingConventions;

    /// <summary>
    /// Tests for workflow serialization and deserialization.
    /// </summary>
    [TestClass]
    public class WorkflowSerializationTests
    {
        private JsonSerializerOptions _jsonOptions = null!;
        private ISerializer _yamlSerializer = null!;
        private IDeserializer _yamlDeserializer = null!;

        /// <summary>
        /// Initializes the test class by registering all workflow node types.
        /// </summary>
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            // Types are automatically discovered via custom converters
            // No manual registration needed
        }

        /// <summary>
        /// Initializes serializers before each test.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this._jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            };
            foreach (var converter in ConverterRegistry.JsonConverters)
            {
                this._jsonOptions.Converters.Add(converter);
            }
            this._jsonOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));

            var serializerBuilder = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance);
            foreach (var converter in ConverterRegistry.YamlConverters)
            {
                serializerBuilder = serializerBuilder.WithTypeConverter(converter);
            }
            this._yamlSerializer = serializerBuilder.Build();

            var deserializerBuilder = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance);
            foreach (var converter in ConverterRegistry.YamlConverters)
            {
                deserializerBuilder = deserializerBuilder.WithTypeConverter(converter);
            }
            this._yamlDeserializer = deserializerBuilder.Build();
        }

        /// <summary>
        /// Tests JSON serialization and deserialization of a CSharpNode.
        /// </summary>
        [TestMethod]
        public void SerializeDeserialize_CSharpNode_Json_ReturnsEquivalentNode()
        {
            // Arrange
            var node = new CSharpNode
            {
                NodeId = "node-1",
                NodeName = "Calculate Sum",
                Description = "Calculates the sum of two numbers",
                PositionX = 100,
                PositionY = 200,
                Enabled = true,
                Code = "var result = a + b; return result;",
                Usings = new List<string> { "System", "System.Linq" },
                References = new List<string> { "System.Runtime" },
                TimeoutMs = 5000,
            };

            // Act
            var json = JsonSerializer.Serialize(node, this._jsonOptions);
            var deserialized = JsonSerializer.Deserialize<CSharpNode>(json, this._jsonOptions);

            // Assert
            deserialized.Should().NotBeNull();
            deserialized!.NodeId.Should().Be(node.NodeId);
            deserialized.NodeName.Should().Be(node.NodeName);
            deserialized.Description.Should().Be(node.Description);
            deserialized.Code.Should().Be(node.Code);
            deserialized.Usings.Should().BeEquivalentTo(node.Usings);
            deserialized.References.Should().BeEquivalentTo(node.References);
            deserialized.TimeoutMs.Should().Be(node.TimeoutMs);
        }

        /// <summary>
        /// Tests YAML serialization and deserialization of a CSharpNode.
        /// </summary>
        [TestMethod]
        public void SerializeDeserialize_CSharpNode_Yaml_ReturnsEquivalentNode()
        {
            // Arrange
            var node = new CSharpNode
            {
                NodeId = "node-1",
                NodeName = "Calculate Sum",
                Description = "Calculates the sum of two numbers",
                Code = "var result = a + b; return result;",
                Usings = new List<string> { "System", "System.Linq" },
                TimeoutMs = 5000,
            };

            // Act
            var yaml = this._yamlSerializer.Serialize(node);
            var deserialized = this._yamlDeserializer.Deserialize<CSharpNode>(yaml);

            // Assert
            deserialized.Should().NotBeNull();
            deserialized!.NodeId.Should().Be(node.NodeId);
            deserialized.NodeName.Should().Be(node.NodeName);
            deserialized.Code.Should().Be(node.Code);
            deserialized.Usings.Should().BeEquivalentTo(node.Usings);
        }

        /// <summary>
        /// Tests JSON serialization of polymorphic node collection.
        /// </summary>
        [TestMethod]
        public void SerializeDeserialize_PolymorphicNodes_Json_PreservesNodeTypes()
        {
            // Arrange
            var nodes = new List<NodeDefinition>
            {
                new CSharpNode
                {
                    NodeId = "node-1",
                    NodeName = "C# Node",
                    Code = "Console.WriteLine(\"Hello\");",
                },
                new PowerShellScriptNode
                {
                    NodeId = "node-2",
                    NodeName = "PowerShell Node",
                    ScriptPath = "Write-Host 'Hello'",
                    ExecutionPolicy = "RemoteSigned",
                },
                new HttpRequestNode
                {
                    NodeId = "node-3",
                    NodeName = "HTTP Request",
                    Method = "GET",
                    Url = "https://api.example.com/data",
                },
                new DelayNode
                {
                    NodeId = "node-4",
                    NodeName = "Wait",
                    DelayMs = 1000,
                },
            };

            // Act
            var json = JsonSerializer.Serialize(nodes, this._jsonOptions);
            var deserialized = JsonSerializer.Deserialize<List<NodeDefinition>>(json, this._jsonOptions);

            // Assert
            deserialized.Should().NotBeNull();
            var deserializedList = deserialized!.ToList();
            deserializedList.Should().HaveCount(4);
            deserializedList[0].Should().BeOfType<CSharpNode>();
            deserializedList[1].Should().BeOfType<PowerShellScriptNode>();
            deserializedList[2].Should().BeOfType<HttpRequestNode>();
            deserializedList[3].Should().BeOfType<DelayNode>();
        }

        /// <summary>
        /// Tests YAML serialization of polymorphic node collection.
        /// </summary>
        [TestMethod]
        public void SerializeDeserialize_PolymorphicNodes_Yaml_PreservesNodeTypes()
        {
            // Arrange
            var nodes = new List<NodeDefinition>
            {
                new CSharpNode
                {
                    NodeId = "node-1",
                    NodeName = "C# Node",
                    Code = "Console.WriteLine(\"Hello\");",
                },
                new IfElseNode
                {
                    NodeId = "node-2",
                    NodeName = "Condition",
                    Condition = "x > 0",
                    TrueNodeId = "node-3",
                    FalseNodeId = "node-4",
                },
            };

            // Act
            var yaml = this._yamlSerializer.Serialize(nodes);
            var deserialized = this._yamlDeserializer.Deserialize<List<NodeDefinition>>(yaml);

            // Assert
            deserialized.Should().NotBeNull();
            var deserializedList = deserialized!.ToList();
            deserializedList.Should().HaveCount(2);
            deserializedList[0].Should().BeOfType<CSharpNode>();
            deserializedList[1].Should().BeOfType<IfElseNode>();

            var ifElseNode = deserializedList[1] as IfElseNode;
            ifElseNode!.Condition.Should().Be("x > 0");
            ifElseNode.TrueNodeId.Should().Be("node-3");
            ifElseNode.FalseNodeId.Should().Be("node-4");
        }

        /// <summary>
        /// Tests JSON serialization of a complete workflow definition.
        /// </summary>
        [TestMethod]
        public void SerializeDeserialize_CompleteWorkflow_Json_PreservesAllData()
        {
            // Arrange
            var workflow = this.CreateSampleWorkflow();

            // Act
            var json = JsonSerializer.Serialize(workflow, this._jsonOptions);
            var deserialized = JsonSerializer.Deserialize<WorkflowDefinition>(json, this._jsonOptions);

            // Assert
            deserialized.Should().NotBeNull();
            deserialized!.Id.Should().Be(workflow.Id);
            deserialized.Name.Should().Be(workflow.Name);
            deserialized.Description.Should().Be(workflow.Description);
            deserialized.Version.Should().Be(workflow.Version);
            deserialized.StartNodeId.Should().Be(workflow.StartNodeId);
            deserialized.Nodes.Should().HaveCount(workflow.Nodes.Count);
            deserialized.Connections.Should().HaveCount(workflow.Connections.Count);
            deserialized.Variables.Should().HaveCount(workflow.Variables.Count);
            deserialized.Variables.Should().ContainKey("apiKey");
            deserialized.Variables.Should().ContainKey("maxRetries");
            deserialized.Variables.Should().ContainKey("timeout");
        }

        /// <summary>
        /// Tests YAML serialization of a complete workflow definition.
        /// </summary>
        [TestMethod]
        public void SerializeDeserialize_CompleteWorkflow_Yaml_PreservesAllData()
        {
            // Arrange
            var workflow = this.CreateSampleWorkflow();

            // Act
            var yaml = this._yamlSerializer.Serialize(workflow);
            var deserialized = this._yamlDeserializer.Deserialize<WorkflowDefinition>(yaml);

            // Assert
            deserialized.Should().NotBeNull();
            deserialized!.Id.Should().Be(workflow.Id);
            deserialized.Name.Should().Be(workflow.Name);
            deserialized.Nodes.Should().HaveCount(workflow.Nodes.Count);
            deserialized.Connections.Should().HaveCount(workflow.Connections.Count);
        }

        /// <summary>
        /// Tests file-based serialization and deserialization.
        /// </summary>
        [TestMethod]
        public async Task SerializeDeserialize_WorkflowToFile_Json_SuccessfullyReadsBack()
        {
            // Arrange
            var workflow = this.CreateSampleWorkflow();
            var tempFile = System.IO.Path.GetTempFileName();

            try
            {
                // Act
                var json = JsonSerializer.Serialize(workflow, this._jsonOptions);
                await System.IO.File.WriteAllTextAsync(tempFile, json);
                var fileContent = await System.IO.File.ReadAllTextAsync(tempFile);
                var deserialized = JsonSerializer.Deserialize<WorkflowDefinition>(fileContent, this._jsonOptions);

                // Assert
                deserialized.Should().NotBeNull();
                deserialized!.Id.Should().Be(workflow.Id);
                deserialized.Nodes.Should().HaveCount(workflow.Nodes.Count);
            }
            finally
            {
                if (System.IO.File.Exists(tempFile))
                {
                    System.IO.File.Delete(tempFile);
                }
            }
        }

        /// <summary>
        /// Tests NodeFactory instantiation from workflow definition.
        /// </summary>
        [TestMethod]
        public void NodeFactory_CreateWorkflowNodes_CreatesAllExecutableNodes()
        {
            // Arrange
            var workflow = this.CreateSampleWorkflow();
            var factory = new NodeFactory();

            // Act
            var executableNodes = factory.CreateWorkflowNodes(workflow);

            // Assert
            executableNodes.Should().HaveCount(workflow.Nodes.Count);
            executableNodes.Should().ContainKey("start-node");
            executableNodes.Should().ContainKey("http-node");
            executableNodes.Should().ContainKey("condition-node");

            var csharpExecutor = executableNodes["start-node"];
            csharpExecutor.Should().BeAssignableTo<IExecutableNode>();
            csharpExecutor.NodeId.Should().Be("start-node");
        }

        /// <summary>
        /// Tests node execution through the factory pattern.
        /// </summary>
        [TestMethod]
        public async Task NodeFactory_ExecuteNode_ReturnsSuccessResult()
        {
            // Arrange
            var node = new CSharpNode
            {
                NodeId = "test-node",
                NodeName = "Test",
                Code = "return 42;",
            };

            var factory = new NodeFactory();
            var executable = factory.CreateNode(node);
            var context = new WorkflowExecutionContext();

            // Act
            var result = await executable.ExecuteAsync(context);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            context.ExecutionTrace.Should().NotBeEmpty();
        }

        /// <summary>
        /// Tests complex workflow with loops and conditions.
        /// </summary>
        [TestMethod]
        public void SerializeDeserialize_ComplexWorkflowWithLoopsAndConditions_PreservesStructure()
        {
            // Arrange
            var workflow = new WorkflowDefinition
            {
                Id = "complex-workflow",
                Name = "Complex Workflow",
                Description = "Workflow with loops and conditions",
                Version = "1.0.0",
                StartNodeId = "loop-node",
                Nodes = new List<NodeDefinition>
                {
                    new WhileLoopNode
                    {
                        NodeId = "loop-node",
                        NodeName = "Main Loop",
                        Condition = "counter < 10",
                        MaxIterations = 100,
                        LoopBodyStartNodeId = "condition-node",
                    },
                    new IfElseNode
                    {
                        NodeId = "condition-node",
                        NodeName = "Check Value",
                        Condition = "value > 5",
                        TrueNodeId = "true-branch",
                        FalseNodeId = "false-branch",
                    },
                    new CSharpNode
                    {
                        NodeId = "true-branch",
                        NodeName = "Handle True",
                        Code = "Console.WriteLine(\"True\");",
                    },
                    new CSharpNode
                    {
                        NodeId = "false-branch",
                        NodeName = "Handle False",
                        Code = "Console.WriteLine(\"False\");",
                    },
                },
                Connections = new List<NodeConnection>
                {
                    new() { SourceNodeId = "loop-node", TargetNodeId = "condition-node" },
                    new() { SourceNodeId = "condition-node", TargetNodeId = "true-branch", Label = "true" },
                    new() { SourceNodeId = "condition-node", TargetNodeId = "false-branch", Label = "false" },
                },
            };

            // Act
            var json = JsonSerializer.Serialize(workflow, this._jsonOptions);
            var deserialized = JsonSerializer.Deserialize<WorkflowDefinition>(json, this._jsonOptions);

            // Assert
            deserialized.Should().NotBeNull();
            deserialized!.Nodes.Should().HaveCount(4);

            var loopNode = deserialized.Nodes.OfType<WhileLoopNode>().First();
            loopNode.Condition.Should().Be("counter < 10");
            loopNode.LoopBodyStartNodeId.Should().Be("condition-node");

            var ifElseNode = deserialized.Nodes.OfType<IfElseNode>().First();
            ifElseNode.TrueNodeId.Should().Be("true-branch");
            ifElseNode.FalseNodeId.Should().Be("false-branch");
        }

        private WorkflowDefinition CreateSampleWorkflow()
        {
            return new WorkflowDefinition
            {
                Id = "workflow-1",
                Name = "Sample Workflow",
                Description = "A sample workflow for testing",
                Version = "1.0.0",
                StartNodeId = "start-node",
                Nodes = new List<NodeDefinition>
                {
                    new CSharpNode
                    {
                        NodeId = "start-node",
                        NodeName = "Initialize",
                        Description = "Initialize workflow variables",
                        PositionX = 100,
                        PositionY = 100,
                        Code = "var x = 0;",
                        Usings = new List<string> { "System" },
                    },
                    new HttpRequestNode
                    {
                        NodeId = "http-node",
                        NodeName = "Fetch Data",
                        Description = "Fetch data from API",
                        PositionX = 300,
                        PositionY = 100,
                        Method = "GET",
                        Url = "https://api.example.com/data",
                        Headers = new Dictionary<string, string>
                        {
                            ["Authorization"] = "Bearer token",
                        },
                    },
                    new IfElseNode
                    {
                        NodeId = "condition-node",
                        NodeName = "Check Response",
                        Description = "Check if response is successful",
                        PositionX = 500,
                        PositionY = 100,
                        Condition = "response.StatusCode == 200",
                        TrueNodeId = "success-node",
                        FalseNodeId = "error-node",
                    },
                    new DelayNode
                    {
                        NodeId = "success-node",
                        NodeName = "Success Delay",
                        Description = "Wait before continuing",
                        PositionX = 700,
                        PositionY = 50,
                        DelayMs = 1000,
                    },
                    new PowerShellScriptNode
                    {
                        NodeId = "error-node",
                        NodeName = "Error Handler",
                        Description = "Handle error",
                        PositionX = 700,
                        PositionY = 150,
                        ScriptPath = "Write-Error 'Request failed'",
                        Parameters = new Dictionary<string, object>
                        {
                            ["ErrorMessage"] = "Request failed",
                        },
                    },
                },
                Connections = new List<NodeConnection>
                {
                    new()
                    {
                        SourceNodeId = "start-node",
                        TargetNodeId = "http-node",
                        Label = "next",
                    },
                    new()
                    {
                        SourceNodeId = "http-node",
                        TargetNodeId = "condition-node",
                        Label = "response",
                    },
                    new()
                    {
                        SourceNodeId = "condition-node",
                        TargetNodeId = "success-node",
                        Label = "true",
                    },
                    new()
                    {
                        SourceNodeId = "condition-node",
                        TargetNodeId = "error-node",
                        Label = "false",
                    },
                },
                Variables = new Dictionary<string, object>
                {
                    ["apiKey"] = "secret-key",
                    ["maxRetries"] = 3,
                    ["timeout"] = 30000,
                },
            };
        }
    }
}
