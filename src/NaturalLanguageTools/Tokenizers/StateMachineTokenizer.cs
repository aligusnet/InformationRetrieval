using System;
using System.Buffers;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace NaturalLanguageTools.Tokenizers
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

            IList<char> input;
            IList<char> output;
            char[] buffer;
            State state;
            int position;
            private readonly bool lowerCase;

            public Tokenizer(IList<char> input, bool lowerCase)
            {
                this.input = input;
                this.lowerCase = lowerCase;
                this.output = new List<char>(input.Count);
                this.buffer = ArrayPool<char>.Shared.Rent(BufferSize);
                this.state = State.None;
                this.position = 0;
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
                ArrayPool<char>.Shared.Return(buffer);
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
                        Add(lowerCase ? char.ToLower(ch) : ch);
                        return State.Word;

                    case UnicodeCategory.LowercaseLetter:
                    case UnicodeCategory.ConnectorPunctuation:
                        Add(ch);
                        return State.Word;

                    case UnicodeCategory.DecimalDigitNumber:
                    case UnicodeCategory.CurrencySymbol:
                        Add(ch);
                        return State.Number;

                    default:
                        return State.None;
                }
            }

            private State ProcessWord(char ch)
            {
                if (IsApostrophe(ch))
                {
                    Add(Apostrophe);
                    return State.Word;
                }

                switch (char.GetUnicodeCategory(ch))
                {
                    case UnicodeCategory.OtherPunctuation when position == 1:
                        return State.Abbriviation;

                    case UnicodeCategory.UppercaseLetter:
                        Add(lowerCase ? char.ToLower(ch) : ch);
                        return State.Word;

                    case UnicodeCategory.LowercaseLetter:
                    case UnicodeCategory.DashPunctuation:
                    case UnicodeCategory.ConnectorPunctuation:
                    case UnicodeCategory.DecimalDigitNumber:
                    case UnicodeCategory.ModifierLetter:
                        Add(ch);
                        return State.Word;

                    case UnicodeCategory.CurrencySymbol:
                        Commit();
                        Add(ch);
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
                        Add(ch);
                        return State.Number;

                    case UnicodeCategory.UppercaseLetter:
                    case UnicodeCategory.LowercaseLetter:
                        if (!IsValidWord())
                        {
                            Commit();
                        }
                        Add(lowerCase ? char.ToLower(ch) : ch);
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
                    Add(Apostrophe);
                    return State.Word;
                }

                switch (char.GetUnicodeCategory(ch))
                {
                    case UnicodeCategory.UppercaseLetter:
                        Add(lowerCase ? char.ToLower(ch) : ch);
                        return State.Abbriviation;

                    case UnicodeCategory.LowercaseLetter:
                        Add(ch);
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
                switch (char.GetUnicodeCategory(buffer[position-1]))
                {
                    case UnicodeCategory.DecimalDigitNumber:
                        break;

                    default:
                        --position;
                        break;
                }
            }

            private void OnCommitWord()
            {
                foreach (var cs in EnglishSpecialCases)
                {
                    if (EndsWith(cs.Suffix))
                    {
                        foreach (var p in cs.SpacePositions)
                        {
                            if (position > p)
                            {
                                Insert(position - p, Space);
                            }
                        }

                        break;
                    }
                }

                while (EndsWith(Apostrophe))
                {
                    --position;
                }
            }

            private void Commit()
            {
                OnCommit();

                if (position == 0)
                {
                    return;
                }

                if (output.Count > 0)
                {
                    output.Add(Space);
                }

                for (int i = 0; i < position; ++i)
                {
                    output.Add(buffer[i]);
                }

                position = 0;
            }

            private void Add(char ch)
            {
                if (position == buffer.Length)
                {
                    Resize();
                }

                buffer[position] = ch;
                ++position;
            }

            private void Insert(int index, char ch)
            {
                if (position == buffer.Length)
                {
                    Resize();
                }

                for (int i = position; i > index; --i)
                {
                    buffer[i] = buffer[i - 1];
                }

                buffer[index] = ch;
                ++position;
            }

            private void Resize()
            {
                var tmp = buffer;
                buffer = ArrayPool<char>.Shared.Rent(tmp.Length * 2);

                Array.Copy(tmp, 0, buffer, 0, tmp.Length);

                ArrayPool<char>.Shared.Return(tmp);
            }

            private bool EndsWith(params char[] chs)
            {
                if (position < chs.Length)
                {
                    return false;
                }

                int offset = position - chs.Length;

                for (int i = 0; i < chs.Length; ++i)
                {
                    if (buffer[i + offset] != chs[i])
                    {
                        return false;
                    }
                }

                return true;
            }

            private bool IsValidWord()
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
