﻿using PT.PM.Common.Nodes.GeneralScope;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PT.PM.Common.Nodes.Statements
{
    public class TypeDeclarationStatement : Statement
    {
        public override NodeType NodeType => NodeType.TypeDeclarationStatement;

        public TypeDeclaration TypeDeclaration { get;set; }

        public TypeDeclarationStatement()
        {
        }

        public TypeDeclarationStatement(TypeDeclaration typeDeclaration, TextSpan textSpan, FileNode fileNode)
            : base(textSpan, fileNode)
        {
            TypeDeclaration = typeDeclaration;
        }

        public override UstNode[] GetChildren()
        {
            var result = new[] { TypeDeclaration };
            return result;
        }
    }
}
