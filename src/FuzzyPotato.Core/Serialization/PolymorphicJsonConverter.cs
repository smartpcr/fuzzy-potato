// -----------------------------------------------------------------------
// <copyright file="PolymorphicJsonConverter.cs" company="FuzzyPotato">
//     Copyright (c) FuzzyPotato. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Serialization
{
    using System;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using FuzzyPotato.Core.Models;

    /// <summary>
    /// JSON converter factory that creates converters for types registered in TypeRegistry.
    /// </summary>
    public class PolymorphicJsonConverterFactory : JsonConverterFactory
    {
        /// <inheritdoc/>
        public override bool CanConvert(Type typeToConvert)
        {
            // Don't handle System.Object - too broad and can be primitives in Dictionary<string, object>
            if (typeToConvert == typeof(object))
            {
                return false;
            }

            // Check if this type or any of its registered derived types are in TypeRegistry
            if (TypeRegistry.GetDiscriminator(typeToConvert) != null)
            {
                return true;
            }

            // Check if any registered type derives from this type
            foreach (var kvp in TypeRegistry.GetAllTypes())
            {
                if (typeToConvert.IsAssignableFrom(kvp.Value))
                {
                    return true;
                }
            }

            return false;
        }

        /// <inheritdoc/>
        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var converterType = typeof(PolymorphicJsonConverter<>).MakeGenericType(typeToConvert);
            return (JsonConverter?)Activator.CreateInstance(converterType);
        }
    }

    /// <summary>
    /// Custom JSON converter for polymorphic types using TypeRegistry.
    /// </summary>
    /// <typeparam name="TBase">The base type for polymorphic serialization.</typeparam>
    internal class PolymorphicJsonConverter<TBase> : JsonConverter<TBase>
    {
        private const string TypeDiscriminatorPropertyName = "$type";

        /// <inheritdoc/>
        public override TBase? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return default;
            }

            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException("Expected start of object");
            }

            using var jsonDoc = JsonDocument.ParseValue(ref reader);
            var root = jsonDoc.RootElement;

            // Try to get the type discriminator
            if (!root.TryGetProperty(TypeDiscriminatorPropertyName, out var typeProperty))
            {
                // No discriminator - try default deserialization if TBase is concrete
                if (!typeToConvert.IsAbstract && !typeToConvert.IsInterface)
                {
                    var json = root.GetRawText();
                    var tempOptions = new JsonSerializerOptions(options);
                    tempOptions.Converters.Clear(); // Avoid recursion
                    return JsonSerializer.Deserialize<TBase>(json, tempOptions);
                }

                throw new JsonException($"Missing '{TypeDiscriminatorPropertyName}' property for polymorphic type '{typeToConvert.Name}'");
            }

            var discriminator = typeProperty.GetString();
            if (string.IsNullOrEmpty(discriminator))
            {
                throw new JsonException($"Invalid '{TypeDiscriminatorPropertyName}' value");
            }

            // Get the actual type from TypeRegistry
            var actualType = TypeRegistry.GetType(discriminator);
            if (actualType == null)
            {
                throw new JsonException($"Type '{discriminator}' not registered in TypeRegistry. Call TypeRegistry.Register<T>(\"{discriminator}\") first.");
            }

            // Deserialize to the actual type (without this converter to avoid recursion)
            var rawJson = root.GetRawText();
            var deserializeOptions = new JsonSerializerOptions(options);
            deserializeOptions.Converters.Clear();
            return (TBase?)JsonSerializer.Deserialize(rawJson, actualType, deserializeOptions);
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, TBase value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }

            var actualType = value.GetType();
            var discriminator = TypeRegistry.GetDiscriminator(actualType);

            if (string.IsNullOrEmpty(discriminator))
            {
                // Type not registered - serialize normally
                var tempOptions = new JsonSerializerOptions(options);
                tempOptions.Converters.Clear();
                JsonSerializer.Serialize(writer, value, actualType, tempOptions);
                return;
            }

            writer.WriteStartObject();

            // Write the type discriminator first
            writer.WriteString(TypeDiscriminatorPropertyName, discriminator);

            // Serialize object properties (without this converter to avoid recursion)
            var serializeOptions = new JsonSerializerOptions(options);
            serializeOptions.Converters.Clear();
            var json = JsonSerializer.Serialize(value, actualType, serializeOptions);
            using var doc = JsonDocument.Parse(json);

            foreach (var property in doc.RootElement.EnumerateObject())
            {
                property.WriteTo(writer);
            }

            writer.WriteEndObject();
        }
    }
}
