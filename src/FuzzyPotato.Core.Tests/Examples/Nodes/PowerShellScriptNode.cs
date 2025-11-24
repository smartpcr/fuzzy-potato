// -----------------------------------------------------------------------
// <copyright file="PowerShellScriptNode.cs" company="Microsoft Corp.">
//     Copyright (c) Microsoft Corp. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Tests.Examples.Nodes
{
    using System.Text.Json.Serialization;
    using FuzzyPotato.Core.Tests.Examples.Nodes.Converters;

    /// <summary>
    /// Node that executes PowerShell scripts from a file.
    /// Supports module loading and captures all output streams.
    /// </summary>
    [JsonConverter(typeof(PowerShellScriptNodeJsonConverter))]
    public class PowerShellScriptNode : NodeDefinition
    {
        /// <inheritdoc/>
        public override string TypeName => "powershell-script-node";

        /// <summary>
        /// Gets or sets the path to the PowerShell script file.
        /// </summary>
        public string ScriptPath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the list of PowerShell modules to import before execution.
        /// </summary>
        public List<string> RequiredModules { get; set; } = new();

        /// <summary>
        /// Gets or sets custom module paths for loading modules.
        /// Key is module name, value is path to module directory.
        /// </summary>
        public Dictionary<string, string> ModulePaths { get; set; } = new();

        /// <summary>
        /// Gets or sets the execution policy for the script.
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