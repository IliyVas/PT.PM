﻿using NUnit.Framework;
using PT.PM.Common;
using PT.PM.Common.CodeRepository;
using PT.PM.Common.Nodes;
using PT.PM.Common.Nodes.Expressions;
using PT.PM.Common.Nodes.Statements;
using PT.PM.Common.Nodes.Tokens;
using PT.PM.Common.Nodes.Tokens.Literals;
using PT.PM.Dsl;
using PT.PM.Matching;
using PT.PM.Matching.Patterns;
using System.Collections.Generic;
using System.Linq;

namespace PT.PM.Tests
{
    [TestFixture]
    public class UstPreprocessorTests
    {
        [Test]
        public void Preprocess_PhpCodeWithConstants_ConstantsFolded()
        {
            var sourceCodeRep = new MemoryCodeRepository(
                "<?php\r\n" +
                "echo 'Hello ' . 'World' . '!';\r\n" +
                "echo 60 * 60 * 24;\r\n" +
                "echo 6 + 6 * 6;\r\n" +
                "$a = -3;\r\n" +
                "$b = -3.1;"
            );
            var logger = new LoggerMessageCounter();
            var workflow = new Workflow(sourceCodeRep, Language.Php, stage: Stage.Preprocess);
            workflow.IsIncludePreprocessing = true;
            workflow.Logger = logger;
            workflow.Process();

            Assert.IsTrue(logger.ContainsDebugMessagePart("Hello World!"));
            Assert.IsTrue(logger.ContainsDebugMessagePart("86400"));
            Assert.IsTrue(logger.ContainsDebugMessagePart("42"));
            Assert.IsTrue(logger.ContainsDebugMessagePart("-3"));
            Assert.IsTrue(logger.ContainsDebugMessagePart("-3.1"));
        }

        [Test]
        public void Preprocess_JavaCodeWithConstantCharArray_ArrayFolded()
        {
            var sourceCodeRep = new MemoryCodeRepository(
                "class Wrapper {\r\n" +
                "  public void init() {\r\n" +
                "    char[] array = { 'n', 'o', 'n', 'e' };\r\n" +
                "  }\r\n" +
                "}"
            );

            var workflow = new Workflow(sourceCodeRep, Language.Java, stage: Stage.Preprocess);
            workflow.IsIncludePreprocessing = true;
            var ust = workflow.Process().Usts.First();

            Assert.IsTrue(ust.DoesAnyDescendantMatchPredicate(
                node => node is StringLiteral str && str.Text == "none"));
        }

        [Test]
        public void Preprocess_MultiMultiPattern_RemovedDuplicates()
        {
            Ust patternWithDuplicateMultiStatementsExpressions = new PatternStatements
            {
                Statements = new List<PatternBase>()
                {
                    new PatternInvocationExpression
                    {
                        Target = new PatternIdToken("test_call"),
                        Arguments = new PatternArgs
                        (
                            new PatternIdToken("a"),
                            new PatternIdToken("b"),
                            new PatternMultipleExpressions(),
                            new PatternMultipleExpressions(),
                            new PatternIdToken("z")
                        )
                    },

                    new PatternVarOrFieldDeclaration
                    {
                        Type = new PatternIdToken("int"),
                        Assignment = new PatternAssignmentExpression
                        {
                            Left = new PatternIdToken("a"),
                            Right = new PatternIntLiteral(42)
                        }
                    }
                }
            };
            var logger = new LoggerMessageCounter();
            var processor = new DslProcessor();
            UstSimplifier preprocessor = new UstSimplifier() { Logger = logger };
            Ust result = preprocessor.Preprocess(patternWithDuplicateMultiStatementsExpressions);

            Assert.AreEqual(1, result.GetAllDescendants().Count(child => child is PatternMultipleExpressions));
        }

        [Test]
        public void Sort_PatternVars_CorrectOrder()
        {
            var unsorted = new PatternOr
            (
                new PatternStringLiteral("42"),
                new PatternIntLiteral(100),
                new PatternIntLiteral(42),
                new PatternIntLiteral(0),
                new PatternStringLiteral("42"),
                new PatternNot(new PatternStringLiteral("42")),
                new PatternStringLiteral("Hello World!"),
                new PatternIdToken("testId"),
                new PatternIdToken("42"),
                new PatternNot(new PatternStringLiteral("42"))
            );
            var expectedSorted = new PatternOr
            (
                new PatternIdToken("testId"),
                new PatternIdToken("42"),
                new PatternStringLiteral("42"),
                new PatternStringLiteral("42"),
                new PatternStringLiteral("Hello World!"),
                new PatternNot(new PatternStringLiteral("42")),
                new PatternNot(new PatternStringLiteral("42")),
                new PatternIntLiteral(100),
                new PatternIntLiteral(42),
                new PatternIntLiteral(0)
            );

            var logger = new LoggerMessageCounter();
            var processor = new DslProcessor();
            UstSimplifier preprocessor = new UstSimplifier() { Logger = logger };

            var actualPattern = (PatternOr)preprocessor.Preprocess(unsorted);
            List<PatternBase> actualAlternatives = actualPattern.Patterns;
            List<PatternBase> expectedAlternatives = expectedSorted.Patterns;

            Assert.AreEqual(expectedAlternatives.Count, actualAlternatives.Count);
            for (int i = 0; i < expectedAlternatives.Count; i++)
            {
                Assert.IsTrue(expectedAlternatives[i].Equals(actualAlternatives[i]),
                    $"Not equal at {i} index: expected {expectedAlternatives[i]} not equals to {actualAlternatives[i]}");
            }
        }
    }
}
