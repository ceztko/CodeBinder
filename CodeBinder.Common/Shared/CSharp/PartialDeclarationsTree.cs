// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Shared.CSharp
{
    public class PartialDeclarationsTree
    {
        /// <summary>
        /// Root partial type declarations
        /// </summary>
        public IReadOnlyList<TypeDeclarationSyntax> RootPartialDeclarations { get; private set; }

        /// <summary>
        /// Children partial declaration trees
        /// </summary>
        public IReadOnlyDictionary<TypeDeclarationSyntax, PartialDeclarationsTree> ChildrenPartialDeclarations { get; private set; }

        public PartialDeclarationsTree(IReadOnlyList<TypeDeclarationSyntax> rootPartialDeclarations,
            IReadOnlyDictionary<TypeDeclarationSyntax, PartialDeclarationsTree> childrenPartialDeclarations)
        {
            RootPartialDeclarations = rootPartialDeclarations;
            ChildrenPartialDeclarations = childrenPartialDeclarations;
        }
    }
}
