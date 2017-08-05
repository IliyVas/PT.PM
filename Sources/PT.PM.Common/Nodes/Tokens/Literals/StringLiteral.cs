using System;

namespace PT.PM.Common.Nodes.Tokens.Literals
{
    public class StringLiteral : Literal
    {
        public override NodeType NodeType => NodeType.StringLiteral;

        public override string TextValue => Text;

        public virtual string Text { get; set; }

        public bool IsChar { get; }

        public StringLiteral(string text, TextSpan textSpan, FileNode fileNode)
            : base(textSpan, fileNode)
        {
            Text = text;
            IsChar = false;
        }

        public StringLiteral(string text, bool isChar, TextSpan textSpan, FileNode fileNode)
            : base(textSpan, fileNode)
        {
            Text = text;
            IsChar = isChar;
        }

        public StringLiteral(string text)
        {
            Text = text;
        }

        public StringLiteral()
        {
        }

        public override int CompareTo(UstNode other)
        {
            var baseCompareResult = base.CompareTo(other);
            if (baseCompareResult != 0)
            {
                return baseCompareResult;
            }

            return String.Compare(Text, ((StringLiteral)other).Text, StringComparison.Ordinal); // TODO: Regular expressions ???
        }

        public override string ToString() => $"\"{Text}\"";
    }
}