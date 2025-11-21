// -----------------------------------------------------------------------
// <copyright file="PolymorphicYamlTypeInspector.cs" company="FuzzyPotato">
//     Copyright (c) FuzzyPotato. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Serialization
{
    using System;
    using System.Collections.Generic;
    using YamlDotNet.Core;
    using YamlDotNet.Core.Events;
    using YamlDotNet.Serialization;
    using FuzzyPotato.Core.Models;

    /// <summary>
    /// Custom YamlDotNet node deserializer for polymorphic types using TypeRegistry.
    /// Handles deserialization of abstract/interface types by reading $type discriminator.
    /// </summary>
    public class PolymorphicYamlNodeDeserializer : INodeDeserializer
    {
        private readonly INodeDeserializer _originalDeserializer;
        private const string TypeDiscriminatorPropertyName = "$type";

        /// <summary>
        /// Initializes a new instance of the <see cref="PolymorphicYamlNodeDeserializer"/> class.
        /// </summary>
        /// <param name="originalDeserializer">The original deserializer to delegate to.</param>
        public PolymorphicYamlNodeDeserializer(INodeDeserializer originalDeserializer)
        {
            this._originalDeserializer = originalDeserializer;
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
            if (!ShouldHandle(expectedType))
            {
                // Delegate to the original deserializer for types we don't handle
                return this._originalDeserializer.Deserialize(reader, expectedType, nestedObjectDeserializer, out value, rootDeserializer);
            }

            // Peek to see if we have a mapping
            if (!reader.Accept<MappingStart>(out _))
            {
                // Not a mapping, delegate to original deserializer
                return this._originalDeserializer.Deserialize(reader, expectedType, nestedObjectDeserializer, out value, rootDeserializer);
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
                else if (depth == 1 && evt is Scalar scalar && scalar.Value == TypeDiscriminatorPropertyName)
                {
                    // Next event should be the discriminator value - consume it but don't buffer either
                    var discriminatorEvent = reader.Consume<Scalar>();

                    actualType = TypeRegistry.GetType(discriminatorEvent.Value);
                    if (actualType == null)
                    {
                        throw new YamlException(
                            scalar.Start,
                            scalar.End,
                            $"Type '{discriminatorEvent.Value}' not registered in TypeRegistry. Call TypeRegistry.Register<T>(\"{discriminatorEvent.Value}\") first.");
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
                    return this._originalDeserializer.Deserialize(replayParser, expectedType, nestedObjectDeserializer, out value, rootDeserializer);
                }

                throw new YamlException($"Missing '{TypeDiscriminatorPropertyName}' property for abstract/interface type '{expectedType.Name}'");
            }

            // Replay the buffered events and deserialize to the actual type
            var parser = new EventReader(new ParsingEventCollection(events));
            return this._originalDeserializer.Deserialize(parser, actualType, nestedObjectDeserializer, out value, rootDeserializer);
        }

        private static bool ShouldHandle(Type expectedType)
        {
            // Handle abstract/interface types
            if (expectedType.IsAbstract || expectedType.IsInterface)
            {
                return true;
            }

            // Handle if any registered type derives from this type
            foreach (var kvp in TypeRegistry.GetAllTypes())
            {
                if (expectedType.IsAssignableFrom(kvp.Value))
                {
                    return true;
                }
            }

            return false;
        }
    }

    /// <summary>
    /// Collection of parsing events that can be enumerated.
    /// </summary>
    internal class ParsingEventCollection : IEnumerable<ParsingEvent>
    {
        private readonly List<ParsingEvent> _events;

        public ParsingEventCollection(List<ParsingEvent> events)
        {
            this._events = events;
        }

        public IEnumerator<ParsingEvent> GetEnumerator() => this._events.GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }

    /// <summary>
    /// Event reader that replays buffered parsing events.
    /// </summary>
    internal class EventReader : IParser
    {
        private readonly IEnumerator<ParsingEvent> _enumerator;
        private ParsingEvent? _current;

        public EventReader(IEnumerable<ParsingEvent> events)
        {
            this._enumerator = events.GetEnumerator();
        }

        public ParsingEvent? Current => this._current;

        public bool MoveNext()
        {
            var result = this._enumerator.MoveNext();
            this._current = result ? this._enumerator.Current : null;
            return result;
        }
    }

    /// <summary>
    /// Custom type inspector that adds $type discriminator property during serialization.
    /// </summary>
    public class PolymorphicYamlSerializingTypeInspector : YamlDotNet.Serialization.TypeInspectors.TypeInspectorSkeleton
    {
        private readonly ITypeInspector _innerTypeInspector;

        /// <summary>
        /// Initializes a new instance of the <see cref="PolymorphicYamlSerializingTypeInspector"/> class.
        /// </summary>
        /// <param name="innerTypeInspector">The inner type inspector.</param>
        public PolymorphicYamlSerializingTypeInspector(ITypeInspector innerTypeInspector)
        {
            this._innerTypeInspector = innerTypeInspector;
        }

        /// <inheritdoc/>
        public override IEnumerable<IPropertyDescriptor> GetProperties(Type type, object? container)
        {
            var properties = this._innerTypeInspector.GetProperties(type, container).ToList();

            // Add $type discriminator property if type is registered in TypeRegistry
            var discriminator = TypeRegistry.GetDiscriminator(type);
            if (!string.IsNullOrEmpty(discriminator))
            {
                properties.Insert(0, new TypeDiscriminatorPropertyDescriptor(discriminator));
            }

            return properties;
        }

        /// <inheritdoc/>
        public override string GetEnumName(Type enumType, string name)
        {
            return this._innerTypeInspector.GetEnumName(enumType, name);
        }

        /// <inheritdoc/>
        public override string GetEnumValue(object enumValue)
        {
            return this._innerTypeInspector.GetEnumValue(enumValue);
        }
    }

    /// <summary>
    /// Property descriptor for the $type discriminator.
    /// </summary>
    internal class TypeDiscriminatorPropertyDescriptor : IPropertyDescriptor
    {
        private readonly string _discriminator;

        public TypeDiscriminatorPropertyDescriptor(string discriminator)
        {
            this._discriminator = discriminator;
        }

        public string Name => "$type";

        public Type Type => typeof(string);

        public Type? TypeOverride { get; set; }

        public int Order { get; set; }

        public ScalarStyle ScalarStyle { get; set; }

        public bool CanWrite => false;

        public bool AllowNulls => false;

        public bool Required => true;

        public Type? ConverterType => null;

        public void Write(object target, object? value)
        {
            // No-op: discriminator is read-only
        }

        public T? GetCustomAttribute<T>() where T : Attribute => null;

        public IObjectDescriptor Read(object target)
        {
            return new ObjectDescriptor(this._discriminator, typeof(string), typeof(string));
        }
    }
}
