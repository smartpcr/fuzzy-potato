// -----------------------------------------------------------------------
// <copyright file="NodeExecutorBase.cs" company="Microsoft Corp.">
//     Copyright (c) Microsoft Corp. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Tests.Examples
{
    using FuzzyPotato.Core.Tests.Examples.Nodes;

    /// <summary>
    /// Base class for node executors.
    /// </summary>
    /// <typeparam name="TDefinition">The node definition type.</typeparam>
    public abstract class NodeExecutorBase<TDefinition> : IExecutableNode
        where TDefinition : NodeDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NodeExecutorBase{TDefinition}"/> class.
        /// </summary>
        /// <param name="definition">The node definition.</param>
        protected NodeExecutorBase(TDefinition definition)
        {
            this.TypedDefinition = definition;
        }

        /// <inheritdoc/>
        public string NodeId => this.TypedDefinition.NodeId;

        /// <inheritdoc/>
        public NodeDefinition Definition => this.TypedDefinition;

        /// <summary>
        /// Gets the typed node definition.
        /// </summary>
        protected TDefinition TypedDefinition { get; }

        /// <inheritdoc/>
        public abstract Task<NodeExecutionResult> ExecuteAsync(
            WorkflowExecutionContext context,
            CancellationToken cancellationToken = default);
    }
}