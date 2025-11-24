// -----------------------------------------------------------------------
// <copyright file="SwitchNode.cs" company="Microsoft Corp.">
//     Copyright (c) Microsoft Corp. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Tests.Examples.Nodes
{
    using System.Text.Json.Serialization;
    using FuzzyPotato.Core.Tests.Examples.Nodes.Converters;

    /// <summary>
    /// Node that performs multi-way branching based on expression result matching.
    /// Routes to specific port based on which case matches, or Default port if no match.
    /// </summary>
    [JsonConverter(typeof(SwitchNodeJsonConverter))]
    public class SwitchNode : NodeDefinition
    {
        /// <inheritdoc/>
        public override string TypeName => "switch-node";

        /// <summary>
        /// Gets or sets the C# expression to evaluate.
        /// Result is converted to string for case matching.
        /// </summary>
        public string Expression { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the case mappings.
        /// Key: Value to match (e.g., "success", "error").
        /// Value: Port name to route to (if empty, uses key as port name).
        /// </summary>
        public Dictionary<string, string> Cases { get; set; } = new();

        /// <summary>
        /// Gets or sets the default port name when no case matches.
        /// Defaults to "Default".
        /// </summary>
        public string DefaultPort { get; set; } = "Default";

        /// <summary>
        /// Gets or sets a value indicating whether case matching is case-sensitive.
        /// </summary>
        public bool CaseSensitive { get; set; } = true;
    }
}