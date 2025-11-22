// -----------------------------------------------------------------------
// <copyright file="BaseJsonConverter.cs" company="FuzzyPotato">
//     Copyright (c) FuzzyPotato. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using FuzzyPotato.Core.Models;

    /// <summary>
    /// JSON converter for ModelBase polymorphic deserialization.
    /// Maps $type discriminator to concrete types using a provided type registry.
    /// </summary>
    public class BaseJsonConverter : JsonConverter<ModelBase>
    {
        private readonly IReadOnlyDictionary<string, Type> _typeMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseJsonConverter"/> class.
        /// </summary>
        /// <param name="typeMap">The TypeName to Type mapping for polymorphic deserialization.</param>
        public BaseJsonConverter(IReadOnlyDictionary<string, Type> typeMap)
        {
            this._typeMap = typeMap ?? throw new ArgumentNullException(nameof(typeMap));
        }

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
            if (string.IsNullOrEmpty(typeName) || !this._typeMap.TryGetValue(typeName, out var concreteType))
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
