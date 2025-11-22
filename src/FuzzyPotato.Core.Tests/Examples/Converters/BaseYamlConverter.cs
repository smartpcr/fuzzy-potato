// -----------------------------------------------------------------------
// <copyright file="BaseYamlConverter.cs" company="FuzzyPotato">
//     Copyright (c) FuzzyPotato. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Tests.Examples.Converters
{
    using System;
    using System.Collections.Generic;
    using FuzzyPotato.Core.Models;
    using FuzzyPotato.Core.Tests.Serialization;
    using YamlDotNet.Core;
    using YamlDotNet.Core.Events;
    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.NamingConventions;

    /// <summary>
    /// YAML type converter for PolymorphicBase polymorphic deserialization.
    /// Maps $type discriminator to concrete types using ConverterRegistry.TypeNameToTypeMap.
    /// </summary>
    public class BaseYamlConverter : IYamlTypeConverter
    {

        /// <inheritdoc/>
        public bool Accepts(Type type)
        {
            // Only accept abstract base types, not concrete types (they have their own converters)
            return typeof(ModelBase).IsAssignableFrom(type) && type.IsAbstract;
        }

        /// <inheritdoc/>
        public object? ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
        {
            if (parser.Current is not MappingStart)
            {
                throw new YamlException("Expected mapping start for PolymorphicBase");
            }

            // Buffer all events for this mapping
            var events = new List<ParsingEvent>();
            var depth = 0;
            string? typeName = null;

            do
            {
                var evt = parser.Consume<ParsingEvent>();

                if (evt is MappingStart)
                {
                    depth++;
                    events.Add(evt);
                }
                else if (evt is MappingEnd)
                {
                    depth--;
                    events.Add(evt);
                }
                else if (depth == 1 && evt is Scalar scalar && scalar.Value == "$type")
                {
                    // Found discriminator - get its value but don't add to events
                    var valueEvent = parser.Consume<Scalar>();
                    typeName = valueEvent.Value;
                    // Skip both $type key and value from buffered events
                }
                else
                {
                    events.Add(evt);
                }
            }
            while (depth > 0);

            if (string.IsNullOrEmpty(typeName) || !ConverterRegistry.TypeNameToTypeMap.TryGetValue(typeName, out var concreteType))
            {
                throw new YamlException($"Unknown or missing type discriminator: {typeName}");
            }

            // Replay buffered events to deserialize the concrete type
            var eventReader = new FuzzyPotato.Core.Serialization.EventReader(
                new FuzzyPotato.Core.Serialization.ParsingEventCollection(events));

            // Use a new deserializer with all converters registered to handle nested polymorphic collections
            var tempDeserializerBuilder = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance);

            foreach (var converter in ConverterRegistry.YamlConverters)
            {
                tempDeserializerBuilder.WithTypeConverter(converter);
            }

            var tempDeserializer = tempDeserializerBuilder.Build();

            return tempDeserializer.Deserialize(eventReader, concreteType);
        }

        /// <inheritdoc/>
        public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
        {
            if (value is not ModelBase polymorphicValue)
            {
                emitter.Emit(new Scalar(string.Empty));
                return;
            }

            // Delegate to the concrete type's serializer
            serializer(value, value.GetType());
        }
    }
}
