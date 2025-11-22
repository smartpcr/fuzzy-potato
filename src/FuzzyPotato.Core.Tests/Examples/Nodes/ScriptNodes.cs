// -----------------------------------------------------------------------
// <copyright file="ScriptNodes.cs" company="FuzzyPotato">
//     Copyright (c) FuzzyPotato. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Tests.Examples.Nodes
{
    using System.Collections.Generic;
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
