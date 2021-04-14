// Copyright(c) 2020 Francesco Pretto
// This file is subject to the MIT license
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
    class ObjCNodeVisitor : CSharpNodeVisitor<ObjCCompilationContext, CSharpBaseTypeContext, ConversionCSharpToObjC>
    {
        static Dictionary<string, List<IMethodSymbol>> _uniqueMethodNames;

        static ObjCNodeVisitor()
        {
            _uniqueMethodNames = new Dictionary<string, List<IMethodSymbol>>();
        }

        public ObjCNodeVisitor(ObjCCompilationContext compilation)
            : base(compilation) { }

        public override void VisitEnumDeclaration(EnumDeclarationSyntax node)
        {
            base.VisitEnumDeclaration(node);
            Compilation.AddEnum(node);
        }

        public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            // TODO Support it (easy, create protocols with all methods of inherited protocols)
            if (node.BaseList != null)
                AddError($"ObjectiveC: Interface {node.Identifier.Text} can't inherit other interfaces");

            base.VisitInterfaceDeclaration(node);
            Compilation.AddInterface(node);
        }

        public override void VisitStructDeclaration(StructDeclarationSyntax node)
        {
            base.VisitStructDeclaration(node);
            Compilation.AddClass(node);
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            base.VisitClassDeclaration(node);
            Compilation.AddClass(node);
        }

        public override void VisitDelegateDeclaration(DelegateDeclarationSyntax node)
        {
            if (node.ShouldDiscard(Compilation))
                return;

            Compilation.AddCallback(node);
            base.VisitDelegateDeclaration(node);
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            if (!(node.IsPartialMethod(out var hasEmptyBody) && hasEmptyBody))
                AddMethodBinding(node);

            base.VisitMethodDeclaration(node);
        }

        public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            AddMethodBinding(node);
            base.VisitConstructorDeclaration(node);
        }

        // Construct a method binding to handle overloaded methods/constructors
        void AddMethodBinding(BaseMethodDeclarationSyntax method)
        {
            var methodSymbol = method.GetDeclaredSymbol<IMethodSymbol>(this);
            string? suffix = null;
            if (methodSymbol.TryGetAttribute<OverloadSuffixAttribute>(out var suffixAttrib))
            {
                suffix = suffixAttrib.GetConstructorArgument<string>(0);
            }

            if (suffix == null && methodSymbol.OverriddenMethod != null)
            {
                if (methodSymbol.OverriddenMethod.TryGetAttribute<OverloadSuffixAttribute>(out suffixAttrib))
                    suffix = suffixAttrib.GetConstructorArgument<string>(0);
            }

            string methodName;
            if (methodSymbol.MethodKind == MethodKind.Constructor)
                methodName = "init";
            else
                methodName = methodSymbol.Name;

            if (methodSymbol.ExplicitInterfaceImplementations.Length != 0)
            {
                // Get name of explicitly interface implemented method
                methodName = methodSymbol.ExplicitInterfaceImplementations[0].Name;
            }

            string bindedName = methodName + suffix;
            string qualifiedBindedName = $"{methodSymbol.ContainingType.GetFullName()}.{bindedName}";
            List<IMethodSymbol>? bindedMethods;
            if (_uniqueMethodNames.TryGetValue(qualifiedBindedName, out bindedMethods))
            {
                bool doParameterOverlaps = false;
                foreach (var bindendMethodSymbol in bindedMethods)
                {
                    doParameterOverlaps = methodSymbol.DoParameterCountOverlap(bindendMethodSymbol);
                    if (doParameterOverlaps)
                    {
                        AddError("Method " + methodSymbol.GetDebugName()
                            + " parameter count overlap with method " + bindendMethodSymbol.GetDebugName());
                        break;
                    }
                }

                if (!doParameterOverlaps)
                    bindedMethods.Add(methodSymbol);
            }
            else
            {
                bindedMethods = new List<IMethodSymbol>();
                bindedMethods.Add(methodSymbol);
                _uniqueMethodNames.Add(qualifiedBindedName, bindedMethods);
            }

            Compilation.AddMethodBinding(methodSymbol, bindedName);
        }
    }
}
