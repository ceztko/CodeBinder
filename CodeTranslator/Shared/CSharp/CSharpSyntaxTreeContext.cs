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
    public class CSharpSyntaxTreeContext : SyntaxTreeContext
    {
        public List<InterfaceDeclarationSyntax> Interfaces { get; private set; }
        public List<ClassDeclarationSyntax> Classes { get; private set; }
        public List<StructDeclarationSyntax> Structs { get; private set; }
        public List<EnumDeclarationSyntax> Enums { get; private set; }

        public CSharpSyntaxTreeContext()
        {
            Interfaces = new List<InterfaceDeclarationSyntax>();
            Classes = new List<ClassDeclarationSyntax>();
            Structs = new List<StructDeclarationSyntax>();
            Enums = new List<EnumDeclarationSyntax>();

        }
    }
}
