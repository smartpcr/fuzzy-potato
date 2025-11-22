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
    /// Custom JSON converter for polymorphic types using TypeName property.
    /// </summary>
    /// <typeparam name="TBase">The base type for polymorphic serialization.</typeparam>
    internal class PolymorphicJsonConverter<TBase> : JsonConverter<TBase>
    {
        private const string TypeDiscriminatorPropertyName = "$type";
        private static readonly Dictionary<string, Type> TypeCache = new(StringComparer.OrdinalIgnoreCase);
        private static readonly object CacheLock = new();

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

            // Get the actual type by TypeName
            var actualType = PolymorphicJsonConverter<TBase>.GetTypeByTypeName(discriminator);
            if (actualType == null)
            {
                throw new JsonException($"Type with TypeName '{discriminator}' not found. Ensure the type exists and has a parameterless constructor.");
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
            string? discriminator = null;

            // Get TypeName from the instance if it's a PolymorphicBase
            if (value is PolymorphicBase polymorphicInstance)
            {
                discriminator = polymorphicInstance.TypeName;
            }

            if (string.IsNullOrEmpty(discriminator))
            {
                // Type doesn't have TypeName - serialize normally
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
                // Skip TypeName property - we already wrote $type
                if (property.Name != nameof(PolymorphicBase.TypeName))
                {
                    property.WriteTo(writer);
                }
            }

            writer.WriteEndObject();
        }

        /// <summary>
        /// Gets a type by its TypeName discriminator.
        /// </summary>
        /// <param name="typeName">The TypeName discriminator.</param>
        /// <returns>The type, or null if not found.</returns>
        private static Type? GetTypeByTypeName(string typeName)
        {
            lock (CacheLock)
            {
                // Check cache first
                if (TypeCache.TryGetValue(typeName, out var cachedType))
                {
                    return cachedType;
                }

                // Scan loaded assemblies for PolymorphicBase types
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        if (typeof(PolymorphicBase).IsAssignableFrom(type) &&
                            !type.IsAbstract &&
                            type.GetConstructor(Type.EmptyTypes) != null)
                        {
                            try
                            {
                                // Create instance to get TypeName
                                var instance = Activator.CreateInstance(type) as PolymorphicBase;
                                if (instance != null && !TypeCache.ContainsKey(instance.TypeName))
                                {
                                    TypeCache[instance.TypeName] = type;
                                }
                            }
                            catch
                            {
                                // Skip types that can't be instantiated
                            }
                        }
                    }
                }

                // Try again after scanning
                return TypeCache.GetValueOrDefault(typeName);
            }
        }
    }
}
