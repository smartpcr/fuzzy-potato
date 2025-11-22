// -----------------------------------------------------------------------
// <copyright file="PowerShellTaskNodeJsonConverter.cs" company="FuzzyPotato">
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
    /// JSON converter for PowerShellTaskNode.
    /// </summary>
    public class PowerShellTaskNodeJsonConverter : JsonConverter<PowerShellTaskNode>
    {
        /// <inheritdoc/>
        public override PowerShellTaskNode? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;
            var node = new PowerShellTaskNode();

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
                    case "scriptPath":
                        node.ScriptPath = property.Value.GetString();
                        break;
                    case "requiredModules":
                        node.RequiredModules = JsonSerializer.Deserialize<List<string>>(property.Value.GetRawText()) ?? new List<string>();
                        break;
                    case "modulePaths":
                        node.ModulePaths = JsonSerializer.Deserialize<Dictionary<string, string>>(property.Value.GetRawText()) ?? new Dictionary<string, string>();
                        break;
                    case "executionPolicy":
                        node.ExecutionPolicy = property.Value.GetString() ?? "RemoteSigned";
                        break;
                    case "parameters":
                        node.Parameters = JsonSerializer.Deserialize<Dictionary<string, object>>(property.Value.GetRawText()) ?? new Dictionary<string, object>();
                        break;
                    case "captureVerbose":
                        node.CaptureVerbose = property.Value.GetBoolean();
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
        public override void Write(Utf8JsonWriter writer, PowerShellTaskNode value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("$type", value.TypeName);
            if (value.ScriptContent != null)
            {
                writer.WriteString("scriptContent", value.ScriptContent);
            }

            if (value.ScriptPath != null)
            {
                writer.WriteString("scriptPath", value.ScriptPath);
            }

            writer.WritePropertyName("requiredModules");
            JsonSerializer.Serialize(writer, value.RequiredModules, options);
            writer.WritePropertyName("modulePaths");
            JsonSerializer.Serialize(writer, value.ModulePaths, options);
            writer.WriteString("executionPolicy", value.ExecutionPolicy);
            writer.WritePropertyName("parameters");
            JsonSerializer.Serialize(writer, value.Parameters, options);
            writer.WriteBoolean("captureVerbose", value.CaptureVerbose);
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
