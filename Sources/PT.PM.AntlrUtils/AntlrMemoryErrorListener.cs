﻿using Antlr4.Runtime;
using PT.PM.Common;
using PT.PM.Common.Exceptions;

namespace PT.PM.AntlrUtils
{
    public class AntlrMemoryErrorListener : IAntlrErrorListener<IToken>, IAntlrErrorListener<int>
    {
        private const int MaxErrorCodeLength = 200;
        private const string ErrorCodeSplitter = " ... ";

        public ILogger Logger { get; set; } = DummyLogger.Instance;

        public string FileName { get; set; }

        public string FileData { get; set; }

        public bool IsPattern { get; set; }

        public int LineOffset { get; set; }

        public AntlrMemoryErrorListener()
        {
        }

        public void SyntaxError(IRecognizer recognizer, int offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            var error = new AntlrLexerError(offendingSymbol, line, charPositionInLine, msg, e);
            int start = TextHelper.LineColumnToLinear(FileData, line, charPositionInLine);
            string errorText = FixLineNumber(error.ToString(), line, charPositionInLine);
            Logger.LogError(new ParsingException(FileName, message: errorText) { TextSpan = new TextSpan(start, 1), IsPattern = IsPattern });
        }

        public void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            var error = new AntlrParserError(offendingSymbol, line, charPositionInLine, msg, e);
            string errorText = FixLineNumber(error.ToString(), line, charPositionInLine);
            int start = TextHelper.LineColumnToLinear(FileData, line, charPositionInLine);
            Logger.LogError(new ParsingException(FileName, message: errorText) { TextSpan = new TextSpan(start, 1), IsPattern = IsPattern });
        }

        private string FixLineNumber(string errorText, int line, int charPositionInLine)
        {
            if (LineOffset != 0)
            {
                int atLastIndexOf = errorText.LastIndexOf("at");
                if (atLastIndexOf != -1)
                {
                    errorText = errorText.Remove(atLastIndexOf) + $"at {LineOffset + line}:{charPositionInLine}";
                }
            }

            return errorText;
        }
    }
}
