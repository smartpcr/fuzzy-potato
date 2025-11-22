// -----------------------------------------------------------------------
// <copyright file="HttpRequestNodeJsonConverter.cs" company="FuzzyPotato">
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
    /// JSON converter for HttpRequestNode.
    /// </summary>
    public class HttpRequestNodeJsonConverter : JsonConverter<HttpRequestNode>
    {
        /// <inheritdoc/>
        public override HttpRequestNode? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;

            var node = new HttpRequestNode();

            foreach (var property in root.EnumerateObject())
            {
                switch (property.Name)
                {
                    case "$type":
                    case "typeName":
                        // Skip discriminator
                        break;
                    case "method":
                        node.Method = property.Value.GetString() ?? string.Empty;
                        break;
                    case "url":
                        node.Url = property.Value.GetString() ?? string.Empty;
                        break;
                    case "headers":
                        node.Headers = JsonSerializer.Deserialize<Dictionary<string, string>>(property.Value.GetRawText()) ?? new Dictionary<string, string>();
                        break;
                    case "body":
                        node.Body = property.Value.GetString();
                        break;
                    case "contentType":
                        node.ContentType = property.Value.GetString() ?? string.Empty;
                        break;
                    case "timeoutMs":
                        node.TimeoutMs = property.Value.GetInt32();
                        break;
                    case "validateSsl":
                        node.ValidateSsl = property.Value.GetBoolean();
                        break;
                    case "followRedirects":
                        node.FollowRedirects = property.Value.GetBoolean();
                        break;
                    case "maxRedirects":
                        node.MaxRedirects = property.Value.GetInt32();
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
        public override void Write(Utf8JsonWriter writer, HttpRequestNode value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("$type", value.TypeName);
            writer.WriteString("method", value.Method);
            writer.WriteString("url", value.Url);
            writer.WritePropertyName("headers");
            JsonSerializer.Serialize(writer, value.Headers, options);
            if (value.Body != null)
            {
                writer.WriteString("body", value.Body);
            }

            writer.WriteString("contentType", value.ContentType);
            writer.WriteNumber("timeoutMs", value.TimeoutMs);
            writer.WriteBoolean("validateSsl", value.ValidateSsl);
            writer.WriteBoolean("followRedirects", value.FollowRedirects);
            writer.WriteNumber("maxRedirects", value.MaxRedirects);
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
