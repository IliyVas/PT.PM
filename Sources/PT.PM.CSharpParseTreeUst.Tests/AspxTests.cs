﻿using PT.PM.Common;
using PT.PM.TestUtils;
using AspxParser;
using NUnit.Framework;
using System.IO;
using System.Linq;

namespace PT.PM.CSharpParseTreeUst.Tests
{
    [TestFixture]
    public class AspxTests
    {
        [Test]
        public void Convert_AspxLineColumnPosition_Correct()
        {
            string fileName = Path.Combine(TestHelper.TestsDataPath, "TestAspxParser.aspx");
            string text = File.ReadAllText(fileName);
            var aspxParser = new AspxParser.AspxParser(fileName, true);
            var source = new AspxSource(fileName, text);
            AspxParseResult result = aspxParser.Parse(source);
            var foundNode = result.RootNode.Descendants<AspxNode.AspxExpressionTag>()
                .FirstOrDefault(node => node.Expression.Contains("Expression text"));
            int line, column;
            TextHelper.LinearToLineColumn(foundNode.Location.Start, source.Text, out line, out column);
            Assert.AreEqual(15, line);
            Assert.AreEqual(13, column);
            Assert.AreEqual(foundNode.Location.Start, TextHelper.LineColumnToLinear(source.Text, line, column));

            TextHelper.LinearToLineColumn(foundNode.Location.End, source.Text, out line, out column);
            Assert.AreEqual(15, line);
            Assert.AreEqual(30, column);
            Assert.AreEqual(foundNode.Location.End, TextHelper.LineColumnToLinear(source.Text, line, column));
        }

        [TestCase("TestAspxParser.aspx")]
        [TestCase("Patterns.aspx")]
        public void Parse_AspxFile_WithoutErrors(string fileName)
        {
            TestHelper.CheckFile(fileName, Language.Aspx, Stage.Convert);
        }
    }
}
