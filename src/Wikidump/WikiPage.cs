using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Wikidump
{
    public class WikiPage
    {
        private static readonly Regex reSpecial = new Regex(@"^(?<key>[\w\d]+):[\w\d]+", RegexOptions.Compiled);
        private static ISet<string> specialKeys = new HashSet<string> { "Category", "Wikipedia", "File", "Template", "Portal", "MediaWiki", "Help", "Draft" };

        private readonly Lazy<bool> isSpecialPage;

        public WikiPage(string title, string text)
        {
            Title = title;
            Text = text;

            isSpecialPage = new Lazy<bool>(IsSpecialPage);
        }

        public string Title { get; }

        public string RedirectTitle { get; set; }

        public string Text { get; }

        public bool IsRedirect => RedirectTitle != null;

        public bool IsSpecial => isSpecialPage.Value;

        public bool IsContent => !IsSpecial && !IsRedirect;

        private bool IsSpecialPage()
        {
            var matches = reSpecial.Matches(Title);
            if (matches.Count > 0)
            {
                var key = matches[0].Groups["key"].Value;
                return specialKeys.Contains(key);
            }

            return false;
        }
    }
}
