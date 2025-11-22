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
    }
}
