using System.Linq;
using Xunit;

using NaturalLanguageTools.Tokenizers;
using System.Collections.Generic;

namespace NaturalLanguageToolsUnitTests.Tokenizers
{
    public class StateMachineTokenizerTests
    {
        [Theory]
        [InlineData("U.K.", "UK", false)]
        [InlineData("U.Kʼs T.V.", "uk 's tv", true)]
        [InlineData("aren't", "are n't", false)]
        [InlineData("April O'Neil", "April O'Neil", false)]
        [InlineData("I'm the Doctor!", "i 'm the doctor", true)]
        [InlineData("£157.75, ₽11.1. ", "£157.75 ₽11.1", false)]
        [InlineData("$1/111.", "$1/111", false)]
        [InlineData("Dates: 2010-11-12 11/12/2018 11.12.2018", "Dates 2010-11-12 11/12/2018 11.12.2018", false)]
        [InlineData("shouldn't've   -could'ven't", "should n't 've could 've n't", false)]
        [InlineData("it's done", "it 's done", false)]
        public void SpecialCase(string input, string output, bool lowerCase)
        {
            var result = StateMachineTokenizer.Tokenize(input.ToCharArray(), lowerCase);
            var actual = new string(result.ToArray());

            Assert.Equal(output, actual);
        }

        [Theory]
        [InlineData(WikipediaMendeleev, TokenizedMendeleev)]
        [InlineData(WikipediaPound, TokenizedPound)]
        [InlineData(WikipediaAltenberg, TokenizedAltenberg)]
        public void WikipediaCases(string input, string output)
        {
            var result = StateMachineTokenizer.Tokenize(input.ToCharArray(), lowerCase: false);
            var actual = new string(result.ToArray());

            Assert.Equal(output, actual);
        }

        const string WikipediaMendeleev = @"'''Dmitri Ivanovich Mendeleev'''<ref>Also [[romanization of Russian|romanized]] 
                                            '''Mendeleyev''' or '''Mendeleef'''</ref> ({{IPAc-en|lang|ˌ|m|ɛ|n|d|əl|ˈ|eɪ|ə|f}} 
                                            {{respell|MEN|dəl|AY|əf}};
                                            <ref>[http://dictionary.reference.com/browse/mendeleev ""Mendeleev""].''[[Random House Webster's Unabridged Dictionary]]''.</ref> 
                                            {{Language with name|ru|Russian|links=yes|Дмитрий Иванович Менделеев}},
                                            <ref group=note>In Mendeleev's day, his name was written Дмитрій Ивановичъ Менделѣевъ.</ref>
                                            <small>[[Romanization of Russian|tr.]]</small> {{lang|ru-Latn|Dmítriy Ivánovich Mendeléyev}}<nowiki>,</nowiki>
                                            {{IPA-ru|ˈdmʲitrʲɪj ɪˈvanəvʲɪtɕ mʲɪndʲɪˈlʲejɪf|IPA|ru-Dmitri Mendeleev.ogg}}; 8 February 1834{{snd}}2 February 1907 <small>
                                            {{bracket|[[Adoption of the Gregorian calendar#Adoption in Eastern Europe|OS]] 27&nbsp;January 1834{{snd}}20 January 1907}}</small>)
                                            was a Russian [[chemistry|chemist]] and inventor.
                                            He formulated the [[Periodic Law]], created a farsighted version of the [[periodic table]] of [[Chemical element|elements]],
                                            and used it to correct the properties of some already discovered elements and also to predict the properties of eight elements yet to be discovered.";

        const string TokenizedMendeleev = "Dmitri Ivanovich Mendeleev ref Also romanization of Russian romanized "
                                         + "Mendeleyev or Mendeleef ref IPAc-en lang m ɛ n d əl eɪ ə f "
                                         + "respell MEN dəl AY əf "
                                         + "ref http dictionary reference com browse mendeleev Mendeleev Random House Webster 's Unabridged Dictionary ref "
                                         + "Language with name ru Russian links yes Дмитрий Иванович Менделеев "
                                         + "ref group note In Mendeleev 's day his name was written Дмитрій Ивановичъ Менделѣевъ ref "
                                         + "small Romanization of Russian tr small lang ru-Latn Dmítriy Ivánovich Mendeléyev nowiki nowiki "
                                         + "IPA-ru dmʲitrʲɪj ɪˈvanəvʲɪtɕ mʲɪndʲɪˈlʲejɪf IPA ru-Dmitri Mendeleev ogg 8 February 1834 snd 2 February 1907 small "
                                         + "bracket Adoption of the Gregorian calendar Adoption in Eastern Europe OS 27 nbsp January 1834 snd 20 January 1907 small "
                                         + "was a Russian chemistry chemist and inventor "
                                         + "He formulated the Periodic Law created a farsighted version of the periodic table of Chemical element elements "
                                         + "and used it to correct the properties of some already discovered elements and also to predict the properties of eight elements yet to be discovered";

        const string WikipediaPound = @"Among the measures, tourists were banned from taking more than £50 out of the country in travellers' cheques and remittances, plus £15 in cash;
                                        this restriction was not lifted until 1979.
                                        The pound was devalued by 14.3 per cent to $2.40 on 18 November 1967.";

        const string TokenizedPound = "Among the measures tourists were banned from taking more than £50 out of the country in travellers cheques and remittances plus £15 in cash "
                                    + "this restriction was not lifted until 1979 "
                                    + "The pound was devalued by 14.3 per cent to $2.40 on 18 November 1967";

        const string WikipediaAltenberg = @"{{Use dmy dates|date=June 2013}}
                                            '''Altenberg''' ([[German language|German]] for ""old mountain"") may refer to:

                                            = Places =

                                            === Germany ===
                                            * [[Altenberg, Saxony]], a town in the Free State of Saxony
                                            __NOTOC__

                                            { { disambiguation}
                                                        }
                                            [[Category: Place name disambiguation pages]]";

        const string TokenizedAltenberg = "Use dmy dates date June 2013 "
                                        + "Altenberg German language German for old mountain may refer to "
                                        + "Places "
                                        + "Germany "
                                        + "Altenberg Saxony a town in the Free State of Saxony "
                                        + "__NOTOC__ "
                                        + "disambiguation "
                                        + "Category Place name disambiguation pages";
    }
}
