// -----------------------------------------------------------------------
// <copyright file="PolymorphicYamlTypeInspector.cs" company="FuzzyPotato">
//     Copyright (c) FuzzyPotato. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Serialization
{
    using System;
    using System.Collections.Generic;
    using YamlDotNet.Serialization;
    using FuzzyPotato.Core.Models;

    /// <summary>
    /// Custom type inspector that adds $type discriminator property during serialization.
    /// </summary>
    public class PolymorphicYamlSerializingTypeInspector : YamlDotNet.Serialization.TypeInspectors.TypeInspectorSkeleton
    {
        private readonly ITypeInspector innerTypeInspector;

        /// <summary>
        /// Initializes a new instance of the <see cref="PolymorphicYamlSerializingTypeInspector"/> class.
        /// </summary>
        /// <param name="innerTypeInspector">The inner type inspector.</param>
        public PolymorphicYamlSerializingTypeInspector(ITypeInspector innerTypeInspector)
        {
            this.innerTypeInspector = innerTypeInspector;
        }

        /// <inheritdoc/>
        public override IEnumerable<IPropertyDescriptor> GetProperties(Type type, object? container)
        {
            var properties = this.innerTypeInspector.GetProperties(type, container)
                .Where(p => !string.Equals(p.Name, "TypeName", StringComparison.OrdinalIgnoreCase) &&
                            !string.Equals(p.Name, "typeName", StringComparison.Ordinal)) // Filter out TypeName property (both PascalCase and camelCase)
                .ToList();

            // Add $type discriminator property if type derives from PolymorphicBase
            if (typeof(PolymorphicBase).IsAssignableFrom(type) && !type.IsAbstract)
            {
                // Create temporary instance to get TypeName value
                var instance = Activator.CreateInstance(type) as PolymorphicBase;
                if (instance != null)
                {
                    properties.Insert(0, new TypeDiscriminatorPropertyDescriptor(instance.TypeName));
                }
            }

            return properties;
        }

        /// <inheritdoc/>
        public override string GetEnumName(Type enumType, string name)
        {
            return this.innerTypeInspector.GetEnumName(enumType, name);
        }

        /// <inheritdoc/>
        public override string GetEnumValue(object enumValue)
        {
            return this.innerTypeInspector.GetEnumValue(enumValue);
        }
    }
}
