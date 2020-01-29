using CodeBinder.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Shared
{
    // CHEC-ME: Evalaute making a NodeVisitor class offering all methods of CSharpSyntaxWalker and exposing it lower level classes
    public interface INodeVisitor<TSyntaxTree>
        where TSyntaxTree : SyntaxTreeContext
    {
        void Visit(TSyntaxTree context);
    }
}
