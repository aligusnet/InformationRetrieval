using System.Collections.Generic;
using System.Linq;

using DocumentStorage;

namespace NaturalLanguageTools
{
    using Tokens = IEnumerable<string>;

    public class DocumentTokenizer
    {
        private readonly ITokenizer tokenizer;

        public DocumentTokenizer(ITokenizer tokenizer)
        {
            this.tokenizer = tokenizer;
        }

        public void Tokenize(IStorageReader<string> reader, IStorageWriter<IEnumerable<string>> writer)
        {
            var wikipedia = reader.Read();
            var processedWikipedia = ProcessWikipedia(wikipedia);
            writer.Write(processedWikipedia);
        }

        private IEnumerable<DocumentCollection<Tokens>> ProcessWikipedia(IEnumerable<DocumentCollection<string>> wikipedia)
        {
            foreach (var collection in wikipedia)
            {
                yield return ProcessDocCollection(collection);
            }
        }

        private DocumentCollection<Tokens> ProcessDocCollection(DocumentCollection<string> collection)
        {
            return new DocumentCollection<Tokens>(
                ProcessDocuments(collection.Documents).ToList(), 
                collection.Metadata);
        }

        private IEnumerable<Document<Tokens>> ProcessDocuments(IEnumerable<Document<string>> docs)
        {
            foreach (var doc in docs)
            {
                yield return new Document<IEnumerable<string>>(
                    doc.Id, 
                    doc.Title, 
                    tokenizer.Tokenize(doc.Data));
            }
        }
    }
}
