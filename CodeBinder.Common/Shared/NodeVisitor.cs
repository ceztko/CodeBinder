using CodeBinder.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Shared
{
    public interface INodeVisitor<TSyntaxTree>
        where TSyntaxTree : SyntaxTreeContext
    {
        void Visit(TSyntaxTree context);
    }
}
