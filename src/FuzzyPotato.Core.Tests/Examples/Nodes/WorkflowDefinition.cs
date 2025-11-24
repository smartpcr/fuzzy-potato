// -----------------------------------------------------------------------
// <copyright file="WorkflowDefinition.cs" company="Microsoft Corp.">
//     Copyright (c) Microsoft Corp. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Tests.Examples.Nodes
{
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