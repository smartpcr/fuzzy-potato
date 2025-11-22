// -----------------------------------------------------------------------
// <copyright file="ConverterRegistry.cs" company="FuzzyPotato">
//     Copyright (c) FuzzyPotato. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Tests.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json.Serialization;
    using FuzzyPotato.Core.Tests.Examples.Converters;
    using FuzzyPotato.Core.Tests.Examples.Nodes;
    using YamlDotNet.Serialization;

    /// <summary>
    /// Central registry for all JSON and YAML type converters.
    /// </summary>
    public static class ConverterRegistry
    {
        private static readonly Dictionary<string, Type> _typeNameToTypeMap = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Gets the explicit TypeName to Type mapping for polymorphic deserialization.
        /// Maps the $type discriminator value to the concrete CLR type.
        /// This map is populated by the AssemblyInitializer during test initialization.
        /// </summary>
        public static IReadOnlyDictionary<string, Type> TypeNameToTypeMap => _typeNameToTypeMap;

        /// <summary>
        /// Registers a polymorphic type with its TypeName discriminator.
        /// Called by AssemblyInitializer to build the TypeName->Type mapping.
        /// </summary>
        /// <param name="typeName">The $type discriminator value.</param>
        /// <param name="type">The concrete CLR type.</param>
        public static void RegisterType(string typeName, Type type)
        {
            _typeNameToTypeMap[typeName] = type;
        }

        /// <summary>
        /// Gets all JSON converters for polymorphic types.
        /// </summary>
        public static IReadOnlyList<JsonConverter> JsonConverters { get; } = new List<JsonConverter>
        {
            // Polymorphic base converter (must be registered first to handle all polymorphic types)
            new BaseJsonConverter(),

            // Document base converter
            new DocumentBaseJsonConverter(),

            // Node definition base converter
            new NodeDefinitionJsonConverter(),

            // Document converters
            new TextDocumentJsonConverter(),
            new ImageDocumentJsonConverter(),
            new VideoDocumentJsonConverter(),

            // Script node converters
            new CSharpScriptNodeJsonConverter(),
            new CSharpTaskNodeJsonConverter(),
            new PowerShellScriptNodeJsonConverter(),
            new PowerShellTaskNodeJsonConverter(),

            // Additional node converters
            new HttpRequestNodeJsonConverter(),
            new DelayNodeJsonConverter(),
            new CSharpNodeJsonConverter(),
            new WhileLoopNodeJsonConverter(),

            // Control flow node converters
            new IfElseNodeJsonConverter(),
            new ForEachNodeJsonConverter(),
            new WhileNodeJsonConverter(),
            new SwitchNodeJsonConverter(),
            new TimerNodeJsonConverter(),

            // Structural node converters
            new ContainerNodeJsonConverter(),
            new SubflowNodeJsonConverter(),
        };

        /// <summary>
        /// Gets all YAML type converters for polymorphic types.
        /// </summary>
        public static IReadOnlyList<IYamlTypeConverter> YamlConverters { get; } = new List<IYamlTypeConverter>
        {
            // Polymorphic base converter (handles all PolymorphicBase-derived types including DocumentBase and NodeDefinition)
            new BaseYamlConverter(),

            // Document converters
            new TextDocumentYamlConverter(),
            new ImageDocumentYamlConverter(),
            new VideoDocumentYamlConverter(),

            // Script node converters
            new CSharpScriptNodeYamlConverter(),
            new CSharpTaskNodeYamlConverter(),
            new PowerShellScriptNodeYamlConverter(),
            new PowerShellTaskNodeYamlConverter(),

            // Additional node converters
            new HttpRequestNodeYamlConverter(),
            new DelayNodeYamlConverter(),
            new CSharpNodeYamlConverter(),
            new WhileLoopNodeYamlConverter(),

            // Control flow node converters
            new IfElseNodeYamlConverter(),
            new ForEachNodeYamlConverter(),
            new WhileNodeYamlConverter(),
            new SwitchNodeYamlConverter(),
            new TimerNodeYamlConverter(),

            // Structural node converters
            new ContainerNodeYamlConverter(),
            new SubflowNodeYamlConverter(),
        };
    }
}
