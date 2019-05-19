using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Wikidump
{
    public class WikiDumpXmlReader : IWikiDumpReader
    {
        private const string PageTag = "page";
        private const string TitlePath = "./x:title";
        private const string RedirectTitlePath = "./x:redirect/@title";
        private const string TextPath = "./x:revision/x:text";

        private Stream stream = null;

        private IXmlNamespaceResolver namespaceResolver;

        public WikiDumpXmlReader(Stream stream)
        {
            this.stream = stream;

            var nsm = new XmlNamespaceManager(new NameTable());
            nsm.AddNamespace(string.Empty, "http://www.mediawiki.org/xml/export-0.10/");
            nsm.AddNamespace("x", "http://www.mediawiki.org/xml/export-0.10/");
            this.namespaceResolver = nsm;
        }

        public WikiDumpXmlReader(string filePath): this(new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {   
        }

        public void Dispose()
        {
            if (stream != null)
            {
                stream.Dispose();
            }
        }

        public IEnumerable<WikiDumpPage> ReadPages()
        {
            using (XmlReader reader = XmlReader.Create(stream))
            {
                reader.MoveToContent();

                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == PageTag)
                    {
                        if (XNode.ReadFrom(reader) is XElement el)
                        {
                            yield return CreatePage(el);
                        }
                    }
                }
            }
        }

        private WikiDumpPage CreatePage(XElement element)
        {
            return new WikiDumpPage(GetValue(element, TitlePath), GetValue(element, TextPath))
            {
                RedirectTitle = GetAttributeValue(element, RedirectTitlePath),
            };
        }

        private string GetValue(XElement element, string path)
        {
            return element.XPathSelectElement(path, this.namespaceResolver)?.Value;
        }

        private string GetAttributeValue(XElement element, string path)
        {
            var attrs = element.XPathEvaluate(path, this.namespaceResolver) as IEnumerable;

            return attrs?.Cast<XAttribute>()?.FirstOrDefault()?.Value;
        }
    }
}
