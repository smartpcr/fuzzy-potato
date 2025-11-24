// -----------------------------------------------------------------------
// <copyright file="CSharpTaskNode.cs" company="Microsoft Corp.">
//     Copyright (c) Microsoft Corp. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Tests.Examples.Nodes
{
    using System.Text.Json.Serialization;
    using FuzzyPotato.Core.Tests.Examples.Nodes.Converters;

    /// <summary>
    /// Node that executes inline C# code or pre-compiled executor delegates.
    /// Supports both dynamic scripting and compiled task execution.
    /// </summary>
    [JsonConverter(typeof(CSharpTaskNodeJsonConverter))]
    public class CSharpTaskNode : NodeDefinition
    {
        /// <inheritdoc/>
        public override string TypeName => "csharp-task-node";

        /// <summary>
        /// Gets or sets the inline C# script content.
        /// If null, uses assembly-based executor delegate.
        /// </summary>
        public string? ScriptContent { get; set; }

        /// <summary>
        /// Gets or sets the path to the assembly containing the compiled task.
        /// </summary>
        public string? AssemblyPath { get; set; }

        /// <summary>
        /// Gets or sets the fully qualified type name of the task executor.
        /// </summary>
        public string? TypeName_Executor { get; set; }

        /// <summary>
        /// Gets or sets the list of assembly references for inline scripts.
        /// </summary>
        public List<string> References { get; set; } = new();

        /// <summary>
        /// Gets or sets the list of namespace imports for inline scripts.
        /// </summary>
        public List<string> Imports { get; set; } = new();

        /// <summary>
        /// Gets or sets the timeout in milliseconds.
        /// </summary>
        public int TimeoutMs { get; set; } = 30000;
    }
}