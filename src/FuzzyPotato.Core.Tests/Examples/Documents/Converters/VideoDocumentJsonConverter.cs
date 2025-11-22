// -----------------------------------------------------------------------
// <copyright file="VideoDocumentJsonConverter.cs" company="FuzzyPotato">
//     Copyright (c) FuzzyPotato. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Tests.Examples.Documents.Converters
{
    using System;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using FuzzyPotato.Core.Tests.Examples.Documents;

    /// <summary>
    /// JSON converter for VideoDocument.
    /// </summary>
    public class VideoDocumentJsonConverter : JsonConverter<VideoDocument>
    {
        /// <inheritdoc/>
        public override VideoDocument? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;

            var videoDoc = new VideoDocument();

            foreach (var property in root.EnumerateObject())
            {
                switch (property.Name)
                {
                    case "$type":
                    case "typeName":
                        break;
                    case "videoUrl":
                        videoDoc.VideoUrl = property.Value.GetString() ?? string.Empty;
                        break;
                    case "durationSeconds":
                        videoDoc.DurationSeconds = property.Value.GetInt32();
                        break;
                    case "resolution":
                        videoDoc.Resolution = property.Value.GetString() ?? "1080p";
                        break;
                    case "codec":
                        videoDoc.Codec = property.Value.GetString() ?? "h264";
                        break;
                    case "id":
                        videoDoc.Id = property.Value.GetString() ?? string.Empty;
                        break;
                    case "name":
                        videoDoc.Name = property.Value.GetString() ?? string.Empty;
                        break;
                }
            }

            return videoDoc;
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, VideoDocument value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("$type", value.TypeName);
            writer.WriteString("videoUrl", value.VideoUrl);
            writer.WriteNumber("durationSeconds", value.DurationSeconds);
            writer.WriteString("resolution", value.Resolution);
            writer.WriteString("codec", value.Codec);
            writer.WriteString("id", value.Id);
            writer.WriteString("name", value.Name);
            writer.WriteEndObject();
        }
    }
}
