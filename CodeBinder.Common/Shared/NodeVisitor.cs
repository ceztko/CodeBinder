using CodeBinder.Shared;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Shared
{
    // CHECK-ME Evaluate create a base NodeVisitor class
    public interface INodeVisitor
    {
        void Visit(SyntaxTree context);
    }
}
