using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using InformationRetrieval.Utility;
namespace InformationRetrieval.Tokenizers
{
    public class StateMachineTokenizer
    {
        public static IList<char> Tokenize(IList<char> input, bool lowerCase)
        {
            using var tokenizer = new Tokenizer(input, lowerCase);
            return tokenizer.Tokenize();
        }

        private struct Tokenizer : IDisposable
        {
            enum State
            {
                None,
                Word,
                Number,
                Abbriviation
            }

            private const int BufferSize = 256;
            private const char Space = '\u0020';
            private const char Apostrophe = '\u0027';
            private static readonly char[] Apostrophes = new[] { '\u0027', '\u2019', '\u02BC' };
            private static readonly (char[] Suffix, int[] SpacePositions)[] EnglishSpecialCases = new []
            {
                (new [] {'n', Apostrophe, 't', Apostrophe, 'v', 'e'}, new []{ 6, 3 }),
                (new [] {Apostrophe, 'v', 'e', 'n', Apostrophe, 't'}, new []{ 6, 3 }),
                (new [] {'n', Apostrophe, 't'}, new []{ 3 }),
                (new [] {Apostrophe, 'v', 'e'}, new []{ 3 }),
                (new [] {Apostrophe, 's'}, new []{ 2 }),
                (new [] {Apostrophe, 'm'}, new []{ 2 }),
            };

            private static readonly Func<char[], int, bool> funcIsValidWord;

            static Tokenizer()
            {
                funcIsValidWord = IsValidWord;
            }


            IList<char> input;
            IList<char> output;
            TemporaryBuffer<char> buffer;
            State state;
            private readonly bool lowerCase;
            

            public Tokenizer(IList<char> input, bool lowerCase)
            {
                this.input = input;
                this.lowerCase = lowerCase;
                this.output = new List<char>(input.Count);
                this.buffer = new TemporaryBuffer<char>(BufferSize);
                this.state = State.None;
            }

            public IList<char> Tokenize()
            {
                foreach (char ch in input)
                {
                    state = state switch
                    {
                    State.None => ProcessNone(ch),
                    State.Word => ProcessWord(ch),
                    State.Number => ProcessNumber(ch),
                    State.Abbriviation => ProcessAbbreviation(ch),
                    _ => throw new InvalidOperationException($"Got unknown state {state}")
                    };
                }

                Commit();

                return output;
            }

            public void Dispose()
            {
                buffer.Dispose();
            }

            private static bool IsApostrophe(char ch)
            {
                return Apostrophes.Contains(ch);
            }

            private State ProcessNone(char ch)
            {
                switch (char.GetUnicodeCategory(ch))
                {
                    case UnicodeCategory.UppercaseLetter:
                        buffer.Add(lowerCase ? char.ToLower(ch) : ch);
                        return State.Word;

                    case UnicodeCategory.LowercaseLetter:
                    case UnicodeCategory.ConnectorPunctuation:
                        buffer.Add(ch);
                        return State.Word;

                    case UnicodeCategory.DecimalDigitNumber:
                    case UnicodeCategory.CurrencySymbol:
                        buffer.Add(ch);
                        return State.Number;

                    default:
                        return State.None;
                }
            }

            private State ProcessWord(char ch)
            {
                if (IsApostrophe(ch))
                {
                    buffer.Add(Apostrophe);
                    return State.Word;
                }

                switch (char.GetUnicodeCategory(ch))
                {
                    case UnicodeCategory.OtherPunctuation when buffer.Count == 1:
                        return State.Abbriviation;

                    case UnicodeCategory.UppercaseLetter:
                        buffer.Add(lowerCase ? char.ToLower(ch) : ch);
                        return State.Word;

                    case UnicodeCategory.LowercaseLetter:
                    case UnicodeCategory.DashPunctuation:
                    case UnicodeCategory.ConnectorPunctuation:
                    case UnicodeCategory.DecimalDigitNumber:
                    case UnicodeCategory.ModifierLetter:
                        buffer.Add(ch);
                        return State.Word;

                    case UnicodeCategory.CurrencySymbol:
                        Commit();
                        buffer.Add(ch);
                        return State.Number;

                    default:
                        Commit();
                        return State.None;
                }
            }

            private State ProcessNumber(char ch)
            {
                switch (char.GetUnicodeCategory(ch))
                {
                    case UnicodeCategory.DecimalDigitNumber:
                    case UnicodeCategory.DashPunctuation:
                    case UnicodeCategory.ConnectorPunctuation:
                    case UnicodeCategory.OtherPunctuation:
                        buffer.Add(ch);
                        return State.Number;

                    case UnicodeCategory.UppercaseLetter:
                    case UnicodeCategory.LowercaseLetter:
                        if (!IsValidWord())
                        {
                            Commit();
                        }
                        buffer.Add(lowerCase ? char.ToLower(ch) : ch);
                        return State.Word;

                    default:
                        Commit();
                        return State.None;
                }
            }

            private State ProcessAbbreviation(char ch)
            {
                if (IsApostrophe(ch))
                {
                    Commit();
                    buffer.Add(Apostrophe);
                    return State.Word;
                }

                switch (char.GetUnicodeCategory(ch))
                {
                    case UnicodeCategory.UppercaseLetter:
                        buffer.Add(lowerCase ? char.ToLower(ch) : ch);
                        return State.Abbriviation;

                    case UnicodeCategory.LowercaseLetter:
                        buffer.Add(ch);
                        return State.Abbriviation;

                    case UnicodeCategory.OtherPunctuation:
                        return State.Abbriviation;

                    default:
                        Commit();
                        return State.None;
                }
            }

            private void OnCommit()
            {
                switch (state)
                {
                    case State.Word:
                        OnCommitWord();
                        break;

                    case State.Number:
                        OnCommitNumber();
                        break;
                }
            }

            private void OnCommitNumber()
            {
                switch (char.GetUnicodeCategory(buffer.Last()))
                {
                    case UnicodeCategory.DecimalDigitNumber:
                        break;

                    default:
                        buffer.DiscardLast();
                        break;
                }
            }

            private void OnCommitWord()
            {
                foreach (var cs in EnglishSpecialCases)
                {
                    if (buffer.EndsWith(cs.Suffix))
                    {
                        foreach (var p in cs.SpacePositions)
                        {
                            if (buffer.Count > p)
                            {
                                buffer.Insert(buffer.Count - p, Space);
                            }
                        }

                        break;
                    }
                }

                while (buffer.EndsWith(Apostrophe))
                {
                    buffer.DiscardLast();
                }
            }

            private void Commit()
            {
                OnCommit();

                if (buffer.Count == 0)
                {
                    return;
                }

                if (output.Count > 0)
                {
                    output.Add(Space);
                }

                buffer.Commit(output);
            }

            private bool IsValidWord() => buffer.Apply(funcIsValidWord);

            private static bool IsValidWord(char[] buffer, int position)
            {
                for (int i = 0; i < position; ++i)
                {
                    switch (char.GetUnicodeCategory(buffer[i]))
                    {
                        case UnicodeCategory.UppercaseLetter:
                        case UnicodeCategory.LowercaseLetter:
                        case UnicodeCategory.DashPunctuation:
                        case UnicodeCategory.ConnectorPunctuation:
                        case UnicodeCategory.DecimalDigitNumber:
                        case UnicodeCategory.ModifierLetter:
                            break;
                        default:
                            return false;
                    }
                }
                return true;
            }

        }
    }
}
