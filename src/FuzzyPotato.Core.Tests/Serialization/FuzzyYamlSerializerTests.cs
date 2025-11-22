// -----------------------------------------------------------------------
// <copyright file="FuzzyYamlSerializerTests.cs" company="FuzzyPotato">
//     Copyright (c) FuzzyPotato. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Tests.Serialization
{
    using FuzzyPotato.Core.Models;
    using FuzzyPotato.Core.Serialization;
    using FuzzyPotato.Core.Tests.Examples;

    [TestClass]
    public class FuzzyYamlSerializerTests
    {
        private FuzzyYamlSerializer _serializer = null!;

        [TestInitialize]
        public void Setup()
        {
            this._serializer = new FuzzyYamlSerializer();
            TypeRegistry.Clear();
            TypeRegistry.Register<TextDocument>();
            TypeRegistry.Register<ImageDocument>();
            TypeRegistry.Register<VideoDocument>();
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
            var yaml = this._serializer.Serialize(document);

            // Assert
            yaml.Should().Contain("type:");
            yaml.Should().Contain("text-document");
            yaml.Should().Contain("Hello, World!");
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
            var yaml = this._serializer.Serialize(original);

            // Act
            var deserialized = this._serializer.Deserialize<TextDocument>(yaml);

            // Assert
            deserialized.Should().NotBeNull();
            deserialized!.Id.Should().Be("doc1");
            deserialized.Name.Should().Be("Sample Text");
            deserialized.Content.Should().Be("Hello, World!");
            deserialized.WordCount.Should().Be(2);
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
            var yaml = this._serializer.Serialize(original);

            // Act
            var deserialized = this._serializer.Deserialize<DocumentBase>(yaml);

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
            var yaml = this._serializer.SerializeCollection(documents);

            // Assert
            yaml.Should().Contain("text-document");
            yaml.Should().Contain("image-document");
            yaml.Should().Contain("video-document");
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
            var yaml = this._serializer.SerializeCollection(documents);

            // Act
            var deserialized = this._serializer.DeserializeCollection<DocumentBase>(yaml)?.ToList();

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
            var filePath = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid()}.yaml");

            try
            {
                // Act
                await this._serializer.SerializeToFileAsync(filePath, document);

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
            var filePath = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid()}.yaml");

            try
            {
                await this._serializer.SerializeToFileAsync(filePath, original);

                // Act
                var deserialized = await this._serializer.DeserializeFromFileAsync<TextDocument>(filePath);

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
