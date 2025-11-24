// -----------------------------------------------------------------------
// <copyright file="CSharpNode.cs" company="Microsoft Corp.">
//     Copyright (c) Microsoft Corp. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Tests.Examples.Nodes
{
    using System.Text.Json.Serialization;
    using FuzzyPotato.Core.Tests.Examples.Nodes.Converters;

    /// <summary>
    /// Legacy alias for CSharpScriptNode for backwards compatibility.
    /// </summary>
    [JsonConverter(typeof(CSharpNodeJsonConverter))]
    public class CSharpNode : CSharpScriptNode
    {
        /// <inheritdoc/>
        public override string TypeName => "csharp-node";

        /// <summary>
        /// Gets or sets the C# code to execute (mapped to ScriptPath for compatibility).
        /// </summary>
        public string Code
        {
            get => this.ScriptPath;
            set => this.ScriptPath = value;
        }

        /// <summary>
        /// Gets or sets the list of using directives (mapped to Imports).
        /// </summary>
        public List<string> Usings
        {
            get => this.Imports;
            set => this.Imports = value;
        }
    }
}