// -----------------------------------------------------------------------
// <copyright file="SubflowNode.cs" company="Microsoft Corp.">
//     Copyright (c) Microsoft Corp. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Tests.Examples.Nodes
{
    using System.Text.Json.Serialization;
    using FuzzyPotato.Core.Tests.Examples.Nodes.Converters;

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
}