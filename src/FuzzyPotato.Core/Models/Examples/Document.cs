// -----------------------------------------------------------------------
// <copyright file="Document.cs" company="FuzzyPotato">
//     Copyright (c) FuzzyPotato. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Models.Examples
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Example: Document with text content.
    /// </summary>
    [JsonDerivedType(typeof(TextDocument), typeDiscriminator: "text-document")]
    public class TextDocument : PolymorphicBase
    {
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
    [JsonDerivedType(typeof(ImageDocument), typeDiscriminator: "image-document")]
    public class ImageDocument : PolymorphicBase
    {
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
    [JsonDerivedType(typeof(VideoDocument), typeDiscriminator: "video-document")]
    public class VideoDocument : PolymorphicBase
    {
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
