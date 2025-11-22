// -----------------------------------------------------------------------
// <copyright file="NodeFactory.cs" company="Microsoft Corp.">
//     Copyright (c) Microsoft Corp. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Tests.Examples.Runs
{
    using FuzzyPotato.Core.Tests.Examples.Nodes;

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
                nodes[nodeDef.NodeId] = this.CreateNode(nodeDef);
            }

            return nodes;
        }
    }
}