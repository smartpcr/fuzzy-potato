// -----------------------------------------------------------------------
// <copyright file="EventReader.cs" company="Microsoft Corp.">
//     Copyright (c) Microsoft Corp. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Serialization
{
    using YamlDotNet.Core;
    using YamlDotNet.Core.Events;

    /// <summary>
    /// Event reader that replays buffered parsing events.
    /// </summary>
    internal class EventReader : IParser
    {
        private readonly IEnumerator<ParsingEvent> enumerator;
        private ParsingEvent? current;

        public EventReader(IEnumerable<ParsingEvent> events)
        {
            this.enumerator = events.GetEnumerator();
        }

        public ParsingEvent? Current => this.current;

        public bool MoveNext()
        {
            var result = this.enumerator.MoveNext();
            this.current = result ? this.enumerator.Current : null;
            return result;
        }
    }
}