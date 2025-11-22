// -----------------------------------------------------------------------
// <copyright file="AssemblyInitializer.cs" company="FuzzyPotato">
//     Copyright (c) FuzzyPotato. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Tests.Serialization
{
    using System;
    using System.Linq;
    using FuzzyPotato.Core.Models;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Assembly-level initializer that discovers and registers all PolymorphicBase-derived types.
    /// This runs once before any tests execute, building the TypeName->Type mapping used by converters.
    /// </summary>
    [TestClass]
    public class AssemblyInitializer
    {
        /// <summary>
        /// Scans all loaded assemblies for PolymorphicBase-derived types and registers them
        /// with ConverterRegistry using their TypeName property as the discriminator.
        /// </summary>
        /// <param name="context">The test context.</param>
        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext context)
        {
            // Scan all loaded assemblies for PolymorphicBase-derived types
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                try
                {
                    var types = assembly.GetTypes()
                        .Where(t => typeof(ModelBase).IsAssignableFrom(t)
                                 && !t.IsAbstract
                                 && t != typeof(ModelBase));

                    foreach (var type in types)
                    {
                        // Create instance to get TypeName
                        var instance = Activator.CreateInstance(type) as ModelBase;
                        if (instance != null)
                        {
                            ConverterRegistry.RegisterType(instance.TypeName, type);
                        }
                    }
                }
                catch
                {
                    // Skip assemblies that can't be scanned
                }
            }
        }
    }
}
