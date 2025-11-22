// -----------------------------------------------------------------------
// <copyright file="DelayNodeJsonConverter.cs" company="FuzzyPotato">
//     Copyright (c) FuzzyPotato. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Tests.Examples.Nodes.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using FuzzyPotato.Core.Tests.Examples.Nodes;

    /// <summary>
    /// JSON converter for DelayNode.
    /// </summary>
    public class DelayNodeJsonConverter : JsonConverter<DelayNode>
    {
        /// <inheritdoc/>
        public override DelayNode? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;

            var node = new DelayNode();

            foreach (var property in root.EnumerateObject())
            {
                switch (property.Name)
                {
                    case "$type":
                    case "typeName":
                        // Skip discriminator
                        break;
                    case "delayMs":
                        node.DelayMs = property.Value.GetInt32();
                        break;
                    case "cancellable":
                        node.Cancellable = property.Value.GetBoolean();
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
        public override void Write(Utf8JsonWriter writer, DelayNode value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("$type", value.TypeName);
            writer.WriteNumber("delayMs", value.DelayMs);
            writer.WriteBoolean("cancellable", value.Cancellable);
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
