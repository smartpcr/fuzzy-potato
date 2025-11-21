// -----------------------------------------------------------------------
// <copyright file="PolymorphicYamlTypeConverter.cs" company="FuzzyPotato">
//     Copyright (c) FuzzyPotato. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Serialization
{
    using System;
    using YamlDotNet.Core;
    using YamlDotNet.Serialization;
    using FuzzyPotato.Core.Models;

    /// <summary>
    /// YAML type converter for polymorphic types.
    /// </summary>
    public class PolymorphicYamlTypeConverter : IYamlTypeConverter
    {
        /// <inheritdoc/>
        public bool Accepts(Type type)
        {
            return typeof(PolymorphicBase).IsAssignableFrom(type);
        }

        /// <inheritdoc/>
        public object? ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
        {
            return rootDeserializer(type);
        }

        /// <inheritdoc/>
        public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
        {
            serializer(value, type);
        }
    }
}
