using System;
using System.Collections.Generic;
using NaturalLangugeTools;
using DocumentStorage;

namespace NaturalLanguageApp
{
    public class WikipediaTokenizer
    {
        private readonly ITokenizer tokenizer;

        public WikipediaTokenizer(ITokenizer tokenizer)
        {
            this.tokenizer = tokenizer;
        }

        public void Tokenize(IStorageReader reader, IStorageWriter writer)
        {
            var wikipedia = reader.Read();
            var processedWikipedia = ProcessWikipedia(wikipedia);
            writer.Write(processedWikipedia);
        }

        private IEnumerable<DocumentCollection> ProcessWikipedia(IEnumerable<DocumentCollection> wikipedia)
        {
            foreach (var collection in wikipedia)
            {
                yield return ProcessDocCollection(collection);
            }
        }

        private DocumentCollection ProcessDocCollection(DocumentCollection collection)
        {
            return new DocumentCollection
            {
                Metadata = collection.Metadata,
                Pages = ProcessDocuments(collection.Pages),
            };
        }

        private IEnumerable<Document> ProcessDocuments(IEnumerable<Document> docs)
        {
            foreach (var doc in docs)
            {
                var data = string.Join(' ', tokenizer.Tokenize(doc.Data));
                var title = string.Join(' ', tokenizer.Tokenize(doc.Title));

                yield return new Document
                {
                    Id = doc.Id,
                    Title = doc.Title,
                    Data = string.Join(Environment.NewLine, title, data)
                };
            }
        }
    }
}
