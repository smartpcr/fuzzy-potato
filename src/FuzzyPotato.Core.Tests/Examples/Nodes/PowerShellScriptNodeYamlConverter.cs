// -----------------------------------------------------------------------
// <copyright file="PowerShellScriptNodeYamlConverter.cs" company="FuzzyPotato">
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
    /// YAML converter for PowerShellScriptNode.
    /// </summary>
    public class PowerShellScriptNodeYamlConverter : IYamlTypeConverter
    {
        /// <inheritdoc/>
        public bool Accepts(Type type) => type == typeof(PowerShellScriptNode);

        /// <inheritdoc/>
        public object? ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
        {
            if (parser.Current is not MappingStart)
            {
                throw new YamlException("Expected mapping start");
            }

            parser.MoveNext();
            var node = new PowerShellScriptNode();

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
                        case "scriptPath":
                            node.ScriptPath = parser.Consume<Scalar>().Value;
                            break;
                        case "requiredModules":
                            node.RequiredModules = this.ReadStringList(parser);
                            break;
                        case "modulePaths":
                            node.ModulePaths = this.ReadStringDict(parser);
                            break;
                        case "executionPolicy":
                            node.ExecutionPolicy = parser.Consume<Scalar>().Value;
                            break;
                        case "captureVerbose":
                            node.CaptureVerbose = bool.Parse(parser.Consume<Scalar>().Value);
                            break;
                        case "parameters":
                            node.Parameters = rootDeserializer(typeof(Dictionary<string, object>)) as Dictionary<string, object> ?? new();
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
            if (value is not PowerShellScriptNode node)
            {
                return;
            }

            emitter.Emit(new MappingStart());
            emitter.Emit(new Scalar("$type"));
            emitter.Emit(new Scalar(node.TypeName));
            emitter.Emit(new Scalar("scriptPath"));
            emitter.Emit(new Scalar(node.ScriptPath));
            emitter.Emit(new Scalar("requiredModules"));
            this.WriteStringList(emitter, node.RequiredModules);
            emitter.Emit(new Scalar("modulePaths"));
            this.WriteStringDict(emitter, node.ModulePaths);
            emitter.Emit(new Scalar("executionPolicy"));
            emitter.Emit(new Scalar(node.ExecutionPolicy));
            emitter.Emit(new Scalar("parameters"));
            serializer(node.Parameters, typeof(Dictionary<string, object>));
            emitter.Emit(new Scalar("captureVerbose"));
            emitter.Emit(new Scalar(node.CaptureVerbose.ToString().ToLower()));
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

        private void WriteStringList(IEmitter emitter, List<string> list)
        {
            emitter.Emit(new SequenceStart(null, null, false, SequenceStyle.Block));
            foreach (var item in list)
            {
                emitter.Emit(new Scalar(item));
            }

            emitter.Emit(new SequenceEnd());
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
