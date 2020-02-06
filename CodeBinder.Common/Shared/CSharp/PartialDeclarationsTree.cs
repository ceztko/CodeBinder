using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Shared.CSharp
{
    public class PartialDeclarationsTree
    {
        /// <summary>
        /// List of partial type declarations
        /// </summary>
        public IReadOnlyList<TypeDeclarationSyntax> PartialDeclarations { get; private set; }

        /// <summary>
        /// Children partial declaration trees
        /// </summary>
        public IReadOnlyDictionary<TypeDeclarationSyntax, PartialDeclarationsTree> MemberPartialDeclarations { get; private set; }

        public PartialDeclarationsTree(IReadOnlyList<TypeDeclarationSyntax> partialDeclarations,
            IReadOnlyDictionary<TypeDeclarationSyntax, PartialDeclarationsTree> memberPartialDeclarations)
        {
            PartialDeclarations = partialDeclarations;
            MemberPartialDeclarations = memberPartialDeclarations;
        }
    }
}
