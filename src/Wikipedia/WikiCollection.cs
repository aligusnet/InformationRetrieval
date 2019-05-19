using System;
using System.Collections.Generic;

namespace Wikipedia
{
    /// <summary>
    /// Represents a collection of wikipedia pages
    /// </summary>
    public class WikiCollection
    {
        /// <summary>
        /// Contents (ID -> Title map) of the collection
        /// </summary>
        public IDictionary<Guid, string> Contents { get; set; }

        /// <summary>
        /// List of Wikipedia pages in the collection
        /// </summary>
        public IEnumerable<WikiPage> Pages { get; set; }
    }
}
