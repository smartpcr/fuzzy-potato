// -----------------------------------------------------------------------
// <copyright file="WhileNode.cs" company="Microsoft Corp.">
//     Copyright (c) Microsoft Corp. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Tests.Examples.Nodes
{
    using System.Text.Json.Serialization;
    using FuzzyPotato.Core.Tests.Examples.Nodes.Converters;

    /// <summary>
    /// Node that iterates while a condition is true with feedback loop architecture.
    /// Re-evaluates condition after each iteration based on updated workflow state.
    /// </summary>
    [JsonConverter(typeof(WhileNodeJsonConverter))]
    public class WhileNode : NodeDefinition
    {
        /// <inheritdoc/>
        public override string TypeName => "while-node";

        /// <summary>
        /// Gets or sets the C# boolean expression to evaluate before each iteration.
        /// </summary>
        public string Condition { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the maximum number of iterations to prevent infinite loops.
        /// Defaults to 1000.
        /// </summary>
        public int MaxIterations { get; set; } = 1000;

        /// <summary>
        /// Gets or sets a value indicating whether to break the loop on error.
        /// </summary>
        public bool BreakOnError { get; set; } = true;
    }
}