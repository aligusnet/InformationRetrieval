using System;
using System.Collections.Generic;

namespace InformationRetrieval.Wikitext
{
    /// <summary>
    /// Represent Wikitext Document
    /// </summary>
    public class WikitextDocument
    {
        public IEnumerable<IWikitextElement> Elements { get; set; } = Array.Empty<IWikitextElement>();
    }

    /// <summary>
    /// Base interface to represent a single element of Wikitext
    /// </summary>
    public interface IWikitextElement
    {
    }

    /// <summary>
    /// WikitextElement that contains string value
    /// </summary>
    public interface IWikitextValue : IWikitextElement
    {
        string Value { get; }
    }

    public class WikitextHeader1 : IWikitextValue
    {
        public string Value { get; set; } = string.Empty;
    }

    public class WikitextPlainText : IWikitextValue
    {
        public string Value { get; set; } = string.Empty;
    }
}
