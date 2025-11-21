// -----------------------------------------------------------------------
// <copyright file="Workflow.cs" company="FuzzyPotato">
//     Copyright (c) FuzzyPotato. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Tests.Examples
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;
    using FuzzyPotato.Core.Models;

    /// <summary>
    /// Base class for all node definitions in a workflow.
    /// Each node type has specific properties for configuration.
    /// </summary>
    [JsonDerivedType(typeof(NodeDefinition), typeDiscriminator: "base-node")]
    public abstract class NodeDefinition : PolymorphicBase
    {
        /// <summary>
        /// Gets or sets the description of what this node does.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the X position in the workflow designer.
        /// </summary>
        public double PositionX { get; set; }

        /// <summary>
        /// Gets or sets the Y position in the workflow designer.
        /// </summary>
        public double PositionY { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this node is enabled.
        /// </summary>
        public bool Enabled { get; set; } = true;
    }

    /// <summary>
    /// Node that executes C# code.
    /// </summary>
    [JsonDerivedType(typeof(CSharpNode), typeDiscriminator: "csharp-node")]
    public class CSharpNode : NodeDefinition
    {
        /// <summary>
        /// Gets or sets the C# code to execute.
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the list of using directives.
        /// </summary>
        public List<string> Usings { get; set; } = new();

        /// <summary>
        /// Gets or sets the list of assembly references.
        /// </summary>
        public List<string> References { get; set; } = new();

        /// <summary>
        /// Gets or sets the timeout in milliseconds.
        /// </summary>
        public int TimeoutMs { get; set; } = 30000;
    }

    /// <summary>
    /// Node that executes PowerShell scripts.
    /// </summary>
    [JsonDerivedType(typeof(PowerShellScriptNode), typeDiscriminator: "powershell-node")]
    public class PowerShellScriptNode : NodeDefinition
    {
        /// <summary>
        /// Gets or sets the PowerShell script to execute.
        /// </summary>
        public string Script { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the execution policy.
        /// </summary>
        public string ExecutionPolicy { get; set; } = "RemoteSigned";

        /// <summary>
        /// Gets or sets the parameters to pass to the script.
        /// </summary>
        public Dictionary<string, object> Parameters { get; set; } = new();

        /// <summary>
        /// Gets or sets a value indicating whether to run in elevated mode.
        /// </summary>
        public bool RunAsAdmin { get; set; }
    }

    /// <summary>
    /// Node that represents a while loop in the workflow.
    /// </summary>
    [JsonDerivedType(typeof(WhileLoopNode), typeDiscriminator: "while-loop-node")]
    public class WhileLoopNode : NodeDefinition
    {
        /// <summary>
        /// Gets or sets the condition expression to evaluate.
        /// </summary>
        public string Condition { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the maximum number of iterations.
        /// </summary>
        public int MaxIterations { get; set; } = 1000;

        /// <summary>
        /// Gets or sets the ID of the first node in the loop body.
        /// </summary>
        public string LoopBodyStartNodeId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether to break on error.
        /// </summary>
        public bool BreakOnError { get; set; } = true;
    }

    /// <summary>
    /// Node that represents conditional branching.
    /// </summary>
    [JsonDerivedType(typeof(IfElseNode), typeDiscriminator: "if-else-node")]
    public class IfElseNode : NodeDefinition
    {
        /// <summary>
        /// Gets or sets the condition to evaluate.
        /// </summary>
        public string Condition { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the ID of the node to execute when condition is true.
        /// </summary>
        public string TrueNodeId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the ID of the node to execute when condition is false.
        /// </summary>
        public string FalseNodeId { get; set; } = string.Empty;
    }

    /// <summary>
    /// Node that performs HTTP requests.
    /// </summary>
    [JsonDerivedType(typeof(HttpRequestNode), typeDiscriminator: "http-request-node")]
    public class HttpRequestNode : NodeDefinition
    {
        /// <summary>
        /// Gets or sets the HTTP method (GET, POST, etc.).
        /// </summary>
        public string Method { get; set; } = "GET";

        /// <summary>
        /// Gets or sets the URL to request.
        /// </summary>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the request headers.
        /// </summary>
        public Dictionary<string, string> Headers { get; set; } = new();

        /// <summary>
        /// Gets or sets the request body.
        /// </summary>
        public string Body { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the timeout in milliseconds.
        /// </summary>
        public int TimeoutMs { get; set; } = 30000;

        /// <summary>
        /// Gets or sets a value indicating whether to validate SSL certificates.
        /// </summary>
        public bool ValidateSsl { get; set; } = true;
    }

    /// <summary>
    /// Node that delays execution.
    /// </summary>
    [JsonDerivedType(typeof(DelayNode), typeDiscriminator: "delay-node")]
    public class DelayNode : NodeDefinition
    {
        /// <summary>
        /// Gets or sets the delay duration in milliseconds.
        /// </summary>
        public int DelayMs { get; set; } = 1000;
    }

    /// <summary>
    /// Represents a connection between two nodes in a workflow.
    /// </summary>
    public class WorkflowConnection
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
        /// Gets or sets the label for this connection.
        /// </summary>
        public string Label { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the source port/output name.
        /// </summary>
        public string SourcePort { get; set; } = "default";

        /// <summary>
        /// Gets or sets the target port/input name.
        /// </summary>
        public string TargetPort { get; set; } = "default";
    }

    /// <summary>
    /// Represents a complete workflow definition with nodes and connections.
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
        public List<WorkflowConnection> Connections { get; set; } = new();

        /// <summary>
        /// Gets or sets the ID of the start node.
        /// </summary>
        public string StartNodeId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets workflow-level variables.
        /// </summary>
        public Dictionary<string, object> Variables { get; set; } = new();

        /// <summary>
        /// Gets or sets the creation timestamp.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the last modified timestamp.
        /// </summary>
        public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
    }
}
