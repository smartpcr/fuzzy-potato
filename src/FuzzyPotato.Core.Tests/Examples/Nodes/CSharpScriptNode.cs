// -----------------------------------------------------------------------
// <copyright file="CSharpScriptNode.cs" company="Microsoft Corp.">
//     Copyright (c) Microsoft Corp. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Tests.Examples.Nodes
{
    using System.Text.Json.Serialization;
    using FuzzyPotato.Core.Tests.Examples.Nodes.Converters;

    /// <summary>
    /// Node that executes C# scripts from a file using Roslyn scripting engine.
    /// Scripts are compiled and cached for performance.
    /// </summary>
    [JsonConverter(typeof(CSharpScriptNodeJsonConverter))]
    public class CSharpScriptNode : NodeDefinition
    {
        /// <inheritdoc/>
        public override string TypeName => "csharp-script-node";

        /// <summary>
        /// Gets or sets the path to the C# script file to execute.
        /// </summary>
        public string ScriptPath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the list of assembly references required by the script.
        /// </summary>
        public List<string> References { get; set; } = new();

        /// <summary>
        /// Gets or sets the list of namespace imports for the script.
        /// </summary>
        public List<string> Imports { get; set; } = new();

        /// <summary>
        /// Gets or sets the timeout in milliseconds for script execution.
        /// </summary>
        public int TimeoutMs { get; set; } = 30000;
    }
}