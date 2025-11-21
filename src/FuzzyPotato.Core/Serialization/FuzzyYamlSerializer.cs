// -----------------------------------------------------------------------
// <copyright file="FuzzyYamlSerializer.cs" company="FuzzyPotato">
//     Copyright (c) FuzzyPotato. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Serialization
{
    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.NamingConventions;
    using FuzzyPotato.Core.Models;

    /// <summary>
    /// YAML serializer with polymorphic support.
    /// </summary>
    public class FuzzyYamlSerializer
    {
        private readonly ISerializer _serializer;
        private readonly IDeserializer _deserializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FuzzyYamlSerializer"/> class.
        /// </summary>
        /// <param name="configureSerializer">Optional action to configure the serializer.</param>
        /// <param name="configureDeserializer">Optional action to configure the deserializer.</param>
        public FuzzyYamlSerializer(
            Action<SerializerBuilder>? configureSerializer = null,
            Action<DeserializerBuilder>? configureDeserializer = null)
        {
            var serializerBuilder = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .WithTypeConverter(new PolymorphicYamlTypeConverter());

            configureSerializer?.Invoke(serializerBuilder);
            this._serializer = serializerBuilder.Build();

            var deserializerBuilder = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .WithTypeConverter(new PolymorphicYamlTypeConverter());

            configureDeserializer?.Invoke(deserializerBuilder);
            this._deserializer = deserializerBuilder.Build();
        }

        /// <summary>
        /// Serializes an object to YAML string.
        /// </summary>
        /// <typeparam name="T">The type of object to serialize.</typeparam>
        /// <param name="value">The object to serialize.</param>
        /// <returns>YAML string representation.</returns>
        public string Serialize<T>(T value) where T : PolymorphicBase
        {
            var wrapper = new PolymorphicWrapper
            {
                Type = TypeRegistry.GetDiscriminator(value.GetType()) ?? value.GetType().Name,
                Data = value,
            };
            return this._serializer.Serialize(wrapper);
        }

        /// <summary>
        /// Serializes a collection of polymorphic objects to YAML string.
        /// </summary>
        /// <param name="values">The collection to serialize.</param>
        /// <returns>YAML string representation.</returns>
        public string SerializeCollection(IEnumerable<PolymorphicBase> values)
        {
            var wrappers = values.Select(v => new PolymorphicWrapper
            {
                Type = TypeRegistry.GetDiscriminator(v.GetType()) ?? v.GetType().Name,
                Data = v,
            }).ToList();

            return this._serializer.Serialize(wrappers);
        }

        /// <summary>
        /// Deserializes YAML string to an object.
        /// </summary>
        /// <typeparam name="T">The expected type.</typeparam>
        /// <param name="yaml">The YAML string.</param>
        /// <returns>Deserialized object.</returns>
        public T? Deserialize<T>(string yaml) where T : PolymorphicBase
        {
            var wrapper = this._deserializer.Deserialize<PolymorphicWrapper>(yaml);
            return wrapper?.Data as T;
        }

        /// <summary>
        /// Deserializes YAML string to a polymorphic base object.
        /// </summary>
        /// <param name="yaml">The YAML string.</param>
        /// <returns>Deserialized object.</returns>
        public PolymorphicBase? DeserializePolymorphic(string yaml)
        {
            var wrapper = this._deserializer.Deserialize<PolymorphicWrapper>(yaml);
            return wrapper?.Data;
        }

        /// <summary>
        /// Deserializes YAML string to a collection of polymorphic objects.
        /// </summary>
        /// <param name="yaml">The YAML string.</param>
        /// <returns>Collection of deserialized objects.</returns>
        public IEnumerable<PolymorphicBase>? DeserializeCollection(string yaml)
        {
            var wrappers = this._deserializer.Deserialize<List<PolymorphicWrapper>>(yaml);
            return wrappers?.Select(w => w.Data).ToList();
        }

        /// <summary>
        /// Serializes an object to a YAML file.
        /// </summary>
        /// <typeparam name="T">The type of object to serialize.</typeparam>
        /// <param name="filePath">The file path.</param>
        /// <param name="value">The object to serialize.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task SerializeToFileAsync<T>(
            string filePath,
            T value,
            CancellationToken cancellationToken = default)
            where T : PolymorphicBase
        {
            var yaml = this.Serialize(value);
            await File.WriteAllTextAsync(filePath, yaml, cancellationToken);
        }

        /// <summary>
        /// Deserializes a YAML file to an object.
        /// </summary>
        /// <typeparam name="T">The expected type.</typeparam>
        /// <param name="filePath">The file path.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Deserialized object.</returns>
        public async Task<T?> DeserializeFromFileAsync<T>(
            string filePath,
            CancellationToken cancellationToken = default)
            where T : PolymorphicBase
        {
            var yaml = await File.ReadAllTextAsync(filePath, cancellationToken);
            return this.Deserialize<T>(yaml);
        }

        private class PolymorphicWrapper
        {
            public string Type { get; set; } = string.Empty;

            public PolymorphicBase Data { get; set; } = null!;
        }
    }
}
