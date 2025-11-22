// -----------------------------------------------------------------------
// <copyright file="CSharpTaskNodeYamlConverter.cs" company="FuzzyPotato">
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
    /// YAML converter for CSharpTaskNode.
    /// </summary>
    public class CSharpTaskNodeYamlConverter : IYamlTypeConverter
    {
        /// <inheritdoc/>
        public bool Accepts(Type type)
        {
            return type == typeof(CSharpTaskNode);
        }

        /// <inheritdoc/>
        public object? ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
        {
            if (parser.Current is not MappingStart)
            {
                throw new YamlException("Expected mapping start");
            }

            parser.MoveNext();
            var node = new CSharpTaskNode();

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
                        case "scriptContent":
                            node.ScriptContent = parser.Consume<Scalar>().Value;
                            break;
                        case "assemblyPath":
                            node.AssemblyPath = parser.Consume<Scalar>().Value;
                            break;
                        case "typeName_Executor":
                            node.TypeName_Executor = parser.Consume<Scalar>().Value;
                            break;
                        case "references":
                            node.References = this.ReadStringList(parser);
                            break;
                        case "imports":
                            node.Imports = this.ReadStringList(parser);
                            break;
                        case "timeoutMs":
                            node.TimeoutMs = int.Parse(parser.Consume<Scalar>().Value);
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
            if (value is not CSharpTaskNode node)
            {
                emitter.Emit(new Scalar(string.Empty));
                return;
            }

            emitter.Emit(new MappingStart());
            emitter.Emit(new Scalar("$type"));
            emitter.Emit(new Scalar(node.TypeName));
            if (node.ScriptContent != null)
            {
                emitter.Emit(new Scalar("scriptContent"));
                emitter.Emit(new Scalar(node.ScriptContent));
            }

            if (node.AssemblyPath != null)
            {
                emitter.Emit(new Scalar("assemblyPath"));
                emitter.Emit(new Scalar(node.AssemblyPath));
            }

            if (node.TypeName_Executor != null)
            {
                emitter.Emit(new Scalar("typeName_Executor"));
                emitter.Emit(new Scalar(node.TypeName_Executor));
            }

            emitter.Emit(new Scalar("references"));
            this.WriteStringList(emitter, node.References);
            emitter.Emit(new Scalar("imports"));
            this.WriteStringList(emitter, node.Imports);
            emitter.Emit(new Scalar("timeoutMs"));
            emitter.Emit(new Scalar(node.TimeoutMs.ToString()));
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

        private List<string> ReadStringList(IParser parser)
        {
            var list = new List<string>();
            if (parser.Current is not SequenceStart)
            {
                return list;
            }

            parser.MoveNext();
            while (parser.Current is not SequenceEnd)
            {
                if (parser.Current is Scalar scalar)
                {
                    list.Add(scalar.Value);
                }

                parser.MoveNext();
            }

            parser.MoveNext();
            return list;
        }

        private void WriteStringList(IEmitter emitter, List<string> list)
        {
            emitter.Emit(new SequenceStart(null, null, false, SequenceStyle.Block));
            foreach (var item in list)
            {
                emitter.Emit(new Scalar(item));
            }

            emitter.Emit(new SequenceEnd());
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
