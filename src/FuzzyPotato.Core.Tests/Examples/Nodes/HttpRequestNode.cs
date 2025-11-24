// -----------------------------------------------------------------------
// <copyright file="HttpRequestNode.cs" company="FuzzyPotato">
//     Copyright (c) FuzzyPotato. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Tests.Examples.Nodes
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;
    using FuzzyPotato.Core.Tests.Examples.Nodes.Converters;

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
}
