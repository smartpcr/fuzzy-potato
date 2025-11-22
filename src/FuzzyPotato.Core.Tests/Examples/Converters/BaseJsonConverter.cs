// -----------------------------------------------------------------------
// <copyright file="BaseJsonConverter.cs" company="FuzzyPotato">
//     Copyright (c) FuzzyPotato. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Tests.Examples.Converters
{
    using System;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using FuzzyPotato.Core.Models;
    using FuzzyPotato.Core.Tests.Serialization;

    /// <summary>
    /// JSON converter for PolymorphicBase polymorphic deserialization.
    /// Maps $type discriminator to concrete types using ConverterRegistry.TypeNameToTypeMap.
    /// </summary>
    public class BaseJsonConverter : JsonConverter<ModelBase>
    {

        /// <inheritdoc/>
        public override ModelBase? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;

            // Read $type discriminator
            if (!root.TryGetProperty("$type", out var typeProperty))
            {
                throw new JsonException($"Missing '$type' discriminator for {typeToConvert.Name}");
            }

            var typeName = typeProperty.GetString();
            if (string.IsNullOrEmpty(typeName) || !ConverterRegistry.TypeNameToTypeMap.TryGetValue(typeName, out var concreteType))
            {
                throw new JsonException($"Unknown type: {typeName}");
            }

            // Deserialize to concrete type using its specific converter
            var json = root.GetRawText();
            return (ModelBase?)JsonSerializer.Deserialize(json, concreteType, options);
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, ModelBase value, JsonSerializerOptions options)
        {
            // Serialize using the concrete type's converter
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }
}
