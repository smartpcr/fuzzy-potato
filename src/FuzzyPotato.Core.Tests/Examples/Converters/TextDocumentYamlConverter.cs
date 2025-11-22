// -----------------------------------------------------------------------
// <copyright file="TextDocumentYamlConverter.cs" company="FuzzyPotato">
//     Copyright (c) FuzzyPotato. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Tests.Examples.Converters
{
    using System;
    using FuzzyPotato.Core.Tests.Examples;
    using YamlDotNet.Core;
    using YamlDotNet.Core.Events;
    using YamlDotNet.Serialization;

    /// <summary>
    /// YAML converter for TextDocument.
    /// </summary>
    public class TextDocumentYamlConverter : IYamlTypeConverter
    {
        /// <inheritdoc/>
        public bool Accepts(Type type)
        {
            return type == typeof(TextDocument);
        }

        /// <inheritdoc/>
        public object? ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
        {
            if (parser.Current is not MappingStart)
            {
                throw new YamlException("Expected mapping start for TextDocument");
            }

            parser.MoveNext(); // Consume MappingStart

            var doc = new TextDocument();

            while (parser.Current is not MappingEnd)
            {
                if (parser.Current is Scalar scalar)
                {
                    var propertyName = scalar.Value;
                    parser.MoveNext(); // Move to value

                    switch (propertyName)
                    {
                        case "$type":
                        case "typeName":
                            // Skip discriminator
                            parser.MoveNext();
                            break;
                        case "content":
                            doc.Content = parser.Consume<Scalar>().Value;
                            break;
                        case "wordCount":
                            doc.WordCount = int.Parse(parser.Consume<Scalar>().Value);
                            break;
                        case "language":
                            doc.Language = parser.Consume<Scalar>().Value;
                            break;
                        case "id":
                            doc.Id = parser.Consume<Scalar>().Value;
                            break;
                        case "name":
                            doc.Name = parser.Consume<Scalar>().Value;
                            break;
                        case "createdAt":
                            doc.CreatedAt = DateTime.Parse(parser.Consume<Scalar>().Value);
                            break;
                        default:
                            // Skip unknown properties
                            this.SkipValue(parser);
                            break;
                    }
                }
                else
                {
                    parser.MoveNext();
                }
            }

            parser.MoveNext(); // Consume MappingEnd
            return doc;
        }

        /// <inheritdoc/>
        public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
        {
            if (value is not TextDocument doc)
            {
                emitter.Emit(new Scalar(string.Empty));
                return;
            }

            emitter.Emit(new MappingStart());

            // Write $type discriminator first
            emitter.Emit(new Scalar("$type"));
            emitter.Emit(new Scalar(doc.TypeName));

            // Write properties
            emitter.Emit(new Scalar("content"));
            emitter.Emit(new Scalar(doc.Content));

            emitter.Emit(new Scalar("wordCount"));
            emitter.Emit(new Scalar(doc.WordCount.ToString()));

            emitter.Emit(new Scalar("language"));
            emitter.Emit(new Scalar(doc.Language));

            emitter.Emit(new Scalar("id"));
            emitter.Emit(new Scalar(doc.Id));

            emitter.Emit(new Scalar("name"));
            emitter.Emit(new Scalar(doc.Name));

            emitter.Emit(new Scalar("createdAt"));
            emitter.Emit(new Scalar(doc.CreatedAt.ToString("o")));

            emitter.Emit(new MappingEnd());
        }

        private void SkipValue(IParser parser)
        {
            var depth = 0;
            do
            {
                if (parser.Current is MappingStart or SequenceStart)
                {
                    depth++;
                }
                else if (parser.Current is MappingEnd or SequenceEnd)
                {
                    depth--;
                }

                parser.MoveNext();
            }
            while (depth > 0);
        }
    }
}
