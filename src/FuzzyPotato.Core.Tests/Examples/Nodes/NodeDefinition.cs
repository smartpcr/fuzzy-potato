// -----------------------------------------------------------------------
// <copyright file="NodeDefinition.cs" company="FuzzyPotato">
//     Copyright (c) FuzzyPotato. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Tests.Examples.Nodes
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;
    using FuzzyPotato.Core.Models;
    using YamlDotNet.Serialization;

    /// <summary>
    /// Base class for all node definitions in a workflow.
    /// Represents the configuration of a node that can be executed in a workflow engine.
    /// </summary>
    public abstract class NodeDefinition : PolymorphicBase
    {
        /// <summary>
        /// Gets or sets the unique identifier (hides base class Id property).
        /// </summary>
        [JsonIgnore]
        [YamlIgnore]
        public new string Id
        {
            get => this.NodeId;
            set => this.NodeId = value;
        }

        /// <summary>
        /// Gets or sets the name (hides base class Name property).
        /// </summary>
        [JsonIgnore]
        [YamlIgnore]
        public new string Name
        {
            get => this.NodeName;
            set => this.NodeName = value;
        }

        /// <summary>
        /// Gets or sets the creation timestamp (hides base class property).
        /// </summary>
        [JsonIgnore]
        [YamlIgnore]
        public new System.DateTime CreatedAt { get; set; }
        /// <summary>
        /// Gets or sets the unique identifier for this node instance.
        /// </summary>
        public string NodeId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the display name of the node.
        /// </summary>
        public string NodeName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of what this node does.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets arbitrary configuration data for the node.
        /// Used for node-specific settings that don't warrant dedicated properties.
        /// </summary>
        public Dictionary<string, object>? Configuration { get; set; }

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
}
