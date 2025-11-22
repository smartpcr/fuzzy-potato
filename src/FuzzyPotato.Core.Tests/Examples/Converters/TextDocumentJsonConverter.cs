// -----------------------------------------------------------------------
// <copyright file="TextDocumentJsonConverter.cs" company="FuzzyPotato">
//     Copyright (c) FuzzyPotato. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Tests.Examples.Converters
{
    using System;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using FuzzyPotato.Core.Tests.Examples;

    /// <summary>
    /// JSON converter for TextDocument.
    /// </summary>
    public class TextDocumentJsonConverter : JsonConverter<TextDocument>
    {
        /// <inheritdoc/>
        public override TextDocument? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;

            var textDoc = new TextDocument();

            foreach (var property in root.EnumerateObject())
            {
                switch (property.Name)
                {
                    case "$type":
                    case "typeName":
                        // Skip discriminator
                        break;
                    case "content":
                        textDoc.Content = property.Value.GetString() ?? string.Empty;
                        break;
                    case "wordCount":
                        textDoc.WordCount = property.Value.GetInt32();
                        break;
                    case "language":
                        textDoc.Language = property.Value.GetString() ?? "en";
                        break;
                    case "id":
                        textDoc.Id = property.Value.GetString() ?? string.Empty;
                        break;
                    case "name":
                        textDoc.Name = property.Value.GetString() ?? string.Empty;
                        break;
                    case "createdAt":
                        textDoc.CreatedAt = property.Value.GetDateTime();
                        break;
                }
            }

            return textDoc;
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, TextDocument value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("$type", value.TypeName);
            writer.WriteString("content", value.Content);
            writer.WriteNumber("wordCount", value.WordCount);
            writer.WriteString("language", value.Language);
            writer.WriteString("id", value.Id);
            writer.WriteString("name", value.Name);
            writer.WriteString("createdAt", value.CreatedAt);
            writer.WriteEndObject();
        }
    }
}
