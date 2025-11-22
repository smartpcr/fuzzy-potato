// -----------------------------------------------------------------------
// <copyright file="ContainerNodeYamlConverter.cs" company="FuzzyPotato">
//     Copyright (c) FuzzyPotato. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Tests.Examples.Nodes
{
    using System;
    using System.Collections.Generic;
    using FuzzyPotato.Core.Tests.Examples;
    using YamlDotNet.Core;
    using YamlDotNet.Core.Events;
    using YamlDotNet.Serialization;

    /// <summary>
    /// YAML converter for ContainerNode.
    /// </summary>
    public class ContainerNodeYamlConverter : IYamlTypeConverter
    {
        /// <inheritdoc/>
        public bool Accepts(Type type)
        {
            return type == typeof(ContainerNode);
        }

        /// <inheritdoc/>
        public object? ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
        {
            if (parser.Current is not MappingStart)
            {
                throw new YamlException("Expected mapping start");
            }

            parser.MoveNext();
            var node = new ContainerNode();

            while (parser.Current is not MappingEnd)
            {
                if (parser.Current is Scalar scalar)
                {
                    var propertyName = scalar.Value;
                    parser.MoveNext();

                    switch (propertyName)
                    {
                        case "$type":
                        case "typeName":
                            parser.MoveNext();
                            break;
                        case "childNodes":
                            node.ChildNodes = (List<NodeDefinition>?)rootDeserializer(typeof(List<NodeDefinition>)) ?? new List<NodeDefinition>();
                            break;
                        case "childConnections":
                            node.ChildConnections = (List<NodeConnection>?)rootDeserializer(typeof(List<NodeConnection>)) ?? new List<NodeConnection>();
                            break;
                        case "executionMode":
                            node.ExecutionMode = parser.Consume<Scalar>().Value;
                            break;
                        case "failFast":
                            node.FailFast = bool.Parse(parser.Consume<Scalar>().Value);
                            break;
                        case "timeoutMs":
                            node.TimeoutMs = int.Parse(parser.Consume<Scalar>().Value);
                            break;
                        case "aggregateOutputs":
                            node.AggregateOutputs = bool.Parse(parser.Consume<Scalar>().Value);
                            break;
                        case "nodeId":
                            node.NodeId = parser.Consume<Scalar>().Value;
                            break;
                        case "nodeName":
                            node.NodeName = parser.Consume<Scalar>().Value;
                            break;
                        case "description":
                            node.Description = parser.Consume<Scalar>().Value;
                            break;
                        case "positionX":
                            node.PositionX = double.Parse(parser.Consume<Scalar>().Value);
                            break;
                        case "positionY":
                            node.PositionY = double.Parse(parser.Consume<Scalar>().Value);
                            break;
                        case "enabled":
                            node.Enabled = bool.Parse(parser.Consume<Scalar>().Value);
                            break;
                        default:
                            this.SkipValue(parser);
                            break;
                    }
                }
                else
                {
                    parser.MoveNext();
                }
            }

            parser.MoveNext();
            return node;
        }

        /// <inheritdoc/>
        public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
        {
            if (value is not ContainerNode node)
            {
                emitter.Emit(new Scalar(string.Empty));
                return;
            }

            emitter.Emit(new MappingStart());
            emitter.Emit(new Scalar("$type"));
            emitter.Emit(new Scalar(node.TypeName));
            emitter.Emit(new Scalar("childNodes"));
            serializer(node.ChildNodes, typeof(List<NodeDefinition>));
            emitter.Emit(new Scalar("childConnections"));
            serializer(node.ChildConnections, typeof(List<NodeConnection>));
            emitter.Emit(new Scalar("executionMode"));
            emitter.Emit(new Scalar(node.ExecutionMode));
            emitter.Emit(new Scalar("failFast"));
            emitter.Emit(new Scalar(node.FailFast.ToString().ToLower()));
            emitter.Emit(new Scalar("timeoutMs"));
            emitter.Emit(new Scalar(node.TimeoutMs.ToString()));
            emitter.Emit(new Scalar("aggregateOutputs"));
            emitter.Emit(new Scalar(node.AggregateOutputs.ToString().ToLower()));
            emitter.Emit(new Scalar("nodeId"));
            emitter.Emit(new Scalar(node.NodeId));
            emitter.Emit(new Scalar("nodeName"));
            emitter.Emit(new Scalar(node.NodeName));
            emitter.Emit(new Scalar("description"));
            emitter.Emit(new Scalar(node.Description));
            emitter.Emit(new Scalar("positionX"));
            emitter.Emit(new Scalar(node.PositionX.ToString()));
            emitter.Emit(new Scalar("positionY"));
            emitter.Emit(new Scalar(node.PositionY.ToString()));
            emitter.Emit(new Scalar("enabled"));
            emitter.Emit(new Scalar(node.Enabled.ToString().ToLower()));
            emitter.Emit(new MappingEnd());
        }

        private void SkipValue(IParser parser)
        {
            var depth = 0;
            do
            {
                if (parser.Current is MappingStart or SequenceStart)
                {
                    depth++;
                }
                else if (parser.Current is MappingEnd or SequenceEnd)
                {
                    depth--;
                }

                parser.MoveNext();
            }
            while (depth > 0);
        }
    }
}
