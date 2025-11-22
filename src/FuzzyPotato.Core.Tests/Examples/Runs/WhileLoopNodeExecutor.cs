// -----------------------------------------------------------------------
// <copyright file="WhileLoopNodeExecutor.cs" company="Microsoft Corp.">
//     Copyright (c) Microsoft Corp. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Tests.Examples.Runs
{
    using FuzzyPotato.Core.Tests.Examples.Nodes;

    /// <summary>
    /// Executor for while loop nodes.
    /// </summary>
    public class WhileLoopNodeExecutor : NodeExecutorBase<WhileLoopNode>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WhileLoopNodeExecutor"/> class.
        /// </summary>
        /// <param name="definition">The node definition.</param>
        public WhileLoopNodeExecutor(WhileLoopNode definition)
            : base(definition)
        {
        }

        /// <inheritdoc/>
        public override Task<NodeExecutionResult> ExecuteAsync(
            WorkflowExecutionContext context,
            CancellationToken cancellationToken = default)
        {
            context.ExecutionTrace.Add($"Evaluating while loop: {this.NodeId} - {this.TypedDefinition.Condition}");

            // Simulated condition evaluation
            var result = new NodeExecutionResult
            {
                Success = true,
                NextNodeId = this.TypedDefinition.LoopBodyStartNodeId, // Continue to loop body or next node based on condition
            };

            return Task.FromResult(result);
        }
    }
}