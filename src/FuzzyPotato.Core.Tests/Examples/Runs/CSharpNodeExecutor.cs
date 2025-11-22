// -----------------------------------------------------------------------
// <copyright file="CSharpNodeExecutor.cs" company="Microsoft Corp.">
//     Copyright (c) Microsoft Corp. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Tests.Examples.Runs
{
    using FuzzyPotato.Core.Tests.Examples.Nodes;

    /// <summary>
    /// Executor for C# nodes.
    /// </summary>
    public class CSharpNodeExecutor : NodeExecutorBase<CSharpNode>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpNodeExecutor"/> class.
        /// </summary>
        /// <param name="definition">The node definition.</param>
        public CSharpNodeExecutor(CSharpNode definition)
            : base(definition)
        {
        }

        /// <inheritdoc/>
        public override async Task<NodeExecutionResult> ExecuteAsync(
            WorkflowExecutionContext context,
            CancellationToken cancellationToken = default)
        {
            context.ExecutionTrace.Add($"Executing C# node: {this.NodeId} - {this.TypedDefinition.NodeName}");

            // Simulated execution - in real implementation, use Roslyn to compile and execute
            await Task.Delay(100, cancellationToken);

            return new NodeExecutionResult
            {
                Success = true,
                Output = $"Executed C# code: {this.TypedDefinition.Code.Substring(0, Math.Min(50, this.TypedDefinition.Code.Length))}...",
            };
        }
    }
}