using System;

namespace Wikipedia
{
    /// <summary>
    /// Wikipedia Page
    /// </summary>
    public class WikiPage
    {
        /// <summary>
        /// Gets/sets unique ID of the page
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets/sets title of the page
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets/sets text data of the page
        /// </summary>
        public string Data { get; set; }
    }
}
