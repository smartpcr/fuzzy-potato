// -----------------------------------------------------------------------
// <copyright file="WorkflowRuntime.cs" company="FuzzyPotato">
//     Copyright (c) FuzzyPotato. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Tests.Examples
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using FuzzyPotato.Core.Models;
    using FuzzyPotato.Core.Tests.Examples.Nodes;

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

    /// <summary>
    /// Factory for creating executable node instances from definitions.
    /// </summary>
    public class NodeFactory
    {
        private readonly Dictionary<string, Func<NodeDefinition, IExecutableNode>> factoryMethods = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeFactory"/> class.
        /// </summary>
        public NodeFactory()
        {
            // Register default node factories
            this.RegisterFactory<CSharpNode>(def => new CSharpNodeExecutor((CSharpNode)def));
            this.RegisterFactory<PowerShellScriptNode>(def => new PowerShellNodeExecutor((PowerShellScriptNode)def));
            this.RegisterFactory<WhileLoopNode>(def => new WhileLoopNodeExecutor((WhileLoopNode)def));
            this.RegisterFactory<IfElseNode>(def => new IfElseNodeExecutor((IfElseNode)def));
            this.RegisterFactory<HttpRequestNode>(def => new HttpRequestNodeExecutor((HttpRequestNode)def));
            this.RegisterFactory<DelayNode>(def => new DelayNodeExecutor((DelayNode)def));
        }

        /// <summary>
        /// Registers a factory method for a specific node type.
        /// </summary>
        /// <typeparam name="TNode">The node definition type.</typeparam>
        /// <param name="factory">The factory method.</param>
        public void RegisterFactory<TNode>(Func<NodeDefinition, IExecutableNode> factory)
            where TNode : NodeDefinition, new()
        {
            var instance = new TNode();
            var discriminator = instance.TypeName;
            this.factoryMethods[discriminator] = factory;
        }

        /// <summary>
        /// Creates an executable node from a definition.
        /// </summary>
        /// <param name="definition">The node definition.</param>
        /// <returns>An executable node instance.</returns>
        public IExecutableNode CreateNode(NodeDefinition definition)
        {
            var discriminator = definition.TypeName;

            if (!this.factoryMethods.TryGetValue(discriminator, out var factory))
            {
                throw new InvalidOperationException($"No factory registered for node type: {discriminator}");
            }

            return factory(definition);
        }

        /// <summary>
        /// Creates all nodes from a workflow definition.
        /// </summary>
        /// <param name="workflow">The workflow definition.</param>
        /// <returns>Dictionary of node ID to executable node.</returns>
        public Dictionary<string, IExecutableNode> CreateWorkflowNodes(WorkflowDefinition workflow)
        {
            var nodes = new Dictionary<string, IExecutableNode>();
            foreach (var nodeDef in workflow.Nodes)
            {
                nodes[nodeDef.Id] = this.CreateNode(nodeDef);
            }

            return nodes;
        }
    }

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
        public string NodeId => this.TypedDefinition.Id;

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

    // Concrete executor implementations

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
            context.ExecutionTrace.Add($"Executing C# node: {this.NodeId} - {this.TypedDefinition.Name}");

            // Simulated execution - in real implementation, use Roslyn to compile and execute
            await Task.Delay(100, cancellationToken);

            return new NodeExecutionResult
            {
                Success = true,
                Output = $"Executed C# code: {this.TypedDefinition.Code.Substring(0, Math.Min(50, this.TypedDefinition.Code.Length))}...",
            };
        }
    }

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
            context.ExecutionTrace.Add($"Executing PowerShell node: {this.NodeId} - {this.TypedDefinition.Name}");

            // Simulated execution - in real implementation, use System.Management.Automation
            await Task.Delay(100, cancellationToken);

            return new NodeExecutionResult
            {
                Success = true,
                Output = $"Executed PowerShell script with {this.TypedDefinition.Parameters.Count} parameters",
            };
        }
    }

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
