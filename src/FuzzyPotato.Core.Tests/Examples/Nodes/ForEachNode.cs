// -----------------------------------------------------------------------
// <copyright file="ForEachNode.cs" company="Microsoft Corp.">
//     Copyright (c) Microsoft Corp. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Tests.Examples.Nodes
{
    using System.Text.Json.Serialization;
    using FuzzyPotato.Core.Tests.Examples.Nodes.Converters;

    /// <summary>
    /// Node that iterates over a collection and executes child nodes for each item.
    /// Sets workflow variables with current item and iteration index.
    /// </summary>
    [JsonConverter(typeof(ForEachNodeJsonConverter))]
    public class ForEachNode : NodeDefinition
    {
        /// <inheritdoc/>
        public override string TypeName => "foreach-node";

        /// <summary>
        /// Gets or sets the C# expression that returns an IEnumerable collection.
        /// </summary>
        public string CollectionExpression { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the variable name for the current item in each iteration.
        /// Defaults to "item".
        /// </summary>
        public string ItemVariableName { get; set; } = "item";

        /// <summary>
        /// Gets or sets a value indicating whether to continue iteration on item error.
        /// </summary>
        public bool ContinueOnError { get; set; } = false;

        /// <summary>
        /// Gets or sets the maximum number of concurrent iterations (for parallel foreach).
        /// Default 1 means sequential execution.
        /// </summary>
        public int MaxDegreeOfParallelism { get; set; } = 1;
    }
}