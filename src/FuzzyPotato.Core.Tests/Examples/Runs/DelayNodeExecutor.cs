// -----------------------------------------------------------------------
// <copyright file="DelayNodeExecutor.cs" company="Microsoft Corp.">
//     Copyright (c) Microsoft Corp. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Tests.Examples
{
    using FuzzyPotato.Core.Tests.Examples.Nodes;
    using FuzzyPotato.Core.Tests.Examples.Runs;

    /// <summary>
    /// Executor for delay nodes.
    /// </summary>
    public class DelayNodeExecutor : NodeExecutorBase<DelayNode>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DelayNodeExecutor"/> class.
        /// </summary>
        /// <param name="definition">The node definition.</param>
        public DelayNodeExecutor(DelayNode definition)
            : base(definition)
        {
        }

        /// <inheritdoc/>
        public override async Task<NodeExecutionResult> ExecuteAsync(
            WorkflowExecutionContext context,
            CancellationToken cancellationToken = default)
        {
            context.ExecutionTrace.Add($"Delaying for {this.TypedDefinition.DelayMs}ms");

            await Task.Delay(this.TypedDefinition.DelayMs, cancellationToken);

            return new NodeExecutionResult
            {
                Success = true,
                Output = $"Delayed for {this.TypedDefinition.DelayMs}ms",
            };
        }
    }
}