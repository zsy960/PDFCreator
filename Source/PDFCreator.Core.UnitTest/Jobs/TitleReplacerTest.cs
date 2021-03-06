﻿using System;
using System.Collections.Generic;
using NUnit.Framework;
using pdfforge.PDFCreator.Core.Jobs;
using pdfforge.PDFCreator.Core.Settings;
using pdfforge.PDFCreator.Core.Settings.Enums;

namespace PDFCreator.Core.UnitTest.Jobs
{
    [TestFixture]
    class TitleReplacerTest
    {
        [Test]
        public void WithEmptyConstructor_DoesNotReplaceAnything()
        {
            var titleReplacer = new TitleReplacer();
            const string originalTitle = "My Sample Title - Microsoft Word";

            string title = titleReplacer.Replace(originalTitle);

            Assert.AreEqual(originalTitle, title);
        }

        [Test]
        public void WithEmptySearchString_DoesNotReplaceAnything()
        {
            var titleReplacer = new TitleReplacer();
            const string originalTitle = "My Sample Title - Microsoft Word";

            AddSearchAndReplace(titleReplacer, "", "This string must be ignored");

            string title = titleReplacer.Replace(originalTitle);

            Assert.AreEqual(originalTitle, title);
        }

        [Test]
        public void WithAddedReplacements_WhenReplacing_ReplacesTitleParts()
        {
            var titleReplacer = new TitleReplacer();
            AddSearchAndReplace(titleReplacer, " - Microsoft Word", "");
            const string originalTitle = "My Sample Title - Microsoft Word";

            string title = titleReplacer.Replace(originalTitle);

            Assert.AreEqual("My Sample Title", title);
        }

        [Test]
        public void WithAddedSearchAndReplaceReplacement_WhenReplacing_ReplacesTitleParts()
        {
            var titleReplacer = new TitleReplacer();
            titleReplacer.AddReplacement(new TitleReplacement(ReplacementType.Replace, "- Microsoft Word", "abc"));
            const string originalTitle = "My Sample Title - Microsoft Word";

            string title = titleReplacer.Replace(originalTitle);

            Assert.AreEqual("My Sample Title abc", title);
        }

        [Test]
        public void WithAddedReplaceStartReplacement_WhenReplacing_ReplacesTitleParts()
        {
            var titleReplacer = new TitleReplacer();
            titleReplacer.AddReplacement(new TitleReplacement(ReplacementType.Start, "My", "xx"));
            const string originalTitle = "My Sample My Title - Microsoft Word";

            string title = titleReplacer.Replace(originalTitle);

            Assert.AreEqual("xx Sample My Title - Microsoft Word", title);
        }

        [Test]
        public void WithAddedReplaceEndReplacement_WhenReplacing_ReplacesTitleParts()
        {
            var titleReplacer = new TitleReplacer();
            titleReplacer.AddReplacement(new TitleReplacement(ReplacementType.End, "Word", "xx"));
            const string originalTitle = "My Sample Word - Microsoft Word";

            string title = titleReplacer.Replace(originalTitle);

            Assert.AreEqual("My Sample Word - Microsoft xx", title);
        }

        [Test]
        public void WithAddedRegexReplacement_WhenReplacing_ReplacesTitleParts()
        {
            var titleReplacer = new TitleReplacer();
            AddRegex(titleReplacer, "(.*) - .*(Microsoft).*", "$1 $2");
            const string originalTitle = "My Sample Title - Microsoft Word";

            string title = titleReplacer.Replace(originalTitle);

            Assert.AreEqual("My Sample Title Microsoft", title);
        }

        [Test]
        public void WithAddedRegexReplacement_WithEmptyReplaceString_DoesNotReplaceAnything()
        {
            var titleReplacer = new TitleReplacer();
            AddRegex(titleReplacer, ".*", "");
            const string originalTitle = "My Sample Title - Microsoft Word";

            string title = titleReplacer.Replace(originalTitle);

            Assert.AreEqual("My Sample Title - Microsoft Word", title);
        }

        [Test]
        public void AfterAddingReplacementCollection_WhenReplacing_ReplacesTitleParts()
        {
            var replacements = new List<TitleReplacement>();
            replacements.Add(new TitleReplacement(ReplacementType.Replace, "One", "Two"));
            replacements.Add(new TitleReplacement(ReplacementType.Replace, "Alpha", "Beta"));
            var titleReplacer = new TitleReplacer();
            titleReplacer.AddReplacements(replacements);
            const string originalTitle = "Alpha - One";

            string title = titleReplacer.Replace(originalTitle);

            Assert.AreEqual("Beta - Two", title);
        }

        [Test]
        public void WithMultipleReplacement_ReplacedInCorrectOrder()
        {
            var titleReplacer = new TitleReplacer();
            
            AddSearchAndReplace(titleReplacer, ".doc", "Must not be visible, because .docx should be already removed");
            AddSearchAndReplace(titleReplacer, ".docx", "Empty");
            
            const string originalTitle = "My Sample Title.docx";

            string title = titleReplacer.Replace(originalTitle);

            Assert.AreEqual("My Sample TitleEmpty", title);
        }

        [Test]
        public void Replace_WithNullTitle_ThrowsArgumentException()
        {
            var titleReplacer = new TitleReplacer();

            Assert.Throws<ArgumentException>(() => titleReplacer.Replace(null));
        }


        private void AddSearchAndReplace(TitleReplacer titleReplacer, string searchString, string replaceWith)
        {
            titleReplacer.AddReplacement(new TitleReplacement(ReplacementType.Replace, searchString, replaceWith));
        }

        private void AddRegex(TitleReplacer titleReplacer, string searchString, string replaceWith)
        {
            titleReplacer.AddReplacement(new TitleReplacement(ReplacementType.RegEx, searchString, replaceWith));
        }
    }
}
