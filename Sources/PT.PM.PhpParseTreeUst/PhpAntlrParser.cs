﻿using PT.PM.AntlrUtils;
using PT.PM.Common;
using PT.PM.PhpParseTreeUst;
using Antlr4.Runtime;
using System.IO;

namespace PT.PM.UstParsing
{
    public class PhpAntlrParser : AntlrParser
    {
        public override Language Language => Language.Php;

        public override CaseInsensitiveType CaseInsensitiveType => CaseInsensitiveType.lower;

        public PhpAntlrParser()
        {
        }

        protected override int CommentsChannel => PhpLexer.PhpComments;

        protected override IVocabulary Vocabulary => PhpLexer.DefaultVocabulary;

        protected override Lexer InitLexer(ICharStream inputStream)
        {
            return new PhpLexer(inputStream);
        }

        protected override Parser InitParser(ITokenStream inputStream)
        {
            return new PhpParser(inputStream);
        }

        protected override ParserRuleContext Parse(Parser parser)
        {
            return ((PhpParser)parser).htmlDocument();
        }

        protected override AntlrParseTree Create(ParserRuleContext syntaxTree)
        {
            return new PhpAntlrParseTree((PhpParser.HtmlDocumentContext)syntaxTree);
        }

        protected override string PreprocessText(SourceCodeFile file)
        {
            var result = base.PreprocessText(file);

            bool trimmed = false;
            var vtIndex = result.IndexOf('\v');
            if (vtIndex != -1 && vtIndex > 0 && result[vtIndex - 1] == '\n' &&
                vtIndex + 1 < result.Length && char.IsDigit(result[vtIndex + 1]))
            {
                result = result.Remove(vtIndex);
                trimmed = true;
            }
            // TODO: Fix Hardcode!
            int lastPhpInd = result.LastIndexOf("?>");
            if (lastPhpInd != -1)
            {
                if (lastPhpInd + "?>".Length + 12 <= result.Length &&
                    result.Substring(lastPhpInd + "?>".Length, 12) == "\r\nChangelog:")
                {
                    result = result.Remove(lastPhpInd + "?>".Length);
                    trimmed = true;
                }
            }

            if (trimmed)
            {
                Logger.LogDebug($"File {Path.Combine(file.RelativePath, file.Name)} has been trimmed.");
            }

            return result;
        }
    }
}
