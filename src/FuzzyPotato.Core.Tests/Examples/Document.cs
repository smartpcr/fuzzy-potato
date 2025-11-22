// -----------------------------------------------------------------------
// <copyright file="Document.cs" company="FuzzyPotato">
//     Copyright (c) FuzzyPotato. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Tests.Examples
{
    using System.Text.Json.Serialization;
    using FuzzyPotato.Core.Models;
    using FuzzyPotato.Core.Tests.Examples.Converters;

    /// <summary>
    /// Base class for document examples.
    /// Demonstrates polymorphic serialization using type-specific converters.
    /// </summary>
    [JsonConverter(typeof(DocumentBaseJsonConverter))]
    public abstract class DocumentBase : ModelBase
    {
    }

    /// <summary>
    /// Example: Document with text content.
    /// </summary>
    [JsonConverter(typeof(TextDocumentJsonConverter))]
    public class TextDocument : DocumentBase
    {
        /// <inheritdoc/>
        public override string TypeName => "text-document";

        /// <summary>
        /// Gets or sets the document content.
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the word count.
        /// </summary>
        public int WordCount { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        public string Language { get; set; } = "en";
    }

    /// <summary>
    /// Example: Image document.
    /// </summary>
    [JsonConverter(typeof(ImageDocumentJsonConverter))]
    public class ImageDocument : DocumentBase
    {
        /// <inheritdoc/>
        public override string TypeName => "image-document";

        /// <summary>
        /// Gets or sets the image URL.
        /// </summary>
        public string ImageUrl { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the width in pixels.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the height in pixels.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets the image format (png, jpg, etc.).
        /// </summary>
        public string Format { get; set; } = "png";
    }

    /// <summary>
    /// Example: Video document.
    /// </summary>
    [JsonConverter(typeof(VideoDocumentJsonConverter))]
    public class VideoDocument : DocumentBase
    {
        /// <inheritdoc/>
        public override string TypeName => "video-document";

        /// <summary>
        /// Gets or sets the video URL.
        /// </summary>
        public string VideoUrl { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the duration in seconds.
        /// </summary>
        public int DurationSeconds { get; set; }

        /// <summary>
        /// Gets or sets the resolution (e.g., 1080p, 4K).
        /// </summary>
        public string Resolution { get; set; } = "1080p";

        /// <summary>
        /// Gets or sets the codec.
        /// </summary>
        public string Codec { get; set; } = "h264";
    }
}
