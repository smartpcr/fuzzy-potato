// -----------------------------------------------------------------------
// <copyright file="WorkflowExecutionContext.cs" company="Microsoft Corp.">
//     Copyright (c) Microsoft Corp. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Tests.Examples.Runs
{
    /// <summary>
    /// Context for workflow execution containing runtime state.
    /// </summary>
    public class WorkflowExecutionContext
    {
        /// <summary>
        /// Gets or sets the workflow variables.
        /// </summary>
        public Dictionary<string, object> Variables { get; set; } = new();

        /// <summary>
        /// Gets or sets the execution trace for debugging.
        /// </summary>
        public List<string> ExecutionTrace { get; set; } = new();

        /// <summary>
        /// Gets or sets a value indicating whether the workflow was cancelled.
        /// </summary>
        public bool IsCancelled { get; set; }

        /// <summary>
        /// Gets or sets the workflow start time.
        /// </summary>
        public DateTime StartTime { get; set; } = DateTime.UtcNow;
    }
}