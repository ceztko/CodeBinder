// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT
using CodeBinder.Attributes;
using CodeBinder.Shared;
using CodeBinder.Shared.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace CodeBinder.Apple
{
    // CHECK-ME Evaluate if it's possible to have an ObjCBaseTypeContext that inheirts CSharpBaseTypeContext
    // and it's a base type for all ObjC types
    class ObjCNodeVisitor : CSharpNodeVisitor<ObjCCompilationContext, CSharpMemberTypeContext, ConversionCSharpToObjC>
    {
        public ObjCNodeVisitor(ObjCCompilationContext compilation)
            : base(compilation) { }

        public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            // TODO Support it (easy, create protocols with all methods of inherited protocols)
            if (node.BaseList != null)
                AddError($"ObjectiveC: Interface {node.Identifier.Text} can't inherit other interfaces");

            base.VisitInterfaceDeclaration(node);
        }

        public override void VisitDelegateDeclaration(DelegateDeclarationSyntax node)
        {
            // CHECK-ME: Shoukd be done in base CSharpNodeVisitor for all types 
            if (node.ShouldDiscard(Compilation))
                return;

            base.VisitDelegateDeclaration(node);
        }

        protected override string GetMethodBaseName(IMethodSymbol symbol)
        {
            if (symbol.MethodKind == MethodKind.Constructor)
                return "init";
            else
                return base.GetMethodBaseName(symbol);
        }
    }
}
