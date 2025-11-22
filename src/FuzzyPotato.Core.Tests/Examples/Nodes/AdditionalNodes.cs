// -----------------------------------------------------------------------
// <copyright file="AdditionalNodes.cs" company="FuzzyPotato">
//     Copyright (c) FuzzyPotato. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Tests.Examples.Nodes
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;
    using FuzzyPotato.Core.Tests.Examples.Nodes.Converters;

    /// <summary>
    /// Legacy alias for WhileNode for backwards compatibility.
    /// </summary>
    [JsonConverter(typeof(WhileLoopNodeJsonConverter))]
    public class WhileLoopNode : WhileNode
    {
        /// <inheritdoc/>
        public override string TypeName => "while-loop-node";

        /// <summary>
        /// Gets or sets the ID of the first node in the loop body.
        /// </summary>
        public string? LoopBodyStartNodeId { get; set; }
    }

    /// <summary>
    /// Node that performs HTTP requests.
    /// </summary>
    [JsonConverter(typeof(HttpRequestNodeJsonConverter))]
    public class HttpRequestNode : NodeDefinition
    {
        /// <inheritdoc/>
        public override string TypeName => "http-request-node";

        /// <summary>
        /// Gets or sets the HTTP method (GET, POST, PUT, DELETE, etc.).
        /// </summary>
        public string Method { get; set; } = "GET";

        /// <summary>
        /// Gets or sets the URL to request.
        /// </summary>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the request headers.
        /// </summary>
        public Dictionary<string, string> Headers { get; set; } = new();

        /// <summary>
        /// Gets or sets the request body.
        /// </summary>
        public string? Body { get; set; }

        /// <summary>
        /// Gets or sets the content type header value.
        /// </summary>
        public string ContentType { get; set; } = "application/json";

        /// <summary>
        /// Gets or sets the timeout in milliseconds.
        /// </summary>
        public int TimeoutMs { get; set; } = 30000;

        /// <summary>
        /// Gets or sets a value indicating whether to validate SSL certificates.
        /// </summary>
        public bool ValidateSsl { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to follow redirects.
        /// </summary>
        public bool FollowRedirects { get; set; } = true;

        /// <summary>
        /// Gets or sets the maximum number of redirects to follow.
        /// </summary>
        public int MaxRedirects { get; set; } = 10;
    }

    /// <summary>
    /// Node that delays execution for a specified duration.
    /// </summary>
    [JsonConverter(typeof(DelayNodeJsonConverter))]
    public class DelayNode : NodeDefinition
    {
        /// <inheritdoc/>
        public override string TypeName => "delay-node";

        /// <summary>
        /// Gets or sets the delay duration in milliseconds.
        /// </summary>
        public int DelayMs { get; set; } = 1000;

        /// <summary>
        /// Gets or sets a value indicating whether the delay can be cancelled.
        /// </summary>
        public bool Cancellable { get; set; } = true;
    }

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
