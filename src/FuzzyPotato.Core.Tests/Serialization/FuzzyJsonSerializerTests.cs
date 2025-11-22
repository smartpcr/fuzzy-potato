// -----------------------------------------------------------------------
// <copyright file="FuzzyJsonSerializerTests.cs" company="FuzzyPotato">
//     Copyright (c) FuzzyPotato. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Tests.Serialization
{
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using FuzzyPotato.Core.Models;
    using FuzzyPotato.Core.Tests.Examples;

    [TestClass]
    public class FuzzyJsonSerializerTests
    {
        private JsonSerializerOptions _options = null!;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            // Types are automatically discovered via custom converters
            // No manual registration needed
        }

        [TestInitialize]
        public void Setup()
        {
            this._options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            };
            foreach (var converter in ConverterRegistry.JsonConverters)
            {
                this._options.Converters.Add(converter);
            }
            this._options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        }

        [TestMethod]
        public void Serialize_TextDocument_ContainsTypeDiscriminator()
        {
            // Arrange
            var document = new TextDocument
            {
                Id = "doc1",
                Name = "Sample Text",
                Content = "Hello, World!",
                WordCount = 2,
                Language = "en",
            };

            // Act
            var json = JsonSerializer.Serialize(document, this._options);

            // Assert
            json.Should().Contain("\"$type\"");
            json.Should().Contain("text-document");
            json.Should().Contain("Hello, World!");
        }

        [TestMethod]
        public void Deserialize_TextDocument_ReturnsCorrectType()
        {
            // Arrange
            var original = new TextDocument
            {
                Id = "doc1",
                Name = "Sample Text",
                Content = "Hello, World!",
                WordCount = 2,
                Language = "en",
            };
            var json = JsonSerializer.Serialize(original, this._options);

            // Act
            var deserialized = JsonSerializer.Deserialize<TextDocument>(json, this._options);

            // Assert
            deserialized.Should().NotBeNull();
            deserialized!.Id.Should().Be("doc1");
            deserialized.Name.Should().Be("Sample Text");
            deserialized.Content.Should().Be("Hello, World!");
            deserialized.WordCount.Should().Be(2);
            deserialized.Language.Should().Be("en");
        }

        [TestMethod]
        public void DeserializePolymorphic_TextDocument_ReturnsBaseType()
        {
            // Arrange
            var original = new TextDocument
            {
                Id = "doc1",
                Name = "Sample Text",
                Content = "Hello, World!",
            };
            var json = JsonSerializer.Serialize(original, this._options);

            // Act
            var deserialized = JsonSerializer.Deserialize<DocumentBase>(json, this._options);

            // Assert
            deserialized.Should().NotBeNull();
            deserialized.Should().BeOfType<TextDocument>();
            var textDoc = (TextDocument)deserialized!;
            textDoc.Content.Should().Be("Hello, World!");
        }

        [TestMethod]
        public void SerializeCollection_MixedTypes_PreservesTypes()
        {
            // Arrange
            var documents = new List<PolymorphicBase>
            {
                new TextDocument { Id = "1", Name = "Text", Content = "Sample" },
                new ImageDocument { Id = "2", Name = "Image", ImageUrl = "http://example.com/img.png", Width = 800 },
                new VideoDocument { Id = "3", Name = "Video", VideoUrl = "http://example.com/vid.mp4", DurationSeconds = 120 },
            };

            // Act
            var json = JsonSerializer.Serialize(documents, this._options);

            // Assert
            json.Should().Contain("text-document");
            json.Should().Contain("image-document");
            json.Should().Contain("video-document");
        }

        [TestMethod]
        public void DeserializeCollection_MixedTypes_ReturnsCorrectTypes()
        {
            // Arrange
            var documents = new List<PolymorphicBase>
            {
                new TextDocument { Id = "1", Name = "Text", Content = "Sample" },
                new ImageDocument { Id = "2", Name = "Image", ImageUrl = "http://example.com/img.png", Width = 800 },
                new VideoDocument { Id = "3", Name = "Video", VideoUrl = "http://example.com/vid.mp4", DurationSeconds = 120 },
            };
            var json = JsonSerializer.Serialize(documents, this._options);

            // Act
            var deserialized = JsonSerializer.Deserialize<List<DocumentBase>>(json, this._options);

            // Assert
            deserialized.Should().NotBeNull();
            deserialized.Should().HaveCount(3);
            deserialized![0].Should().BeOfType<TextDocument>();
            deserialized[1].Should().BeOfType<ImageDocument>();
            deserialized[2].Should().BeOfType<VideoDocument>();
        }

        [TestMethod]
        public async Task SerializeToFileAsync_TextDocument_CreatesFile()
        {
            // Arrange
            var document = new TextDocument
            {
                Id = "doc1",
                Name = "Sample Text",
                Content = "Hello, World!",
            };
            var filePath = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid()}.json");

            try
            {
                // Act
                var json = JsonSerializer.Serialize(document, this._options);
                await File.WriteAllTextAsync(filePath, json);

                // Assert
                File.Exists(filePath).Should().BeTrue();
                var fileContent = await File.ReadAllTextAsync(filePath);
                fileContent.Should().Contain("text-document");
            }
            finally
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }

        [TestMethod]
        public async Task DeserializeFromFileAsync_TextDocument_ReturnsObject()
        {
            // Arrange
            var original = new TextDocument
            {
                Id = "doc1",
                Name = "Sample Text",
                Content = "Hello, World!",
            };
            var filePath = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid()}.json");

            try
            {
                var json = JsonSerializer.Serialize(original, this._options);
                await File.WriteAllTextAsync(filePath, json);

                // Act
                var fileContent = await File.ReadAllTextAsync(filePath);
                var deserialized = JsonSerializer.Deserialize<TextDocument>(fileContent, this._options);

                // Assert
                deserialized.Should().NotBeNull();
                deserialized!.Id.Should().Be("doc1");
                deserialized.Content.Should().Be("Hello, World!");
            }
            finally
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }
    }
}
