// -----------------------------------------------------------------------
// <copyright file="NodeConnection.cs" company="FuzzyPotato">
//     Copyright (c) FuzzyPotato. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Tests.Examples.Nodes
{
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
}
