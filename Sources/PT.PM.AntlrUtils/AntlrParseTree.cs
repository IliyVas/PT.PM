﻿using PT.PM.Common;
using Antlr4.Runtime;
using System;
using System.Collections.Generic;

namespace PT.PM.AntlrUtils
{
    public abstract class AntlrParseTree : ParseTree
    {
        public IList<IToken> Tokens = new List<IToken>();

        public ParserRuleContext SyntaxTree;

        public TimeSpan LexerTimeSpan;

        public TimeSpan ParserTimeSpan;

        public IList<IToken> Comments = new List<IToken>();

        protected AntlrParseTree(ParserRuleContext syntaxTree)
        {
            SyntaxTree = syntaxTree;
        }

        public AntlrParseTree()
        {
        }
    }
}
