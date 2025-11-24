// -----------------------------------------------------------------------
// <copyright file="DelayNode.cs" company="Microsoft Corp.">
//     Copyright (c) Microsoft Corp. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Tests.Examples.Nodes
{
    using System.Text.Json.Serialization;
    using FuzzyPotato.Core.Tests.Examples.Nodes.Converters;

    /// <summary>
    /// Node that delays execution for a specified duration.
    /// </summary>
    [JsonConverter(typeof(DelayNodeJsonConverter))]
    public class DelayNode : NodeDefinition
    {
        /// <inheritdoc/>
        public override string TypeName => "delay-node";

        /// <summary>
        /// Gets or sets the delay duration in milliseconds.
        /// </summary>
        public int DelayMs { get; set; } = 1000;

        /// <summary>
        /// Gets or sets a value indicating whether the delay can be cancelled.
        /// </summary>
        public bool Cancellable { get; set; } = true;
    }
}