// -----------------------------------------------------------------------
// <copyright file="ContainerNode.cs" company="Microsoft Corp.">
//     Copyright (c) Microsoft Corp. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Tests.Examples.Nodes
{
    using System.Text.Json.Serialization;
    using FuzzyPotato.Core.Tests.Examples.Nodes.Converters;

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
}