// -----------------------------------------------------------------------
// <copyright file="PolymorphicBase.cs" company="FuzzyPotato">
//     Copyright (c) FuzzyPotato. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Models
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Base class for polymorphic serialization support.
    /// Derived classes must specify their type discriminator using JsonDerivedType attributes.
    /// </summary>
    public abstract class PolymorphicBase
    {
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
