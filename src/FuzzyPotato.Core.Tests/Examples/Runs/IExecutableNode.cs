// -----------------------------------------------------------------------
// <copyright file="IExecutableNode.cs" company="Microsoft Corp.">
//     Copyright (c) Microsoft Corp. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Tests.Examples
{
    using FuzzyPotato.Core.Tests.Examples.Nodes;

    /// <summary>
    /// Interface for executable workflow nodes.
    /// </summary>
    public interface IExecutableNode
    {
        /// <summary>
        /// Gets the node ID.
        /// </summary>
        string NodeId { get; }

        /// <summary>
        /// Gets the node definition.
        /// </summary>
        NodeDefinition Definition { get; }

        /// <summary>
        /// Executes the node.
        /// </summary>
        /// <param name="context">The execution context.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The execution result.</returns>
        Task<NodeExecutionResult> ExecuteAsync(
            WorkflowExecutionContext context,
            CancellationToken cancellationToken = default);
    }
}