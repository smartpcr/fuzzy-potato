// -----------------------------------------------------------------------
// <copyright file="IfElseNode.cs" company="Microsoft Corp.">
//     Copyright (c) Microsoft Corp. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Tests.Examples.Nodes
{
    using System.Text.Json.Serialization;
    using FuzzyPotato.Core.Tests.Examples.Nodes.Converters;

    /// <summary>
    /// Node that performs conditional branching based on a boolean expression.
    /// Routes to TrueBranch or FalseBranch ports based on condition evaluation.
    /// </summary>
    [JsonConverter(typeof(IfElseNodeJsonConverter))]
    public class IfElseNode : NodeDefinition
    {
        /// <inheritdoc/>
        public override string TypeName => "if-else-node";

        /// <summary>
        /// Gets or sets the C# boolean expression to evaluate.
        /// Expression has access to workflow variables via ExecutionState.
        /// </summary>
        public string Condition { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the ID of the node to execute when condition is true.
        /// </summary>
        public string? TrueNodeId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the node to execute when condition is false.
        /// </summary>
        public string? FalseNodeId { get; set; }
    }
}