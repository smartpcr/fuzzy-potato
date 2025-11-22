// -----------------------------------------------------------------------
// <copyright file="PolymorphicYamlNodeDeserializer.cs" company="Microsoft Corp.">
//     Copyright (c) Microsoft Corp. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Serialization
{
    using FuzzyPotato.Core.Models;
    using YamlDotNet.Core;
    using YamlDotNet.Core.Events;
    using YamlDotNet.Serialization;

    /// <summary>
    /// Custom YamlDotNet node deserializer for polymorphic types using TypeName property.
    /// Handles deserialization of abstract/interface types by reading $type discriminator.
    /// </summary>
    public class PolymorphicYamlNodeDeserializer : INodeDeserializer
    {
        private readonly INodeDeserializer originalDeserializer;
        private const string TypeDiscriminatorPropertyName = "$type";
        private static readonly Dictionary<string, Type> TypeCache = new(StringComparer.OrdinalIgnoreCase);
        private static readonly object CacheLock = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="PolymorphicYamlNodeDeserializer"/> class.
        /// </summary>
        /// <param name="originalDeserializer">The original deserializer to delegate to.</param>
        public PolymorphicYamlNodeDeserializer(INodeDeserializer originalDeserializer)
        {
            this.originalDeserializer = originalDeserializer;
        }

        /// <inheritdoc/>
        public bool Deserialize(
            IParser reader,
            Type expectedType,
            Func<IParser, Type, object?> nestedObjectDeserializer,
            out object? value,
            ObjectDeserializer rootDeserializer)
        {
            // Only handle abstract/interface types or types with registered derived types
            if (!PolymorphicYamlNodeDeserializer.ShouldHandle(expectedType))
            {
                // Delegate to the original deserializer for types we don't handle
                return this.originalDeserializer.Deserialize(reader, expectedType, nestedObjectDeserializer, out value, rootDeserializer);
            }

            // Peek to see if we have a mapping
            if (!reader.Accept<MappingStart>(out _))
            {
                // Not a mapping, delegate to original deserializer
                return this.originalDeserializer.Deserialize(reader, expectedType, nestedObjectDeserializer, out value, rootDeserializer);
            }

            // We need to buffer the entire mapping to find the $type property
            var events = new List<ParsingEvent>();
            var depth = 0;
            Type? actualType = null;

            // Consume and buffer all events for this mapping
            do
            {
                var evt = reader.Consume<ParsingEvent>();

                if (evt is MappingStart)
                {
                    events.Add(evt);
                    depth++;
                }
                else if (evt is MappingEnd)
                {
                    events.Add(evt);
                    depth--;
                }
                else if (depth == 1 && evt is Scalar scalar && scalar.Value == PolymorphicYamlNodeDeserializer.TypeDiscriminatorPropertyName)
                {
                    // Next event should be the discriminator value - consume it but don't buffer either
                    var discriminatorEvent = reader.Consume<Scalar>();

                    actualType = PolymorphicYamlNodeDeserializer.GetTypeByTypeName(discriminatorEvent.Value);
                    if (actualType == null)
                    {
                        throw new YamlException(
                            scalar.Start,
                            scalar.End,
                            $"Type with TypeName '{discriminatorEvent.Value}' not found. Ensure the type exists and has a parameterless constructor.");
                    }
                    // Don't add $type key or value to events - skip them
                }
                else
                {
                    events.Add(evt);
                }
            }
            while (depth > 0);

            // If no type discriminator found, check if expectedType is concrete
            if (actualType == null)
            {
                if (!expectedType.IsAbstract && !expectedType.IsInterface)
                {
                    // Replay events with original deserializer for concrete types
                    var replayParser = new EventReader(new ParsingEventCollection(events));
                    return this.originalDeserializer.Deserialize(replayParser, expectedType, nestedObjectDeserializer, out value, rootDeserializer);
                }

                throw new YamlException($"Missing '{PolymorphicYamlNodeDeserializer.TypeDiscriminatorPropertyName}' property for abstract/interface type '{expectedType.Name}'");
            }

            // Replay the buffered events and deserialize to the actual type
            var parser = new EventReader(new ParsingEventCollection(events));
            return this.originalDeserializer.Deserialize(parser, actualType, nestedObjectDeserializer, out value, rootDeserializer);
        }

        private static bool ShouldHandle(Type expectedType)
        {
            // Handle abstract/interface types
            if (expectedType.IsAbstract || expectedType.IsInterface)
            {
                return true;
            }

            // Handle if type derives from PolymorphicBase
            return typeof(PolymorphicBase).IsAssignableFrom(expectedType);
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