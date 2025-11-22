// -----------------------------------------------------------------------
// <copyright file="PolymorphicJsonConverterFactory.cs" company="Microsoft Corp.">
//     Copyright (c) Microsoft Corp. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Serialization
{
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using FuzzyPotato.Core.Models;

    /// <summary>
    /// JSON converter factory that creates converters for PolymorphicBase types.
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

            // Handle types that derive from PolymorphicBase
            return typeof(PolymorphicBase).IsAssignableFrom(typeToConvert);
        }

        /// <inheritdoc/>
        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var converterType = typeof(PolymorphicJsonConverter<>).MakeGenericType(typeToConvert);
            return (JsonConverter?)Activator.CreateInstance(converterType);
        }
    }
}