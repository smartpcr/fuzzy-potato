// -----------------------------------------------------------------------
// <copyright file="CSharpTaskNodeJsonConverter.cs" company="FuzzyPotato">
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
    /// JSON converter for CSharpTaskNode.
    /// </summary>
    public class CSharpTaskNodeJsonConverter : JsonConverter<CSharpTaskNode>
    {
        /// <inheritdoc/>
        public override CSharpTaskNode? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;
            var node = new CSharpTaskNode();

            foreach (var property in root.EnumerateObject())
            {
                switch (property.Name)
                {
                    case "$type":
                    case "typeName":
                        break;
                    case "scriptContent":
                        node.ScriptContent = property.Value.GetString();
                        break;
                    case "assemblyPath":
                        node.AssemblyPath = property.Value.GetString();
                        break;
                    case "typeName_Executor":
                        node.TypeName_Executor = property.Value.GetString();
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
        public override void Write(Utf8JsonWriter writer, CSharpTaskNode value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("$type", value.TypeName);
            if (value.ScriptContent != null)
            {
                writer.WriteString("scriptContent", value.ScriptContent);
            }

            if (value.AssemblyPath != null)
            {
                writer.WriteString("assemblyPath", value.AssemblyPath);
            }

            if (value.TypeName_Executor != null)
            {
                writer.WriteString("typeName_Executor", value.TypeName_Executor);
            }

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
