// -----------------------------------------------------------------------
// <copyright file="TypeDiscriminatorPropertyDescriptor.cs" company="Microsoft Corp.">
//     Copyright (c) Microsoft Corp. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Serialization
{
    using YamlDotNet.Core;
    using YamlDotNet.Serialization;

    /// <summary>
    /// Property descriptor for the $type discriminator.
    /// </summary>
    internal class TypeDiscriminatorPropertyDescriptor : IPropertyDescriptor
    {
        private readonly string discriminator;

        public TypeDiscriminatorPropertyDescriptor(string discriminator)
        {
            this.discriminator = discriminator;
        }

        public string Name => "$type";

        public Type Type => typeof(string);

        public Type? TypeOverride { get; set; }

        public int Order { get; set; }

        public ScalarStyle ScalarStyle { get; set; }

        public bool CanWrite => false;

        public bool AllowNulls => false;

        public bool Required => true;

        public Type? ConverterType => null;

        public void Write(object target, object? value)
        {
            // No-op: discriminator is read-only
        }

        public T? GetCustomAttribute<T>() where T : Attribute => null;

        public IObjectDescriptor Read(object target)
        {
            return new ObjectDescriptor(this.discriminator, typeof(string), typeof(string));
        }
    }
}