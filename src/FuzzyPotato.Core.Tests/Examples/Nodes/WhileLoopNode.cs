// -----------------------------------------------------------------------
// <copyright file="WhileLoopNode.cs" company="Microsoft Corp.">
//     Copyright (c) Microsoft Corp. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Tests.Examples.Nodes
{
    using System.Text.Json.Serialization;
    using FuzzyPotato.Core.Tests.Examples.Nodes.Converters;

    /// <summary>
    /// Legacy alias for WhileNode for backwards compatibility.
    /// </summary>
    [JsonConverter(typeof(WhileLoopNodeJsonConverter))]
    public class WhileLoopNode : WhileNode
    {
        /// <inheritdoc/>
        public override string TypeName => "while-loop-node";

        /// <summary>
        /// Gets or sets the ID of the first node in the loop body.
        /// </summary>
        public string? LoopBodyStartNodeId { get; set; }
    }
}