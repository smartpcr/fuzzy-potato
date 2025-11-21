// -----------------------------------------------------------------------
// <copyright file="TypeRegistry.cs" company="FuzzyPotato">
//     Copyright (c) FuzzyPotato. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Registry for managing polymorphic type mappings.
    /// </summary>
    public static class TypeRegistry
    {
        private static readonly Dictionary<string, Type> TypeMap = new(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<Type, string> ReverseTypeMap = new();

        /// <summary>
        /// Registers a type with a discriminator value.
        /// </summary>
        /// <typeparam name="T">The type to register.</typeparam>
        /// <param name="discriminator">The discriminator value.</param>
        public static void Register<T>(string discriminator) where T : PolymorphicBase
        {
            var type = typeof(T);
            TypeMap[discriminator] = type;
            ReverseTypeMap[type] = discriminator;
        }

        /// <summary>
        /// Gets the type for a discriminator value.
        /// </summary>
        /// <param name="discriminator">The discriminator value.</param>
        /// <returns>The registered type, or null if not found.</returns>
        public static Type? GetType(string discriminator)
        {
            return TypeMap.TryGetValue(discriminator, out var type) ? type : null;
        }

        /// <summary>
        /// Gets the discriminator for a type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The discriminator value, or null if not found.</returns>
        public static string? GetDiscriminator(Type type)
        {
            return ReverseTypeMap.TryGetValue(type, out var discriminator) ? discriminator : null;
        }

        /// <summary>
        /// Gets all registered types.
        /// </summary>
        /// <returns>Dictionary of discriminator to type mappings.</returns>
        public static IReadOnlyDictionary<string, Type> GetAllTypes()
        {
            return TypeMap;
        }

        /// <summary>
        /// Clears all registered types.
        /// </summary>
        public static void Clear()
        {
            TypeMap.Clear();
            ReverseTypeMap.Clear();
        }
    }
}
