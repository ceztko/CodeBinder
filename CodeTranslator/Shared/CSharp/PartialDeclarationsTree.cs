using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.Shared.CSharp
{
    public class PartialDeclarationsTree
    {
        public IReadOnlyList<TypeDeclarationSyntax> PartialDeclarations { get; private set; }

        public IReadOnlyDictionary<TypeDeclarationSyntax, PartialDeclarationsTree> MemberPartialDeclarations { get; private set; }

        public PartialDeclarationsTree(IReadOnlyList<TypeDeclarationSyntax> partialDeclarations,
            IReadOnlyDictionary<TypeDeclarationSyntax, PartialDeclarationsTree> memberPartialDeclarations)
        {
            PartialDeclarations = partialDeclarations;
            MemberPartialDeclarations = memberPartialDeclarations;
        }
    }
}
