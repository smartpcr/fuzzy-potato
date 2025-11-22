// -----------------------------------------------------------------------
// <copyright file="FuzzyYamlSerializer.cs" company="FuzzyPotato">
//     Copyright (c) FuzzyPotato. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Serialization
{
    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.NamingConventions;

    /// <summary>
    /// YAML serializer with polymorphic support via TypeRegistry.
    /// </summary>
    public class FuzzyYamlSerializer
    {
        private readonly ISerializer serializer;
        private readonly IDeserializer deserializer;

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
                .WithTypeInspector(inner => new PolymorphicYamlSerializingTypeInspector(inner));

            configureSerializer?.Invoke(serializerBuilder);
            this.serializer = serializerBuilder.Build();

            var deserializerBuilder = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .WithNodeDeserializer(
                    inner => new PolymorphicYamlNodeDeserializer(inner),
                    syntax => syntax.InsteadOf<YamlDotNet.Serialization.NodeDeserializers.ObjectNodeDeserializer>());

            configureDeserializer?.Invoke(deserializerBuilder);
            this.deserializer = deserializerBuilder.Build();
        }

        /// <summary>
        /// Serializes an object to YAML string.
        /// Polymorphic types registered with TypeRegistry automatically include $type discriminator.
        /// </summary>
        /// <typeparam name="T">The type of object to serialize.</typeparam>
        /// <param name="value">The object to serialize.</param>
        /// <returns>YAML string representation.</returns>
        public string Serialize<T>(T value)
        {
            return this.serializer.Serialize(value);
        }

        /// <summary>
        /// Serializes a collection of objects to YAML string.
        /// Polymorphic types registered with TypeRegistry automatically include $type discriminator.
        /// </summary>
        /// <typeparam name="T">The base type of collection items.</typeparam>
        /// <param name="values">The collection to serialize.</param>
        /// <returns>YAML string representation.</returns>
        public string SerializeCollection<T>(IEnumerable<T> values)
        {
            return this.serializer.Serialize(values);
        }

        /// <summary>
        /// Deserializes YAML string to an object.
        /// Polymorphic types are automatically resolved using $type discriminator and TypeRegistry.
        /// </summary>
        /// <typeparam name="T">The expected type.</typeparam>
        /// <param name="yaml">The YAML string.</param>
        /// <returns>Deserialized object.</returns>
        public T Deserialize<T>(string yaml)
        {
            return this.deserializer.Deserialize<T>(yaml);
        }

        /// <summary>
        /// Deserializes YAML string to a collection of objects.
        /// Polymorphic types are automatically resolved using $type discriminator and TypeRegistry.
        /// </summary>
        /// <typeparam name="T">The base type of collection items.</typeparam>
        /// <param name="yaml">The YAML string.</param>
        /// <returns>Collection of deserialized objects.</returns>
        public IEnumerable<T> DeserializeCollection<T>(string yaml)
        {
            return this.deserializer.Deserialize<IEnumerable<T>>(yaml);
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
        {
            var yaml = await File.ReadAllTextAsync(filePath, cancellationToken);
            return this.Deserialize<T>(yaml);
        }
    }
}
