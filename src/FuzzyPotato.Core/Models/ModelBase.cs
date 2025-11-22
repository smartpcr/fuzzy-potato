// -----------------------------------------------------------------------
// <copyright file="PolymorphicBase.cs" company="FuzzyPotato">
//     Copyright (c) FuzzyPotato. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Models
{
    /// <summary>
    /// Base class for polymorphic serialization support.
    /// Derived classes must specify their type discriminator via the TypeName property.
    /// </summary>
    public abstract class ModelBase
    {
        /// <summary>
        /// Gets the type discriminator used for polymorphic serialization.
        /// </summary>
        public abstract string TypeName { get; }

        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the creation timestamp.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
