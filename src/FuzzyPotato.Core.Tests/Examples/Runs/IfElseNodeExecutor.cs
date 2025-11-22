// -----------------------------------------------------------------------
// <copyright file="IfElseNodeExecutor.cs" company="Microsoft Corp.">
//     Copyright (c) Microsoft Corp. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Tests.Examples
{
    using FuzzyPotato.Core.Tests.Examples.Nodes;

    /// <summary>
    /// Executor for if-else nodes.
    /// </summary>
    public class IfElseNodeExecutor : NodeExecutorBase<IfElseNode>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IfElseNodeExecutor"/> class.
        /// </summary>
        /// <param name="definition">The node definition.</param>
        public IfElseNodeExecutor(IfElseNode definition)
            : base(definition)
        {
        }

        /// <inheritdoc/>
        public override Task<NodeExecutionResult> ExecuteAsync(
            WorkflowExecutionContext context,
            CancellationToken cancellationToken = default)
        {
            context.ExecutionTrace.Add($"Evaluating condition: {this.NodeId} - {this.TypedDefinition.Condition}");

            // Simulated condition evaluation - in real implementation, use expression evaluator
            var conditionResult = true; // Placeholder

            var result = new NodeExecutionResult
            {
                Success = true,
                NextNodeId = conditionResult ? this.TypedDefinition.TrueNodeId : this.TypedDefinition.FalseNodeId,
            };

            return Task.FromResult(result);
        }
    }
}