// -----------------------------------------------------------------------
// <copyright file="SwitchNodeYamlConverter.cs" company="FuzzyPotato">
//     Copyright (c) FuzzyPotato. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Tests.Examples.Nodes
{
    using System;
    using System.Collections.Generic;
    using YamlDotNet.Core;
    using YamlDotNet.Core.Events;
    using YamlDotNet.Serialization;

    /// <summary>
    /// YAML converter for SwitchNode.
    /// </summary>
    public class SwitchNodeYamlConverter : IYamlTypeConverter
    {
        /// <inheritdoc/>
        public bool Accepts(Type type)
        {
            return type == typeof(SwitchNode);
        }

        /// <inheritdoc/>
        public object? ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
        {
            if (parser.Current is not MappingStart)
            {
                throw new YamlException("Expected mapping start");
            }

            parser.MoveNext();
            var node = new SwitchNode();

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
                        case "expression":
                            node.Expression = parser.Consume<Scalar>().Value;
                            break;
                        case "cases":
                            node.Cases = this.ReadStringDict(parser);
                            break;
                        case "defaultPort":
                            node.DefaultPort = parser.Consume<Scalar>().Value;
                            break;
                        case "caseSensitive":
                            node.CaseSensitive = bool.Parse(parser.Consume<Scalar>().Value);
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
            if (value is not SwitchNode node)
            {
                emitter.Emit(new Scalar(string.Empty));
                return;
            }

            emitter.Emit(new MappingStart());
            emitter.Emit(new Scalar("$type"));
            emitter.Emit(new Scalar(node.TypeName));
            emitter.Emit(new Scalar("expression"));
            emitter.Emit(new Scalar(node.Expression));
            emitter.Emit(new Scalar("cases"));
            this.WriteStringDict(emitter, node.Cases);
            emitter.Emit(new Scalar("defaultPort"));
            emitter.Emit(new Scalar(node.DefaultPort));
            emitter.Emit(new Scalar("caseSensitive"));
            emitter.Emit(new Scalar(node.CaseSensitive.ToString().ToLower()));
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

        private Dictionary<string, string> ReadStringDict(IParser parser)
        {
            var dict = new Dictionary<string, string>();
            if (parser.Current is not MappingStart)
            {
                return dict;
            }

            parser.MoveNext();
            while (parser.Current is not MappingEnd)
            {
                if (parser.Current is Scalar keyScalar)
                {
                    var key = keyScalar.Value;
                    parser.MoveNext();
                    if (parser.Current is Scalar valueScalar)
                    {
                        dict[key] = valueScalar.Value;
                    }

                    parser.MoveNext();
                }
                else
                {
                    parser.MoveNext();
                }
            }

            parser.MoveNext();
            return dict;
        }

        private void WriteStringDict(IEmitter emitter, Dictionary<string, string> dict)
        {
            emitter.Emit(new MappingStart());
            foreach (var kvp in dict)
            {
                emitter.Emit(new Scalar(kvp.Key));
                emitter.Emit(new Scalar(kvp.Value));
            }

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
