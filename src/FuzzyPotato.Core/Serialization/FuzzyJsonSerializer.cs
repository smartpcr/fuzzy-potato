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
    /// JSON serializer with TypeRegistry-based polymorphic support.
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
                    new PolymorphicJsonConverterFactory(), // Handle TypeRegistry-registered types automatically
                },
            };

            configure?.Invoke(this._options);
        }

        /// <summary>
        /// Serializes an object to JSON string.
        /// Polymorphic types registered with TypeRegistry are automatically handled.
        /// </summary>
        /// <typeparam name="T">The type of object to serialize.</typeparam>
        /// <param name="value">The object to serialize.</param>
        /// <returns>JSON string representation.</returns>
        public string Serialize<T>(T value)
        {
            return JsonSerializer.Serialize(value, this._options);
        }

        /// <summary>
        /// Serializes a collection of objects to JSON string.
        /// Polymorphic types registered with TypeRegistry are automatically handled.
        /// </summary>
        /// <typeparam name="T">The base type of collection items.</typeparam>
        /// <param name="values">The collection to serialize.</param>
        /// <returns>JSON string representation.</returns>
        public string SerializeCollection<T>(IEnumerable<T> values)
        {
            return JsonSerializer.Serialize(values, this._options);
        }

        /// <summary>
        /// Deserializes JSON string to an object.
        /// Polymorphic types registered with TypeRegistry are automatically handled.
        /// </summary>
        /// <typeparam name="T">The expected type.</typeparam>
        /// <param name="json">The JSON string.</param>
        /// <returns>Deserialized object.</returns>
        public T? Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, this._options);
        }

        /// <summary>
        /// Deserializes JSON string to a collection of objects.
        /// Polymorphic types registered with TypeRegistry are automatically handled.
        /// </summary>
        /// <typeparam name="T">The base type of collection items.</typeparam>
        /// <param name="json">The JSON string.</param>
        /// <returns>Collection of deserialized objects.</returns>
        public IEnumerable<T>? DeserializeCollection<T>(string json)
        {
            return JsonSerializer.Deserialize<IEnumerable<T>>(json, this._options);
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
        {
            var json = this.Serialize(value);
            await File.WriteAllTextAsync(filePath, json, cancellationToken);
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
        {
            var json = await File.ReadAllTextAsync(filePath, cancellationToken);
            return this.Deserialize<T>(json);
        }
    }
}
