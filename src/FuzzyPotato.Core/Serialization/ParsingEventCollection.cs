// -----------------------------------------------------------------------
// <copyright file="ParsingEventCollection.cs" company="Microsoft Corp.">
//     Copyright (c) Microsoft Corp. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FuzzyPotato.Core.Serialization
{
    using YamlDotNet.Core.Events;

    /// <summary>
    /// Collection of parsing events that can be enumerated.
    /// </summary>
    internal class ParsingEventCollection : IEnumerable<ParsingEvent>
    {
        private readonly List<ParsingEvent> events;

        public ParsingEventCollection(List<ParsingEvent> events)
        {
            this.events = events;
        }

        public IEnumerator<ParsingEvent> GetEnumerator() => this.events.GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}