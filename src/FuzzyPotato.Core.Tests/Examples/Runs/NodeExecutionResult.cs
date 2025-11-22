// -----------------------------------------------------------------------
// <copyright file="WorkflowRuntime.cs" company="FuzzyPotato">
//     Copyright (c) FuzzyPotato. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Tests.Examples.Runs
{
    /// <summary>
    /// Result of a node execution.
    /// </summary>
    public class NodeExecutionResult
    {
        /// <summary>
        /// Gets or sets a value indicating whether the execution was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the output value from the node.
        /// </summary>
        public object? Output { get; set; }

        /// <summary>
        /// Gets or sets the error message if execution failed.
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the ID of the next node to execute.
        /// </summary>
        public string? NextNodeId { get; set; }
    }

    // Concrete executor implementations
}
