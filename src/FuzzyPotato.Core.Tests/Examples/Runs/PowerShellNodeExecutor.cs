// -----------------------------------------------------------------------
// <copyright file="PowerShellNodeExecutor.cs" company="Microsoft Corp.">
//     Copyright (c) Microsoft Corp. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Tests.Examples
{
    using FuzzyPotato.Core.Tests.Examples.Nodes;

    /// <summary>
    /// Executor for PowerShell nodes.
    /// </summary>
    public class PowerShellNodeExecutor : NodeExecutorBase<PowerShellScriptNode>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PowerShellNodeExecutor"/> class.
        /// </summary>
        /// <param name="definition">The node definition.</param>
        public PowerShellNodeExecutor(PowerShellScriptNode definition)
            : base(definition)
        {
        }

        /// <inheritdoc/>
        public override async Task<NodeExecutionResult> ExecuteAsync(
            WorkflowExecutionContext context,
            CancellationToken cancellationToken = default)
        {
            context.ExecutionTrace.Add($"Executing PowerShell node: {this.NodeId} - {this.TypedDefinition.NodeName}");

            // Simulated execution - in real implementation, use System.Management.Automation
            await Task.Delay(100, cancellationToken);

            return new NodeExecutionResult
            {
                Success = true,
                Output = $"Executed PowerShell script with {this.TypedDefinition.Parameters.Count} parameters",
            };
        }
    }
}