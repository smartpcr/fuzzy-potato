// -----------------------------------------------------------------------
// <copyright file="SubflowNodeJsonConverter.cs" company="FuzzyPotato">
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
    /// JSON converter for SubflowNode.
    /// </summary>
    public class SubflowNodeJsonConverter : JsonConverter<SubflowNode>
    {
        /// <inheritdoc/>
        public override SubflowNode? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;

            var node = new SubflowNode();

            foreach (var property in root.EnumerateObject())
            {
                switch (property.Name)
                {
                    case "$type":
                    case "typeName":
                        // Skip discriminator
                        break;
                    case "workflowFilePath":
                        node.WorkflowFilePath = property.Value.GetString();
                        break;
                    case "childWorkflowDefinition":
                        node.ChildWorkflowDefinition = JsonSerializer.Deserialize<WorkflowDefinition>(property.Value.GetRawText(), options);
                        break;
                    case "inputMappings":
                        node.InputMappings = JsonSerializer.Deserialize<Dictionary<string, string>>(property.Value.GetRawText()) ?? new Dictionary<string, string>();
                        break;
                    case "outputMappings":
                        node.OutputMappings = JsonSerializer.Deserialize<Dictionary<string, string>>(property.Value.GetRawText()) ?? new Dictionary<string, string>();
                        break;
                    case "timeoutMs":
                        node.TimeoutMs = property.Value.GetInt32();
                        break;
                    case "isolateContext":
                        node.IsolateContext = property.Value.GetBoolean();
                        break;
                    case "propagateCancellation":
                        node.PropagateCancellation = property.Value.GetBoolean();
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
        public override void Write(Utf8JsonWriter writer, SubflowNode value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("$type", value.TypeName);
            if (value.WorkflowFilePath != null)
            {
                writer.WriteString("workflowFilePath", value.WorkflowFilePath);
            }

            if (value.ChildWorkflowDefinition != null)
            {
                writer.WritePropertyName("childWorkflowDefinition");
                JsonSerializer.Serialize(writer, value.ChildWorkflowDefinition, options);
            }

            writer.WritePropertyName("inputMappings");
            JsonSerializer.Serialize(writer, value.InputMappings, options);
            writer.WritePropertyName("outputMappings");
            JsonSerializer.Serialize(writer, value.OutputMappings, options);
            writer.WriteNumber("timeoutMs", value.TimeoutMs);
            writer.WriteBoolean("isolateContext", value.IsolateContext);
            writer.WriteBoolean("propagateCancellation", value.PropagateCancellation);
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
