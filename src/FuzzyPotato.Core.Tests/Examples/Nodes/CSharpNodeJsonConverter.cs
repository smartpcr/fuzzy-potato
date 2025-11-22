// -----------------------------------------------------------------------
// <copyright file="CSharpNodeJsonConverter.cs" company="FuzzyPotato">
//     Copyright (c) FuzzyPotato. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Tests.Examples.Nodes
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    /// <summary>
    /// JSON converter for CSharpNode.
    /// </summary>
    public class CSharpNodeJsonConverter : JsonConverter<CSharpNode>
    {
        /// <inheritdoc/>
        public override CSharpNode? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;

            var node = new CSharpNode();

            foreach (var property in root.EnumerateObject())
            {
                switch (property.Name)
                {
                    case "$type":
                    case "typeName":
                        // Skip discriminator
                        break;
                    case "code":
                        node.Code = property.Value.GetString() ?? string.Empty;
                        break;
                    case "usings":
                        node.Usings = JsonSerializer.Deserialize<List<string>>(property.Value.GetRawText()) ?? new List<string>();
                        break;
                    case "scriptPath":
                        node.ScriptPath = property.Value.GetString() ?? string.Empty;
                        break;
                    case "references":
                        node.References = JsonSerializer.Deserialize<List<string>>(property.Value.GetRawText()) ?? new List<string>();
                        break;
                    case "imports":
                        node.Imports = JsonSerializer.Deserialize<List<string>>(property.Value.GetRawText()) ?? new List<string>();
                        break;
                    case "timeoutMs":
                        node.TimeoutMs = property.Value.GetInt32();
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
        public override void Write(Utf8JsonWriter writer, CSharpNode value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("$type", value.TypeName);
            writer.WriteString("code", value.Code);
            writer.WritePropertyName("usings");
            JsonSerializer.Serialize(writer, value.Usings, options);
            writer.WriteString("scriptPath", value.ScriptPath);
            writer.WritePropertyName("references");
            JsonSerializer.Serialize(writer, value.References, options);
            writer.WritePropertyName("imports");
            JsonSerializer.Serialize(writer, value.Imports, options);
            writer.WriteNumber("timeoutMs", value.TimeoutMs);
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
