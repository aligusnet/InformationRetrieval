using System;
using System.Linq;

namespace Wikidump
{
    public class WikiDumpTransformer
    {
        public static readonly Func<WikiPage, bool> DefaultFilter = p => p.IsContent;

        public static void Transform(IWikiDumpReader reader, IWikiDumpWriter writer, Func<WikiPage, bool> where)
        {
            writer.WritePages(reader.ReadPages().Where(where));
        }

        public static void Transform(IWikiDumpReader reader, IWikiDumpWriter writer, Func<WikiPage, bool> filter, int count)
        {
            writer.WritePages(reader.ReadPages().Where(filter).Take(count));
        }

        public static void Transform(string dumpFilePath, string pathToSave, int count = -1)
        {
            using (var reader = new WikiDumpXmlReader(dumpFilePath))
            {
                var writer = new WikiDumpZipWriter(pathToSave);

                if (count > 0)
                {
                    Transform(reader, writer, DefaultFilter, count);
                }
                else
                {
                    Transform(reader, writer, DefaultFilter);
                }
            }
        }
    }
}
