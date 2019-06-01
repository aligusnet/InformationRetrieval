using System;
using System.Collections.Generic;
using NaturalLangugeTools;
using DocumentStorage;
using System.Linq;

namespace NaturalLanguageApp
{
    using Tokens = IEnumerable<string>;

    public class WikipediaTokenizer
    {
        private readonly ITokenizer tokenizer;

        public WikipediaTokenizer(ITokenizer tokenizer)
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
            return new DocumentCollection<Tokens>
            {
                Metadata = collection.Metadata,
                Documents = ProcessDocuments(collection.Documents).ToList(),
            };
        }

        private IEnumerable<Document<Tokens>> ProcessDocuments(IEnumerable<Document<string>> docs)
        {
            foreach (var doc in docs)
            {
                yield return new Document<IEnumerable<string>>
                {
                    Id = doc.Id,
                    Title = doc.Title,
                    Data = tokenizer.Tokenize(doc.Data),
                };
            }
        }
    }
}
