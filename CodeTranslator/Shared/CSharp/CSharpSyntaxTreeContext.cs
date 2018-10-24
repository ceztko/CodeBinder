// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeTranslator.Shared.CSharp
{
    public class CSharpSyntaxTreeContext : RoslynSyntaxTreeContext
    {
        public List<ClassDeclarationSyntax> Classes { get; private set; }
        public List<EnumDeclarationSyntax> Enums { get; private set; }

        public CSharpSyntaxTreeContext()
        {
            Classes = new List<ClassDeclarationSyntax>();
            Enums = new List<EnumDeclarationSyntax>();
        }
    }
}
