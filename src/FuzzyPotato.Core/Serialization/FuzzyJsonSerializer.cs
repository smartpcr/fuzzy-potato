// -----------------------------------------------------------------------
// <copyright file="FuzzyJsonSerializer.cs" company="FuzzyPotato">
//     Copyright (c) FuzzyPotato. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Serialization
{
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using FuzzyPotato.Core.Models;

    /// <summary>
    /// JSON serializer with polymorphic support.
    /// </summary>
    public class FuzzyJsonSerializer
    {
        private readonly JsonSerializerOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="FuzzyJsonSerializer"/> class.
        /// </summary>
        /// <param name="configure">Optional action to configure JSON options.</param>
        public FuzzyJsonSerializer(Action<JsonSerializerOptions>? configure = null)
        {
            this._options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
                },
            };

            configure?.Invoke(this._options);
        }

        /// <summary>
        /// Serializes an object to JSON string.
        /// </summary>
        /// <typeparam name="T">The type of object to serialize.</typeparam>
        /// <param name="value">The object to serialize.</param>
        /// <returns>JSON string representation.</returns>
        public string Serialize<T>(T value) where T : PolymorphicBase
        {
            return JsonSerializer.Serialize<PolymorphicBase>(value, this._options);
        }

        /// <summary>
        /// Serializes a collection of polymorphic objects to JSON string.
        /// </summary>
        /// <param name="values">The collection to serialize.</param>
        /// <returns>JSON string representation.</returns>
        public string SerializeCollection(IEnumerable<PolymorphicBase> values)
        {
            return JsonSerializer.Serialize(values, this._options);
        }

        /// <summary>
        /// Deserializes JSON string to an object.
        /// </summary>
        /// <typeparam name="T">The expected type.</typeparam>
        /// <param name="json">The JSON string.</param>
        /// <returns>Deserialized object.</returns>
        public T? Deserialize<T>(string json) where T : PolymorphicBase
        {
            return JsonSerializer.Deserialize<T>(json, this._options);
        }

        /// <summary>
        /// Deserializes JSON string to a polymorphic base object.
        /// </summary>
        /// <param name="json">The JSON string.</param>
        /// <returns>Deserialized object.</returns>
        public PolymorphicBase? DeserializePolymorphic(string json)
        {
            return JsonSerializer.Deserialize<PolymorphicBase>(json, this._options);
        }

        /// <summary>
        /// Deserializes JSON string to a collection of polymorphic objects.
        /// </summary>
        /// <param name="json">The JSON string.</param>
        /// <returns>Collection of deserialized objects.</returns>
        public IEnumerable<PolymorphicBase>? DeserializeCollection(string json)
        {
            return JsonSerializer.Deserialize<IEnumerable<PolymorphicBase>>(json, this._options);
        }

        /// <summary>
        /// Serializes an object to a JSON file.
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
            await using var stream = File.Create(filePath);
            await JsonSerializer.SerializeAsync<PolymorphicBase>(stream, value, this._options, cancellationToken);
        }

        /// <summary>
        /// Deserializes a JSON file to an object.
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
            await using var stream = File.OpenRead(filePath);
            return await JsonSerializer.DeserializeAsync<T>(stream, this._options, cancellationToken);
        }

        // General object serialization methods (without PolymorphicBase constraint)

        /// <summary>
        /// Serializes any object to JSON string.
        /// </summary>
        /// <typeparam name="T">The type of object to serialize.</typeparam>
        /// <param name="value">The object to serialize.</param>
        /// <returns>JSON string representation.</returns>
        public string SerializeObject<T>(T value)
        {
            return JsonSerializer.Serialize(value, this._options);
        }

        /// <summary>
        /// Deserializes JSON string to any object type.
        /// </summary>
        /// <typeparam name="T">The expected type.</typeparam>
        /// <param name="json">The JSON string.</param>
        /// <returns>Deserialized object.</returns>
        public T? DeserializeObject<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, this._options);
        }

        /// <summary>
        /// Serializes any object to a JSON file.
        /// </summary>
        /// <typeparam name="T">The type of object to serialize.</typeparam>
        /// <param name="filePath">The file path.</param>
        /// <param name="value">The object to serialize.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task SerializeObjectToFileAsync<T>(
            string filePath,
            T value,
            CancellationToken cancellationToken = default)
        {
            await using var stream = File.Create(filePath);
            await JsonSerializer.SerializeAsync(stream, value, this._options, cancellationToken);
        }

        /// <summary>
        /// Deserializes a JSON file to any object type.
        /// </summary>
        /// <typeparam name="T">The expected type.</typeparam>
        /// <param name="filePath">The file path.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Deserialized object.</returns>
        public async Task<T?> DeserializeObjectFromFileAsync<T>(
            string filePath,
            CancellationToken cancellationToken = default)
        {
            await using var stream = File.OpenRead(filePath);
            return await JsonSerializer.DeserializeAsync<T>(stream, this._options, cancellationToken);
        }
    }
}
