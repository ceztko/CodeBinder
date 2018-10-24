// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator
{
    public abstract class NodeVisitor<TSyntaxTreeContext>
        where TSyntaxTreeContext : SyntaxTreeContext
    {
        public TSyntaxTreeContext Context
        {
            get; internal set;
        }

        public abstract void Visit(SyntaxNode node);
    }
}
