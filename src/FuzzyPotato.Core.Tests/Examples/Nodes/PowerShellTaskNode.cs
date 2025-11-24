// -----------------------------------------------------------------------
// <copyright file="PowerShellTaskNode.cs" company="FuzzyPotato">
//     Copyright (c) FuzzyPotato. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Tests.Examples.Nodes
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;
    using FuzzyPotato.Core.Tests.Examples.Nodes.Converters;

    /// <summary>
    /// Node that executes inline PowerShell scripts with integrated helper cmdlets.
    /// Provides Get-Input, Set-Output, Get-Global, Set-Global cmdlets for workflow integration.
    /// </summary>
    [JsonConverter(typeof(PowerShellTaskNodeJsonConverter))]
    public class PowerShellTaskNode : NodeDefinition
    {
        /// <inheritdoc/>
        public override string TypeName => "powershell-task-node";

        /// <summary>
        /// Gets or sets the inline PowerShell script content.
        /// If null, loads from ScriptPath.
        /// </summary>
        public string? ScriptContent { get; set; }

        /// <summary>
        /// Gets or sets the path to the PowerShell script file (used if ScriptContent is null).
        /// </summary>
        public string? ScriptPath { get; set; }

        /// <summary>
        /// Gets or sets the list of PowerShell modules to import.
        /// </summary>
        public List<string> RequiredModules { get; set; } = new();

        /// <summary>
        /// Gets or sets custom module paths.
        /// </summary>
        public Dictionary<string, string> ModulePaths { get; set; } = new();

        /// <summary>
        /// Gets or sets the execution policy.
        /// </summary>
        public string ExecutionPolicy { get; set; } = "RemoteSigned";

        /// <summary>
        /// Gets or sets the parameters to pass to the script.
        /// </summary>
        public Dictionary<string, object> Parameters { get; set; } = new();

        /// <summary>
        /// Gets or sets a value indicating whether to capture verbose output.
        /// </summary>
        public bool CaptureVerbose { get; set; } = true;

        /// <summary>
        /// Gets or sets the timeout in milliseconds.
        /// </summary>
        public int TimeoutMs { get; set; } = 300000;
    }
}
