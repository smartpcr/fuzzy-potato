// -----------------------------------------------------------------------
// <copyright file="ImageDocumentJsonConverter.cs" company="FuzzyPotato">
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
    /// JSON converter for ImageDocument.
    /// </summary>
    public class ImageDocumentJsonConverter : JsonConverter<ImageDocument>
    {
        /// <inheritdoc/>
        public override ImageDocument? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;

            var imageDoc = new ImageDocument();

            foreach (var property in root.EnumerateObject())
            {
                switch (property.Name)
                {
                    case "$type":
                    case "typeName":
                        break;
                    case "imageUrl":
                        imageDoc.ImageUrl = property.Value.GetString() ?? string.Empty;
                        break;
                    case "width":
                        imageDoc.Width = property.Value.GetInt32();
                        break;
                    case "height":
                        imageDoc.Height = property.Value.GetInt32();
                        break;
                    case "format":
                        imageDoc.Format = property.Value.GetString() ?? "png";
                        break;
                    case "id":
                        imageDoc.Id = property.Value.GetString() ?? string.Empty;
                        break;
                    case "name":
                        imageDoc.Name = property.Value.GetString() ?? string.Empty;
                        break;
                }
            }

            return imageDoc;
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, ImageDocument value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("$type", value.TypeName);
            writer.WriteString("imageUrl", value.ImageUrl);
            writer.WriteNumber("width", value.Width);
            writer.WriteNumber("height", value.Height);
            writer.WriteString("format", value.Format);
            writer.WriteString("id", value.Id);
            writer.WriteString("name", value.Name);
            writer.WriteEndObject();
        }
    }
}
