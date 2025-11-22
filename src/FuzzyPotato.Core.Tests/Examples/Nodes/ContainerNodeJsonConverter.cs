// -----------------------------------------------------------------------
// <copyright file="ContainerNodeJsonConverter.cs" company="FuzzyPotato">
//     Copyright (c) FuzzyPotato. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Tests.Examples.Nodes
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using FuzzyPotato.Core.Tests.Examples;

    /// <summary>
    /// JSON converter for ContainerNode.
    /// </summary>
    public class ContainerNodeJsonConverter : JsonConverter<ContainerNode>
    {
        /// <inheritdoc/>
        public override ContainerNode? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;

            var node = new ContainerNode();

            foreach (var property in root.EnumerateObject())
            {
                switch (property.Name)
                {
                    case "$type":
                    case "typeName":
                        // Skip discriminator
                        break;
                    case "childNodes":
                        node.ChildNodes = JsonSerializer.Deserialize<List<NodeDefinition>>(property.Value.GetRawText(), options) ?? new List<NodeDefinition>();
                        break;
                    case "childConnections":
                        node.ChildConnections = JsonSerializer.Deserialize<List<NodeConnection>>(property.Value.GetRawText(), options) ?? new List<NodeConnection>();
                        break;
                    case "executionMode":
                        node.ExecutionMode = property.Value.GetString() ?? string.Empty;
                        break;
                    case "failFast":
                        node.FailFast = property.Value.GetBoolean();
                        break;
                    case "timeoutMs":
                        node.TimeoutMs = property.Value.GetInt32();
                        break;
                    case "aggregateOutputs":
                        node.AggregateOutputs = property.Value.GetBoolean();
                        break;
                    case "nodeId":
                        node.NodeId = property.Value.GetString() ?? string.Empty;
                        break;
                    case "nodeName":
                        node.NodeName = property.Value.GetString() ?? string.Empty;
                        break;
                    case "description":
                        node.Description = property.Value.GetString() ?? string.Empty;
                        break;
                    case "configuration":
                        node.Configuration = JsonSerializer.Deserialize<Dictionary<string, object>>(property.Value.GetRawText());
                        break;
                    case "positionX":
                        node.PositionX = property.Value.GetDouble();
                        break;
                    case "positionY":
                        node.PositionY = property.Value.GetDouble();
                        break;
                    case "enabled":
                        node.Enabled = property.Value.GetBoolean();
                        break;
                }
            }

            return node;
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, ContainerNode value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("$type", value.TypeName);
            writer.WritePropertyName("childNodes");
            JsonSerializer.Serialize(writer, value.ChildNodes, options);
            writer.WritePropertyName("childConnections");
            JsonSerializer.Serialize(writer, value.ChildConnections, options);
            writer.WriteString("executionMode", value.ExecutionMode);
            writer.WriteBoolean("failFast", value.FailFast);
            writer.WriteNumber("timeoutMs", value.TimeoutMs);
            writer.WriteBoolean("aggregateOutputs", value.AggregateOutputs);
            writer.WriteString("nodeId", value.NodeId);
            writer.WriteString("nodeName", value.NodeName);
            writer.WriteString("description", value.Description);
            if (value.Configuration != null)
            {
                writer.WritePropertyName("configuration");
                JsonSerializer.Serialize(writer, value.Configuration, options);
            }

            writer.WriteNumber("positionX", value.PositionX);
            writer.WriteNumber("positionY", value.PositionY);
            writer.WriteBoolean("enabled", value.Enabled);
            writer.WriteEndObject();
        }
    }
}
