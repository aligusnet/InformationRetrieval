using System;
using System.Collections.Generic;
using System.Globalization;

using NaturalLanguageTools.Utility;

namespace NaturalLanguageTools.Wikitext
{
    /// <summary>
    /// Performs rough cleaning from some wikitext markups, like [[]] and {{}}
    /// </summary>
    public class WikitextCleaner
    {
        public static IList<char> Clean(IList<char> input)
        {
            using var cleaner = new Cleaner(input);
            return cleaner.Clean();
        }

        private class Cleaner : IDisposable
        {
            enum State
            {
                NormalText,
                Template,
                OpenCurlyBracket,
                Link,
                OpenSquareBracket,
                Tag,
                OpenAngleBracket,
            }

            private readonly TemporaryBuffer<char> buffer;
            private readonly IList<char> input;
            private readonly IList<char> output;
            private State state;
            private int templateNestingLevel;
            private readonly Action<char[], int> actionCommitLink;

            public Cleaner(IList<char> input)
            {
                buffer = new TemporaryBuffer<char>(256);
                state = State.NormalText;
                this.input = input;
                output = new List<char>(input.Count);
                templateNestingLevel = 0;
                actionCommitLink = CommitLink;
            }

            public IList<char> Clean()
            {
                foreach (char ch in input)
                {
                    var category = char.GetUnicodeCategory(ch);

                    // ignore Surrogate symbols
                    if (category == UnicodeCategory.Surrogate)
                    {
                        continue;
                    }

                    state = state switch
                    {
                        State.NormalText => processNormalText(ch),
                        State.Template => processTemplate(ch),
                        State.OpenCurlyBracket => processOpenCurlyBracket(ch),
                        State.Link => processLink(ch),
                        State.OpenSquareBracket => processOpenSquareBracket(ch),
                        State.Tag => processTag(ch, category),
                        State.OpenAngleBracket => processOpenAngleBracket(ch, category),
                        _ => throw new InvalidOperationException($"Got unknown state {state}")
                    };
                }
                return output;
            }

            private State processNormalText(char ch)
            {
                switch (ch)
                {
                    case '{':
                        buffer.Add(ch);
                        return State.OpenCurlyBracket;

                    case '[':
                        buffer.Add(ch);
                        return State.OpenSquareBracket;

                    case '<':
                        buffer.Add(ch);
                        return State.OpenAngleBracket;

                    default:
                        output.Add(ch);
                        return State.NormalText;
                }
            }

            // ignores eveything inside template {{ <template> }}
            private State processTemplate(char ch)
            {
                switch (ch)
                {
                    case '}' when buffer.Last() == '}':
                        templateNestingLevel--;
                        if (templateNestingLevel <= 0)
                        {
                            buffer.Discard();
                            return State.NormalText;
                        }
                        else
                        {
                            buffer.Add(ch);
                            return State.Template;
                        }

                    case '{' when buffer.Last() == '{':
                        templateNestingLevel++;
                        buffer.Add(ch);
                        return State.Template;

                    default:
                        buffer.Add(ch);
                        return State.Template;
                }
            }

            private State processOpenCurlyBracket(char ch)
            {
                switch (ch)
                {
                    case '{':
                        templateNestingLevel++;
                        buffer.Add(ch);
                        return State.Template;

                    default:
                        buffer.Commit(output);
                        output.Add(ch);
                        return State.NormalText;
                }
            }

            // extracts text from inside the link [[<target>|<text>]] or [[<text>]]
            private State processLink(char ch)
            {
                switch (ch)
                {
                    case ']' when buffer.Last() == ']':
                        buffer.Apply(actionCommitLink);
                        buffer.Discard();
                        return State.NormalText;

                    default:
                        buffer.Add(ch);
                        return State.Link;
                }
            }

            private State processOpenSquareBracket(char ch)
            {
                switch (ch)
                {
                    case '[':
                        buffer.Add(ch);
                        return State.Link;

                    default:
                        buffer.Commit(output);
                        output.Add(ch);
                        return State.NormalText;
                }
            }

            private void CommitLink(char[] data, int length)
            {
                int start = Array.IndexOf<char>(data, '|', 0, length);
                if (start < 0)
                {
                    start = 2;  // skip "[["
                }
                else
                {
                    start += 1;
                }

                int end = Array.IndexOf<char>(data, ']', start, length - start);

                for (int i = start; i < end; ++i)
                {
                    output.Add(data[i]);
                }
            }

            // ignores tags
            private State processTag(char ch, UnicodeCategory category)
            {
                switch (ch, category)
                {
                    case ('>', _):
                        buffer.Discard();
                        return State.NormalText;

                    default:
                        buffer.Add(ch);
                        return State.Tag;
                }
            }

            private State processOpenAngleBracket(char ch, UnicodeCategory category)
            {
                switch (ch, category)
                {
                    case (_, UnicodeCategory.UppercaseLetter):
                    case (_, UnicodeCategory.LowercaseLetter):
                    case ('!', _):
                    case ('/', _):
                        buffer.Add(ch);
                        return State.Tag;

                    default:
                        buffer.Commit(output);
                        output.Add(ch);
                        return State.NormalText;
                }
            }

            public void Dispose()
            {
                buffer.Dispose();
            }
        }
    }
}
