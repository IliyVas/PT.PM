﻿using PT.PM.Common.Nodes.Expressions;
using PT.PM.Common.Nodes.Tokens;
using System.Collections.Generic;
using System.Linq;

namespace PT.PM.Common.Nodes.TypeMembers
{
    public class FieldDeclaration : EntityDeclaration
    {
        public override NodeType NodeType => NodeType.FieldDeclaration;

        public TypeToken Type { get; set; }

        public List<AssignmentExpression> Variables { get; set; }

        public FieldDeclaration(TypeToken type, IEnumerable<AssignmentExpression> variables,
            TextSpan textSpan, FileNode fileNode)
            : base(null, textSpan, fileNode)
        {
            Type = type;
            Variables = variables as List<AssignmentExpression> ?? variables.ToList();
        }

        public FieldDeclaration(IEnumerable<AssignmentExpression> variables,
            TextSpan textSpan, FileNode fileNode)
            : base(null, textSpan, fileNode)
        {
            Type = null;
            Variables = variables as List<AssignmentExpression> ?? variables.ToList();
        }

        public FieldDeclaration()
        {
        }

        public override UstNode[] GetChildren()
        {
            var result = new List<UstNode>(base.GetChildren());
            result.Add(Type);
            result.AddRange(Variables);
            return result.ToArray();
        }
    }
}
