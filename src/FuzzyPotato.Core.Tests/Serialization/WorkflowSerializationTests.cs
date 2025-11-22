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
    using System.Threading.Tasks;
    using FluentAssertions;
    using FuzzyPotato.Core.Models;
    using FuzzyPotato.Core.Serialization;
    using FuzzyPotato.Core.Tests.Examples;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for workflow serialization and deserialization.
    /// </summary>
    [TestClass]
    public class WorkflowSerializationTests
    {
        private FuzzyJsonSerializer _jsonSerializer = null!;
        private FuzzyYamlSerializer _yamlSerializer = null!;

        /// <summary>
        /// Initializes the test class by registering all workflow node types.
        /// </summary>
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            // Register all workflow node types for polymorphic serialization
            TypeRegistry.Register<CSharpNode>();
            TypeRegistry.Register<PowerShellScriptNode>();
            TypeRegistry.Register<WhileLoopNode>();
            TypeRegistry.Register<IfElseNode>();
            TypeRegistry.Register<HttpRequestNode>();
            TypeRegistry.Register<DelayNode>();
        }

        /// <summary>
        /// Initializes serializers before each test.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this._jsonSerializer = new FuzzyJsonSerializer();
            this._yamlSerializer = new FuzzyYamlSerializer();
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
                Id = "node-1",
                Name = "Calculate Sum",
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
            var json = this._jsonSerializer.Serialize(node);
            var deserialized = this._jsonSerializer.Deserialize<CSharpNode>(json);

            // Assert
            deserialized.Should().NotBeNull();
            deserialized!.Id.Should().Be(node.Id);
            deserialized.Name.Should().Be(node.Name);
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
                Id = "node-1",
                Name = "Calculate Sum",
                Description = "Calculates the sum of two numbers",
                Code = "var result = a + b; return result;",
                Usings = new List<string> { "System", "System.Linq" },
                TimeoutMs = 5000,
            };

            // Act
            var yaml = this._yamlSerializer.Serialize(node);
            var deserialized = this._yamlSerializer.Deserialize<CSharpNode>(yaml);

            // Assert
            deserialized.Should().NotBeNull();
            deserialized!.Id.Should().Be(node.Id);
            deserialized.Name.Should().Be(node.Name);
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
                    Id = "node-1",
                    Name = "C# Node",
                    Code = "Console.WriteLine(\"Hello\");",
                },
                new PowerShellScriptNode
                {
                    Id = "node-2",
                    Name = "PowerShell Node",
                    Script = "Write-Host 'Hello'",
                    ExecutionPolicy = "RemoteSigned",
                },
                new HttpRequestNode
                {
                    Id = "node-3",
                    Name = "HTTP Request",
                    Method = "GET",
                    Url = "https://api.example.com/data",
                },
                new DelayNode
                {
                    Id = "node-4",
                    Name = "Wait",
                    DelayMs = 1000,
                },
            };

            // Act
            var json = this._jsonSerializer.SerializeCollection(nodes);
            var deserialized = this._jsonSerializer.DeserializeCollection<NodeDefinition>(json);

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
                    Id = "node-1",
                    Name = "C# Node",
                    Code = "Console.WriteLine(\"Hello\");",
                },
                new IfElseNode
                {
                    Id = "node-2",
                    Name = "Condition",
                    Condition = "x > 0",
                    TrueNodeId = "node-3",
                    FalseNodeId = "node-4",
                },
            };

            // Act
            var yaml = this._yamlSerializer.SerializeCollection(nodes);
            var deserialized = this._yamlSerializer.DeserializeCollection<NodeDefinition>(yaml);

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
            var json = this._jsonSerializer.Serialize(workflow);
            var deserialized = this._jsonSerializer.Deserialize<WorkflowDefinition>(json);

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
            var deserialized = this._yamlSerializer.Deserialize<WorkflowDefinition>(yaml);

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
                await this._jsonSerializer.SerializeToFileAsync(tempFile, workflow);
                var deserialized = await this._jsonSerializer.DeserializeFromFileAsync<WorkflowDefinition>(tempFile);

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
                Id = "test-node",
                Name = "Test",
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
                        Id = "loop-node",
                        Name = "Main Loop",
                        Condition = "counter < 10",
                        MaxIterations = 100,
                        LoopBodyStartNodeId = "condition-node",
                    },
                    new IfElseNode
                    {
                        Id = "condition-node",
                        Name = "Check Value",
                        Condition = "value > 5",
                        TrueNodeId = "true-branch",
                        FalseNodeId = "false-branch",
                    },
                    new CSharpNode
                    {
                        Id = "true-branch",
                        Name = "Handle True",
                        Code = "Console.WriteLine(\"True\");",
                    },
                    new CSharpNode
                    {
                        Id = "false-branch",
                        Name = "Handle False",
                        Code = "Console.WriteLine(\"False\");",
                    },
                },
                Connections = new List<WorkflowConnection>
                {
                    new() { SourceNodeId = "loop-node", TargetNodeId = "condition-node" },
                    new() { SourceNodeId = "condition-node", TargetNodeId = "true-branch", Label = "true" },
                    new() { SourceNodeId = "condition-node", TargetNodeId = "false-branch", Label = "false" },
                },
            };

            // Act
            var json = this._jsonSerializer.Serialize(workflow);
            var deserialized = this._jsonSerializer.Deserialize<WorkflowDefinition>(json);

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
                        Id = "start-node",
                        Name = "Initialize",
                        Description = "Initialize workflow variables",
                        PositionX = 100,
                        PositionY = 100,
                        Code = "var x = 0;",
                        Usings = new List<string> { "System" },
                    },
                    new HttpRequestNode
                    {
                        Id = "http-node",
                        Name = "Fetch Data",
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
                        Id = "condition-node",
                        Name = "Check Response",
                        Description = "Check if response is successful",
                        PositionX = 500,
                        PositionY = 100,
                        Condition = "response.StatusCode == 200",
                        TrueNodeId = "success-node",
                        FalseNodeId = "error-node",
                    },
                    new DelayNode
                    {
                        Id = "success-node",
                        Name = "Success Delay",
                        Description = "Wait before continuing",
                        PositionX = 700,
                        PositionY = 50,
                        DelayMs = 1000,
                    },
                    new PowerShellScriptNode
                    {
                        Id = "error-node",
                        Name = "Error Handler",
                        Description = "Handle error",
                        PositionX = 700,
                        PositionY = 150,
                        Script = "Write-Error 'Request failed'",
                        Parameters = new Dictionary<string, object>
                        {
                            ["ErrorMessage"] = "Request failed",
                        },
                    },
                },
                Connections = new List<WorkflowConnection>
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
