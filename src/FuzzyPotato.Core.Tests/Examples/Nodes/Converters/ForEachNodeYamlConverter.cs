// -----------------------------------------------------------------------
// <copyright file="ForEachNodeYamlConverter.cs" company="FuzzyPotato">
//     Copyright (c) FuzzyPotato. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Tests.Examples.Nodes.Converters
{
    using System;
    using FuzzyPotato.Core.Tests.Examples.Nodes;
    using YamlDotNet.Core;
    using YamlDotNet.Core.Events;
    using YamlDotNet.Serialization;

    /// <summary>
    /// YAML converter for ForEachNode.
    /// </summary>
    public class ForEachNodeYamlConverter : IYamlTypeConverter
    {
        /// <inheritdoc/>
        public bool Accepts(Type type)
        {
            return type == typeof(ForEachNode);
        }

        /// <inheritdoc/>
        public object? ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
        {
            if (parser.Current is not MappingStart)
            {
                throw new YamlException("Expected mapping start");
            }

            parser.MoveNext();
            var node = new ForEachNode();

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
                        case "collectionExpression":
                            node.CollectionExpression = parser.Consume<Scalar>().Value;
                            break;
                        case "itemVariableName":
                            node.ItemVariableName = parser.Consume<Scalar>().Value;
                            break;
                        case "continueOnError":
                            node.ContinueOnError = bool.Parse(parser.Consume<Scalar>().Value);
                            break;
                        case "maxDegreeOfParallelism":
                            node.MaxDegreeOfParallelism = int.Parse(parser.Consume<Scalar>().Value);
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
            if (value is not ForEachNode node)
            {
                emitter.Emit(new Scalar(string.Empty));
                return;
            }

            emitter.Emit(new MappingStart());
            emitter.Emit(new Scalar("$type"));
            emitter.Emit(new Scalar(node.TypeName));
            emitter.Emit(new Scalar("collectionExpression"));
            emitter.Emit(new Scalar(node.CollectionExpression));
            emitter.Emit(new Scalar("itemVariableName"));
            emitter.Emit(new Scalar(node.ItemVariableName));
            emitter.Emit(new Scalar("continueOnError"));
            emitter.Emit(new Scalar(node.ContinueOnError.ToString().ToLower()));
            emitter.Emit(new Scalar("maxDegreeOfParallelism"));
            emitter.Emit(new Scalar(node.MaxDegreeOfParallelism.ToString()));
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
