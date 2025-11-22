// -----------------------------------------------------------------------
// <copyright file="NodeSerializationYamlTests.cs" company="FuzzyPotato">
//     Copyright (c) FuzzyPotato. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Tests.Serialization
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using FuzzyPotato.Core.Tests.Examples.Nodes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.NamingConventions;

    /// <summary>
    /// Comprehensive YAML serialization tests for all workflow node types.
    /// Tests polymorphic serialization/deserialization with type discriminators.
    /// </summary>
    [TestClass]
    public class NodeSerializationYamlTests
    {
        private ISerializer _serializer = null!;
        private IDeserializer _deserializer = null!;

        /// <summary>
        /// Initializes the test class and registers node types.
        /// </summary>
        /// <param name="context">Test context.</param>
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            // Note: Type registration is optional - types are auto-discovered via assembly scanning
            // We register here for performance (faster first access)
        }

        /// <summary>
        /// Initializes each test with a fresh serializer instance.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            var serializerBuilder = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance);
            foreach (var converter in ConverterRegistry.YamlConverters)
            {
                serializerBuilder = serializerBuilder.WithTypeConverter(converter);
            }
            this._serializer = serializerBuilder.Build();

            var deserializerBuilder = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance);
            foreach (var converter in ConverterRegistry.YamlConverters)
            {
                deserializerBuilder = deserializerBuilder.WithTypeConverter(converter);
            }
            this._deserializer = deserializerBuilder.Build();
        }

        #region Script Execution Nodes

        [TestMethod]
        public void Serialize_CSharpScriptNode_ContainsTypeDiscriminator()
        {
            // Arrange
            var node = new CSharpScriptNode
            {
                NodeId = "csharp-script-1",
                NodeName = "Process Data",
                Description = "Processes input data using C# script",
                ScriptPath = "/scripts/process-data.csx",
                References = new List<string> { "System.Linq", "Newtonsoft.Json" },
                Imports = new List<string> { "System", "System.Linq" },
                TimeoutMs = 60000,
                PositionX = 100,
                PositionY = 200,
                Enabled = true,
            };

            // Act
            var yaml = this._serializer.Serialize(node);

            // Assert
            yaml.Should().Contain("$type: csharp-script-node");
            yaml.Should().Contain("nodeId: csharp-script-1");
            yaml.Should().Contain("scriptPath: /scripts/process-data.csx");
            yaml.Should().Contain("timeoutMs: 60000");
        }

        [TestMethod]
        public void Deserialize_CSharpScriptNode_RestoresAllProperties()
        {
            // Arrange
            var yaml = @"
$type: csharp-script-node
nodeId: csharp-script-1
nodeName: Process Data
description: Processes input data
scriptPath: /scripts/process.csx
references:
  - System.Linq
imports:
  - System
timeoutMs: 60000
positionX: 100
positionY: 200
enabled: true
";

            // Act
            var node = this._deserializer.Deserialize<NodeDefinition>(yaml);

            // Assert
            node.Should().BeOfType<CSharpScriptNode>();
            var csNode = (CSharpScriptNode)node;
            csNode.NodeId.Should().Be("csharp-script-1");
            csNode.ScriptPath.Should().Be("/scripts/process.csx");
            csNode.TimeoutMs.Should().Be(60000);
            csNode.References.Should().Contain("System.Linq");
        }

        [TestMethod]
        public void RoundTrip_CSharpTaskNode_PreservesAllData()
        {
            // Arrange
            var original = new CSharpTaskNode
            {
                NodeId = "csharp-task-1",
                NodeName = "Inline Task",
                ScriptContent = "return new Dictionary<string, object> { [\"result\"] = 42 };",
                References = new List<string> { "System.Collections.Generic" },
                Imports = new List<string> { "System.Collections.Generic" },
                TimeoutMs = 30000,
            };

            // Act
            var yaml = this._serializer.Serialize(original);
            var deserialized = this._deserializer.Deserialize<NodeDefinition>(yaml);

            // Assert
            deserialized.Should().BeOfType<CSharpTaskNode>();
            var taskNode = (CSharpTaskNode)deserialized;
            taskNode.NodeId.Should().Be(original.NodeId);
            taskNode.ScriptContent.Should().Be(original.ScriptContent);
            taskNode.TimeoutMs.Should().Be(original.TimeoutMs);
        }

        [TestMethod]
        public void RoundTrip_PowerShellScriptNode_PreservesComplexProperties()
        {
            // Arrange
            var original = new PowerShellScriptNode
            {
                NodeId = "ps-script-1",
                NodeName = "PowerShell Task",
                ScriptPath = "/scripts/process.ps1",
                RequiredModules = new List<string> { "Az.Accounts", "Az.Storage" },
                ModulePaths = new Dictionary<string, string>
                {
                    ["CustomModule"] = "/modules/custom",
                },
                ExecutionPolicy = "Unrestricted",
                Parameters = new Dictionary<string, object>
                {
                    ["ResourceGroup"] = "my-rg",
                    ["Timeout"] = 300,
                },
                CaptureVerbose = true,
                TimeoutMs = 300000,
            };

            // Act
            var yaml = this._serializer.Serialize(original);
            var deserialized = this._deserializer.Deserialize<NodeDefinition>(yaml);

            // Assert
            deserialized.Should().BeOfType<PowerShellScriptNode>();
            var psNode = (PowerShellScriptNode)deserialized;
            psNode.ScriptPath.Should().Be(original.ScriptPath);
            psNode.RequiredModules.Should().BeEquivalentTo(original.RequiredModules);
            psNode.ModulePaths.Should().BeEquivalentTo(original.ModulePaths);
            psNode.Parameters.Should().ContainKey("ResourceGroup");
        }

        [TestMethod]
        public void RoundTrip_PowerShellTaskNode_PreservesInlineScript()
        {
            // Arrange
            var original = new PowerShellTaskNode
            {
                NodeId = "ps-task-1",
                ScriptContent = "$result = Get-Date; Set-Output -Key 'timestamp' -Value $result",
                CaptureVerbose = false,
            };

            // Act
            var yaml = this._serializer.Serialize(original);
            var deserialized = this._deserializer.Deserialize<NodeDefinition>(yaml);

            // Assert
            deserialized.Should().BeOfType<PowerShellTaskNode>();
            var psTask = (PowerShellTaskNode)deserialized;
            psTask.ScriptContent.Should().Be(original.ScriptContent);
        }

        #endregion

        #region Control Flow Nodes

        [TestMethod]
        public void RoundTrip_IfElseNode_PreservesCondition()
        {
            // Arrange
            var original = new IfElseNode
            {
                NodeId = "if-1",
                NodeName = "Check Status",
                Condition = "status == \"success\"",
                TrueNodeId = "node-success",
                FalseNodeId = "node-failure",
            };

            // Act
            var yaml = this._serializer.Serialize(original);
            var deserialized = this._deserializer.Deserialize<NodeDefinition>(yaml);

            // Assert
            deserialized.Should().BeOfType<IfElseNode>();
            var ifNode = (IfElseNode)deserialized;
            ifNode.Condition.Should().Be(original.Condition);
            ifNode.TrueNodeId.Should().Be("node-success");
            ifNode.FalseNodeId.Should().Be("node-failure");
        }

        [TestMethod]
        public void RoundTrip_ForEachNode_PreservesIterationSettings()
        {
            // Arrange
            var original = new ForEachNode
            {
                NodeId = "foreach-1",
                CollectionExpression = "items.Where(x => x.IsActive)",
                ItemVariableName = "currentItem",
                ContinueOnError = true,
                MaxDegreeOfParallelism = 4,
            };

            // Act
            var yaml = this._serializer.Serialize(original);
            var deserialized = this._deserializer.Deserialize<NodeDefinition>(yaml);

            // Assert
            deserialized.Should().BeOfType<ForEachNode>();
            var forEachNode = (ForEachNode)deserialized;
            forEachNode.CollectionExpression.Should().Be(original.CollectionExpression);
            forEachNode.ItemVariableName.Should().Be("currentItem");
            forEachNode.MaxDegreeOfParallelism.Should().Be(4);
        }

        [TestMethod]
        public void RoundTrip_WhileNode_PreservesLoopConfiguration()
        {
            // Arrange
            var original = new WhileNode
            {
                NodeId = "while-1",
                Condition = "counter < 100",
                MaxIterations = 500,
                BreakOnError = false,
            };

            // Act
            var yaml = this._serializer.Serialize(original);
            var deserialized = this._deserializer.Deserialize<NodeDefinition>(yaml);

            // Assert
            deserialized.Should().BeOfType<WhileNode>();
            var whileNode = (WhileNode)deserialized;
            whileNode.Condition.Should().Be(original.Condition);
            whileNode.MaxIterations.Should().Be(500);
            whileNode.BreakOnError.Should().BeFalse();
        }

        [TestMethod]
        public void RoundTrip_SwitchNode_PreservesCaseMappings()
        {
            // Arrange
            var original = new SwitchNode
            {
                NodeId = "switch-1",
                Expression = "statusCode.ToString()",
                Cases = new Dictionary<string, string>
                {
                    ["200"] = "SuccessPort",
                    ["404"] = "NotFoundPort",
                    ["500"] = "ErrorPort",
                },
                DefaultPort = "UnknownPort",
                CaseSensitive = false,
            };

            // Act
            var yaml = this._serializer.Serialize(original);
            var deserialized = this._deserializer.Deserialize<NodeDefinition>(yaml);

            // Assert
            deserialized.Should().BeOfType<SwitchNode>();
            var switchNode = (SwitchNode)deserialized;
            switchNode.Cases.Should().HaveCount(3);
            switchNode.Cases["200"].Should().Be("SuccessPort");
            switchNode.DefaultPort.Should().Be("UnknownPort");
            switchNode.CaseSensitive.Should().BeFalse();
        }

        [TestMethod]
        public void RoundTrip_TimerNode_PreservesScheduleSettings()
        {
            // Arrange
            var original = new TimerNode
            {
                NodeId = "timer-1",
                NodeName = "Daily Backup",
                Schedule = "0 2 * * *",
                TriggerOnStart = true,
                TimeZone = "America/New_York",
                IsEnabled = true,
            };

            // Act
            var yaml = this._serializer.Serialize(original);
            var deserialized = this._deserializer.Deserialize<NodeDefinition>(yaml);

            // Assert
            deserialized.Should().BeOfType<TimerNode>();
            var timerNode = (TimerNode)deserialized;
            timerNode.Schedule.Should().Be("0 2 * * *");
            timerNode.TriggerOnStart.Should().BeTrue();
            timerNode.TimeZone.Should().Be("America/New_York");
        }

        #endregion

        #region Structural Nodes

        [TestMethod]
        public void RoundTrip_ContainerNode_PreservesNestedStructure()
        {
            // Arrange
            var original = new ContainerNode
            {
                NodeId = "container-1",
                NodeName = "Processing Container",
                ExecutionMode = "Parallel",
                ChildNodes = new List<NodeDefinition>
                {
                    new CSharpScriptNode
                    {
                        NodeId = "child-1",
                        ScriptPath = "/scripts/step1.csx",
                    },
                    new PowerShellTaskNode
                    {
                        NodeId = "child-2",
                        ScriptContent = "Write-Output 'Step 2'",
                    },
                },
                ChildConnections = new List<NodeConnection>
                {
                    new NodeConnection
                    {
                        SourceNodeId = "child-1",
                        TargetNodeId = "child-2",
                        SourcePort = "default",
                        TargetPort = "default",
                    },
                },
                FailFast = true,
                TimeoutMs = 600000,
                AggregateOutputs = true,
            };

            // Act
            var yaml = this._serializer.Serialize(original);
            var deserialized = this._deserializer.Deserialize<NodeDefinition>(yaml);

            // Assert
            deserialized.Should().BeOfType<ContainerNode>();
            var containerNode = (ContainerNode)deserialized;
            containerNode.ChildNodes.Should().HaveCount(2);
            containerNode.ChildNodes[0].Should().BeOfType<CSharpScriptNode>();
            containerNode.ChildNodes[1].Should().BeOfType<PowerShellTaskNode>();
            containerNode.ChildConnections.Should().HaveCount(1);
            containerNode.ExecutionMode.Should().Be("Parallel");
        }

        [TestMethod]
        public void RoundTrip_SubflowNode_PreservesWorkflowReference()
        {
            // Arrange
            var original = new SubflowNode
            {
                NodeId = "subflow-1",
                WorkflowFilePath = "/workflows/child-workflow.yaml",
                InputMappings = new Dictionary<string, string>
                {
                    ["parentVar1"] = "childVar1",
                    ["parentVar2"] = "childVar2",
                },
                OutputMappings = new Dictionary<string, string>
                {
                    ["childResult"] = "parentResult",
                },
                TimeoutMs = 300000,
                IsolateContext = true,
                PropagateCancellation = true,
            };

            // Act
            var yaml = this._serializer.Serialize(original);
            var deserialized = this._deserializer.Deserialize<NodeDefinition>(yaml);

            // Assert
            deserialized.Should().BeOfType<SubflowNode>();
            var subflowNode = (SubflowNode)deserialized;
            subflowNode.WorkflowFilePath.Should().Be("/workflows/child-workflow.yaml");
            subflowNode.InputMappings.Should().HaveCount(2);
            subflowNode.OutputMappings.Should().HaveCount(1);
            subflowNode.IsolateContext.Should().BeTrue();
        }

        [TestMethod]
        public void RoundTrip_SubflowNode_WithInlineWorkflow_PreservesNestedDefinition()
        {
            // Arrange
            var original = new SubflowNode
            {
                NodeId = "subflow-2",
                ChildWorkflowDefinition = new WorkflowDefinition
                {
                    Id = "child-wf-1",
                    Name = "Child Workflow",
                    Version = "1.0.0",
                    Nodes = new List<NodeDefinition>
                    {
                        new CSharpScriptNode { NodeId = "child-node-1" },
                    },
                    Connections = new List<NodeConnection>(),
                    StartNodeId = "child-node-1",
                    Variables = new Dictionary<string, object>
                    {
                        ["childVar"] = "value",
                    },
                },
            };

            // Act
            var yaml = this._serializer.Serialize(original);
            var deserialized = this._deserializer.Deserialize<NodeDefinition>(yaml);

            // Assert
            deserialized.Should().BeOfType<SubflowNode>();
            var subflowNode = (SubflowNode)deserialized;
            subflowNode.ChildWorkflowDefinition.Should().NotBeNull();
            subflowNode.ChildWorkflowDefinition!.Name.Should().Be("Child Workflow");
            subflowNode.ChildWorkflowDefinition.Nodes.Should().HaveCount(1);
            subflowNode.ChildWorkflowDefinition.Nodes[0].Should().BeOfType<CSharpScriptNode>();
        }

        #endregion

        #region Complex Scenarios

        [TestMethod]
        public void Serialize_WorkflowDefinition_WithMixedNodeTypes_PreservesPolymorphism()
        {
            // Arrange
            var workflow = new WorkflowDefinition
            {
                Id = "workflow-1",
                Name = "Complex Workflow",
                Description = "Tests polymorphic node serialization",
                Version = "2.0.0",
                Nodes = new List<NodeDefinition>
                {
                    new CSharpScriptNode { NodeId = "node-1", ScriptPath = "/script1.csx" },
                    new IfElseNode { NodeId = "node-2", Condition = "x > 10" },
                    new ForEachNode { NodeId = "node-3", CollectionExpression = "items" },
                    new PowerShellTaskNode { NodeId = "node-4", ScriptContent = "Write-Output 'Hello'" },
                    new TimerNode { NodeId = "node-5", Schedule = "0 * * * *" },
                },
                Connections = new List<NodeConnection>
                {
                    new NodeConnection { SourceNodeId = "node-1", TargetNodeId = "node-2" },
                    new NodeConnection { SourceNodeId = "node-2", TargetNodeId = "node-3", SourcePort = "TrueBranch" },
                },
                StartNodeId = "node-1",
                Variables = new Dictionary<string, object>
                {
                    ["counter"] = 0,
                    ["maxRetries"] = 3,
                },
            };

            // Act
            var yaml = this._serializer.Serialize(workflow);
            var deserialized = this._deserializer.Deserialize<WorkflowDefinition>(yaml);

            // Assert
            deserialized.Should().NotBeNull();
            deserialized.Nodes.Should().HaveCount(5);
            deserialized.Nodes[0].Should().BeOfType<CSharpScriptNode>();
            deserialized.Nodes[1].Should().BeOfType<IfElseNode>();
            deserialized.Nodes[2].Should().BeOfType<ForEachNode>();
            deserialized.Nodes[3].Should().BeOfType<PowerShellTaskNode>();
            deserialized.Nodes[4].Should().BeOfType<TimerNode>();
            deserialized.Connections.Should().HaveCount(2);
            deserialized.Variables.Should().ContainKey("counter");
        }

        [TestMethod]
        public void Serialize_DeeplyNestedContainerNode_PreservesHierarchy()
        {
            // Arrange
            var workflow = new WorkflowDefinition
            {
                Id = "nested-test",
                Nodes = new List<NodeDefinition>
                {
                    new ContainerNode
                    {
                        NodeId = "outer-container",
                        ChildNodes = new List<NodeDefinition>
                        {
                            new ContainerNode
                            {
                                NodeId = "inner-container",
                                ChildNodes = new List<NodeDefinition>
                                {
                                    new CSharpScriptNode { NodeId = "deepest-node" },
                                },
                            },
                        },
                    },
                },
            };

            // Act
            var yaml = this._serializer.Serialize(workflow);
            var deserialized = this._deserializer.Deserialize<WorkflowDefinition>(yaml);

            // Assert
            deserialized.Nodes.Should().HaveCount(1);
            var outerContainer = deserialized.Nodes[0] as ContainerNode;
            outerContainer.Should().NotBeNull();
            outerContainer!.ChildNodes.Should().HaveCount(1);
            var innerContainer = outerContainer.ChildNodes[0] as ContainerNode;
            innerContainer.Should().NotBeNull();
            innerContainer!.ChildNodes[0].Should().BeOfType<CSharpScriptNode>();
        }

        [TestMethod]
        public void Deserialize_YamlWorkflow_WithAllNodeTypes_SuccessfullyParsesAll()
        {
            // Arrange
            var yaml = @"
id: comprehensive-workflow
name: Comprehensive Node Test
version: 1.0.0
nodes:
  - $type: csharp-script-node
    nodeId: cs-script
    scriptPath: /scripts/test.csx
  - $type: csharp-task-node
    nodeId: cs-task
    scriptContent: return 42;
  - $type: powershell-script-node
    nodeId: ps-script
    scriptPath: /scripts/test.ps1
  - $type: powershell-task-node
    nodeId: ps-task
    scriptContent: Write-Output 'Test'
  - $type: if-else-node
    nodeId: if-else
    condition: x > 0
  - $type: foreach-node
    nodeId: foreach
    collectionExpression: items
  - $type: while-node
    nodeId: while
    condition: counter < 10
  - $type: switch-node
    nodeId: switch
    expression: status
    cases:
      ok: OkPort
      error: ErrorPort
  - $type: timer-node
    nodeId: timer
    schedule: '0 0 * * *'
  - $type: container-node
    nodeId: container
    childNodes: []
  - $type: subflow-node
    nodeId: subflow
    workflowFilePath: /workflows/child.yaml
connections: []
startNodeId: cs-script
variables: {}
";

            // Act
            var workflow = this._deserializer.Deserialize<WorkflowDefinition>(yaml);

            // Assert
            workflow.Should().NotBeNull();
            workflow.Nodes.Should().HaveCount(11);
            workflow.Nodes[0].Should().BeOfType<CSharpScriptNode>();
            workflow.Nodes[1].Should().BeOfType<CSharpTaskNode>();
            workflow.Nodes[2].Should().BeOfType<PowerShellScriptNode>();
            workflow.Nodes[3].Should().BeOfType<PowerShellTaskNode>();
            workflow.Nodes[4].Should().BeOfType<IfElseNode>();
            workflow.Nodes[5].Should().BeOfType<ForEachNode>();
            workflow.Nodes[6].Should().BeOfType<WhileNode>();
            workflow.Nodes[7].Should().BeOfType<SwitchNode>();
            workflow.Nodes[8].Should().BeOfType<TimerNode>();
            workflow.Nodes[9].Should().BeOfType<ContainerNode>();
            workflow.Nodes[10].Should().BeOfType<SubflowNode>();
        }

        #endregion
    }
}
