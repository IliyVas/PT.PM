using PT.PM.Common;
using PT.PM.Common.CodeRepository;
using PT.PM.Common.Nodes.Expressions;
using PT.PM.Common.Nodes.Tokens;
using PT.PM.Common.Nodes.Tokens.Literals;
using System;
using System.Linq;
using System.Collections.Generic;
using PT.PM.TestUtils;
using NUnit.Framework;
using PT.PM.Common.Nodes.GeneralScope;

namespace PT.PM.JavaParseTreeUst.Tests
{
    [TestFixture]
    public class JavaConverterTests
    {
        [TestCase("ManyStringsConcat.java")]
        [TestCase("AllInOne.java")]
        [TestCase("AllInOne8.java")]
        public void Convert_Java_WithoutErrors(string fileName)
        {
            TestHelper.CheckFile(fileName, Language.Java, Stage.Convert);
        }

        [Test]
        public void Convert_JavaWebGoat_WithoutErrors()
        {
            string projectKey = "WebGoat.Java-05a1f5";
            TestHelper.CheckProject(TestProjects.JavaProjects.Single(p => p.Key == projectKey),
                Language.Java, Stage.Parse);
        }

        [Test]
        public void Convert_JavaPatternsWithErrors_MatchedResultsEqual()
        {
            var patternsLogger = new LoggerMessageCounter();
            TestHelper.CheckFile("Patterns.java", Language.Java, Stage.Match, patternsLogger);

            var patternWithErrorsLogger = new LoggerMessageCounter();
            TestHelper.CheckFile("PatternsWithParseErrors.java", Language.Java, Stage.Match, patternWithErrorsLogger, true);

            Assert.AreEqual(patternsLogger.InfoMessageCount, patternWithErrorsLogger.InfoMessageCount);
        }

        [Test]
        public void Convert_JavaArrayInitialization()
        {
            var sourceCodeRep = new MemoryCodeRepository(
                "class ArrayInitialization {\r\n" +
                    "public void init() {\r\n" +
                        "int[] arr1 = new int[] { 1, 2, 3 };\r\n" +
                        "int[][] arr2 = new int[1][2];\r\n" +
                        "int[][] arr3 = new int[1][];\r\n" +
                    "}\r\n" +
                "}"
            );

            var workflow = new Workflow(sourceCodeRep, Language.Java, stage: Stage.Convert);
            var workflowResult = workflow.Process();
            var ust = workflowResult.Usts.First();
            var intType = new TypeToken("int");

            var arrayData = new List<Tuple<List<Expression>, List<Expression>>>();
            // new int[] { 1, 2, 3 };
            arrayData.Add(new Tuple<List<Expression>, List<Expression>>(
                Enumerable.Range(1, 3).Select(num => new IntLiteral(num)).ToList<Expression>(),
                new List<Expression> { new IntLiteral(0) }
            ));
            // new int[1][2];
            arrayData.Add(new Tuple<List<Expression>, List<Expression>>(
                null,
                new List<Expression> { new IntLiteral(1), new IntLiteral(2) }
            ));
            // new int[1][];
            arrayData.Add(new Tuple<List<Expression>, List<Expression>>(
                null,
                new List<Expression> { new IntLiteral(1), new IntLiteral(0) }
            ));

            for (var i = 0; i < arrayData.Count; i++)
            {
                var data = arrayData[i];
                var arrayCreationExpression = new ArrayCreationExpression
                {
                    Type = intType,
                    Initializers = data.Item1,
                    Sizes = data.Item2
                };
                bool exist = ust.Root.DoesAnyDescendantMatchPredicate(node => node.Equals(arrayCreationExpression));
                Assert.IsTrue(exist, "Test failed on " + i + " iteration.");
            }
        }

        [Test]
        public void Convert_Char_StringLiteralWithoutQuotes()
        {
            var sourceCodeRep = new MemoryCodeRepository(
                @"class foo {
                    bar() {
                        obj.f1 = 'a';
                        obj.f2 = ""'b'"";
                    }
                }"
            );

            var workflow = new Workflow(sourceCodeRep, Language.Java, stage: Stage.Convert);
            var workflowResult = workflow.Process();
            var ust = workflowResult.Usts.First();

            Assert.IsTrue(ust.Root.DoesAnyDescendantMatchPredicate(ustNode =>
                ustNode is StringLiteral stringLiteral && stringLiteral.Text == "a"));
        }

        [TestCase("AllInOne.java")]
        public void Convert_Java_BaseTypesExist(string fileName)
        {
            var workflowResults = TestHelper.CheckFile(fileName, Language.Java, Stage.Convert);
            var ust = workflowResults.Usts.First();
            bool result = ust.Root.DoesAnyDescendantMatchPredicate(el =>
            {
                bool isTypeDeclaration = el.NodeType == Common.Nodes.NodeType.TypeDeclaration;
                return isTypeDeclaration && ((TypeDeclaration)el).BaseTypes.Any(t => t.TypeText == "Runnable");
            });
            Assert.IsTrue(result, "Ust doesn't contain type declaration node with Runnable base type");
        }
    }
}
