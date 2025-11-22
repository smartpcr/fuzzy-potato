// -----------------------------------------------------------------------
// <copyright file="HttpRequestNodeExecutor.cs" company="Microsoft Corp.">
//     Copyright (c) Microsoft Corp. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Tests.Examples
{
    using FuzzyPotato.Core.Tests.Examples.Nodes;

    /// <summary>
    /// Executor for HTTP request nodes.
    /// </summary>
    public class HttpRequestNodeExecutor : NodeExecutorBase<HttpRequestNode>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRequestNodeExecutor"/> class.
        /// </summary>
        /// <param name="definition">The node definition.</param>
        public HttpRequestNodeExecutor(HttpRequestNode definition)
            : base(definition)
        {
        }

        /// <inheritdoc/>
        public override async Task<NodeExecutionResult> ExecuteAsync(
            WorkflowExecutionContext context,
            CancellationToken cancellationToken = default)
        {
            context.ExecutionTrace.Add($"HTTP {this.TypedDefinition.Method} request to: {this.TypedDefinition.Url}");

            // Simulated HTTP request - in real implementation, use HttpClient
            await Task.Delay(50, cancellationToken);

            return new NodeExecutionResult
            {
                Success = true,
                Output = $"HTTP {this.TypedDefinition.Method} response from {this.TypedDefinition.Url}",
            };
        }
    }
}