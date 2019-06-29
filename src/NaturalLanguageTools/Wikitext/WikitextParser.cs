using Sprache;
using System.Collections.Generic;
using System.Linq;

namespace NaturalLanguageTools.Wikitext
{
    public static class WikitextParser
    {
        public static WikitextDocument Parse(string wikitext)
        {
            return new WikitextDocument() { Elements = TermList.Parse(wikitext) };
        }

        static readonly Parser<IEnumerable<IWikitextElement>> TermList =
            from first in Sprache.Parse.WhiteSpace.Many().Optional()
            from terms in FormattedText.DelimitedBy(Sprache.Parse.LineEnd.Repeat(2))
            from rest in Sprache.Parse.WhiteSpace.Many().Optional().End()
            select terms;

        private static readonly Parser<IWikitextElement> Header1 =
            from ot in Sprache.Parse.Char('=').Once()
            from text in ParsePlainText(except: WikiMarkup.Or(Sprache.Parse.Char('=')))
            from ct in Sprache.Parse.Char('=').Once()
            select new WikitextHeader1 { Value = text };

        private static readonly Parser<char> WikiMarkup =
            Sprache.Parse.Char('{')
            .Or(Sprache.Parse.Char('}'))
            .Or(Sprache.Parse.Char('['))
            .Or(Sprache.Parse.Char('['))
            .Or(Sprache.Parse.Char('&'));

        private static Parser<IWikitextElement> PlainText =
            from text in ParsePlainText(except: WikiMarkup)
                select new WikitextPlainText { Value = text };

        private static readonly Parser<IWikitextElement> FormattedText =
            Header1
            .Or(PlainText);

        private static Parser<string> ParsePlainText(Parser<char> except)
        {
            return
                from text in Sprache.Parse.AnyChar.Except(except).AtLeastOnce().Text()
                select text.Trim();
        }

    }

}
