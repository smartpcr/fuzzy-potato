// -----------------------------------------------------------------------
// <copyright file="StructuralNodes.cs" company="FuzzyPotato">
//     Copyright (c) FuzzyPotato. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Tests.Examples.Nodes
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;
    using FuzzyPotato.Core.Tests.Examples.Nodes.Converters;

    /// <summary>
    /// Represents a connection between two nodes in a workflow.
    /// Defines routing from source node's output port to target node's input port.
    /// </summary>
    public class NodeConnection
    {
        /// <summary>
        /// Gets or sets the source node ID.
        /// </summary>
        public string SourceNodeId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the target node ID.
        /// </summary>
        public string TargetNodeId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the source port/output name.
        /// Control flow nodes use ports like "TrueBranch", "FalseBranch", "Default".
        /// </summary>
        public string SourcePort { get; set; } = "default";

        /// <summary>
        /// Gets or sets the target port/input name.
        /// </summary>
        public string TargetPort { get; set; } = "default";

        /// <summary>
        /// Gets or sets the label for this connection (for visualization).
        /// </summary>
        public string? Label { get; set; }
    }

    /// <summary>
    /// Node that groups related nodes into a logical unit with encapsulated execution flow.
    /// Supports parallel, sequential, or mixed execution modes.
    /// </summary>
    [JsonConverter(typeof(ContainerNodeJsonConverter))]
    public class ContainerNode : NodeDefinition
    {
        /// <inheritdoc/>
        public override string TypeName => "container-node";

        /// <summary>
        /// Gets or sets the nodes contained within this container.
        /// These nodes form a sub-graph with their own execution flow.
        /// </summary>
        public List<NodeDefinition> ChildNodes { get; set; } = new();

        /// <summary>
        /// Gets or sets the connections between child nodes.
        /// Defines the execution flow within the container.
        /// </summary>
        public List<NodeConnection> ChildConnections { get; set; } = new();

        /// <summary>
        /// Gets or sets the execution mode.
        /// Values: "Parallel" (all entry points start simultaneously),
        /// "Sequential" (entry points execute in order),
        /// "Mixed" (default - follows connection dependencies).
        /// </summary>
        public string ExecutionMode { get; set; } = "Mixed";

        /// <summary>
        /// Gets or sets a value indicating whether to stop on first child failure.
        /// If false, continues executing other children after one fails.
        /// </summary>
        public bool FailFast { get; set; } = true;

        /// <summary>
        /// Gets or sets the maximum time to wait for all children to complete (milliseconds).
        /// </summary>
        public int TimeoutMs { get; set; } = 600000;

        /// <summary>
        /// Gets or sets a value indicating whether to collect output from all children.
        /// If true, aggregates outputs in ChildResults dictionary.
        /// </summary>
        public bool AggregateOutputs { get; set; } = true;
    }

    /// <summary>
    /// Node that executes another workflow as a child/nested workflow with context isolation.
    /// Supports input/output variable mapping between parent and child workflows.
    /// </summary>
    [JsonConverter(typeof(SubflowNodeJsonConverter))]
    public class SubflowNode : NodeDefinition
    {
        /// <inheritdoc/>
        public override string TypeName => "subflow-node";

        /// <summary>
        /// Gets or sets the path to the workflow definition file to execute.
        /// Can be relative or absolute path.
        /// </summary>
        public string? WorkflowFilePath { get; set; }

        /// <summary>
        /// Gets or sets the inline workflow definition (alternative to WorkflowFilePath).
        /// If both are set, inline definition takes precedence.
        /// </summary>
        public WorkflowDefinition? ChildWorkflowDefinition { get; set; }

        /// <summary>
        /// Gets or sets the input variable mappings.
        /// Key: Parent workflow variable name.
        /// Value: Child workflow variable name.
        /// Copies values from parent to child before execution.
        /// </summary>
        public Dictionary<string, string> InputMappings { get; set; } = new();

        /// <summary>
        /// Gets or sets the output variable mappings.
        /// Key: Child workflow variable name.
        /// Value: Parent workflow variable name.
        /// Copies values from child to parent after execution.
        /// </summary>
        public Dictionary<string, string> OutputMappings { get; set; } = new();

        /// <summary>
        /// Gets or sets the timeout for child workflow execution (milliseconds).
        /// </summary>
        public int TimeoutMs { get; set; } = 600000;

        /// <summary>
        /// Gets or sets a value indicating whether child workflow runs in isolated context.
        /// If true, child cannot access parent variables except via InputMappings.
        /// </summary>
        public bool IsolateContext { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to propagate cancellation to child.
        /// </summary>
        public bool PropagateCancellation { get; set; } = true;
    }

    /// <summary>
    /// Represents a complete workflow definition with nodes and connections.
    /// Can be used standalone or as a child workflow in SubflowNode.
    /// </summary>
    public class WorkflowDefinition
    {
        /// <summary>
        /// Gets or sets the workflow ID.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the workflow name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the workflow description.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the workflow version.
        /// </summary>
        public string Version { get; set; } = "1.0.0";

        /// <summary>
        /// Gets or sets the nodes in the workflow.
        /// </summary>
        public List<NodeDefinition> Nodes { get; set; } = new();

        /// <summary>
        /// Gets or sets the connections between nodes.
        /// </summary>
        public List<NodeConnection> Connections { get; set; } = new();

        /// <summary>
        /// Gets or sets the ID of the start node (entry point).
        /// </summary>
        public string StartNodeId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets workflow-level variables and their initial values.
        /// </summary>
        public Dictionary<string, object> Variables { get; set; } = new();

        /// <summary>
        /// Gets or sets the creation timestamp.
        /// </summary>
        public System.DateTime CreatedAt { get; set; } = System.DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the last modified timestamp.
        /// </summary>
        public System.DateTime ModifiedAt { get; set; } = System.DateTime.UtcNow;
    }
}
