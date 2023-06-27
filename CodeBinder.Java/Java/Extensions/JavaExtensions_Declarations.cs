// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT
using CodeBinder.Shared;
using CodeBinder.Shared.CSharp;
using CodeBinder.Utils;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace CodeBinder.Java
{
    static partial class JavaExtensions
    {
        public static IEnumerable<CodeWriter> GetWriters(this MemberDeclarationSyntax member,
            PartialDeclarationsTree partialDeclarations, JavaCodeConversionContext context)
        {
            var kind = member.Kind();
            switch (kind)
            {
                case SyntaxKind.ConstructorDeclaration:
                    return getConstructorWriters((ConstructorDeclarationSyntax)member, context);
                case SyntaxKind.DestructorDeclaration:
                    return new[] { new JavaDestructorWriter((DestructorDeclarationSyntax)member, context) };
                case SyntaxKind.MethodDeclaration:
                    return getMethodWriters((MethodDeclarationSyntax)member, context);
                case SyntaxKind.PropertyDeclaration:
                    return new[] { new JavaPropertyWriter((PropertyDeclarationSyntax)member, context) };
                case SyntaxKind.IndexerDeclaration:
                    return new[] { new JavaIndexerWriter((IndexerDeclarationSyntax)member, context) };
                case SyntaxKind.FieldDeclaration:
                    return new[] { new JavaFieldWriter((FieldDeclarationSyntax)member, context) };
                case SyntaxKind.InterfaceDeclaration:
                    var iface = (InterfaceDeclarationSyntax)member;
                    return new[] { new JavaInterfaceWriter(iface, partialDeclarations.ChildrenPartialDeclarations[iface], context) };
                case SyntaxKind.ClassDeclaration:
                    var cls = (ClassDeclarationSyntax)member;
                    return new[] { new JavaClassWriter(cls, partialDeclarations.ChildrenPartialDeclarations[cls], context) };
                case SyntaxKind.StructDeclaration:
                    var structure = (StructDeclarationSyntax)member;
                    return new[] { new JavaStructWriter(structure, partialDeclarations.ChildrenPartialDeclarations[structure], context) };
                case SyntaxKind.EnumDeclaration:
                    return new[] { new JavaEnumWriter((EnumDeclarationSyntax)member, context) };
                default:
                    throw new Exception();
            }
        }

        static IEnumerable<CodeWriter> getMethodWriters(MethodDeclarationSyntax method, JavaCodeConversionContext context)
        {
            if (method.GetCSharpModifiers().Contains(SyntaxKind.PartialKeyword) && method.Body == null)
                yield break;

            IMethodSymbol symbol;
            if (method.Identifier.Text == "FreeHandle")
            {
                symbol = method.GetDeclaredSymbol<IMethodSymbol>(context);
                if (symbol.OverriddenMethod?.ContainingType.GetFullName() == "CodeBinder.HandledObjectBase")
                {
                    yield return new CreateFinalizerMethodWriter(symbol.ContainingType);
                    yield return new ClassFinalizerWriter(method.Body!, symbol.ContainingType, context);
                    yield break;
                }
            }

            for (int i = method.ParameterList.Parameters.Count - 1; i >= 0; i--)
            {
                var parameter = method.ParameterList.Parameters[i];
                if (parameter.Default == null)
                    break;

                yield return new MethodWriter(method, i, context);
            }

            yield return new MethodWriter(method, -1, context);
        }

        static IEnumerable<CodeWriter> getConstructorWriters(ConstructorDeclarationSyntax method, JavaCodeConversionContext context)
        {
            for (int i = method.ParameterList.Parameters.Count - 1; i >= 0; i--)
            {
                var parameter = method.ParameterList.Parameters[i];
                if (parameter.Default == null)
                    break;

                yield return new ConstructorWriter(method, i, context);
            }

            yield return new ConstructorWriter(method, -1, context);
        }

        class CreateFinalizerMethodWriter : CodeWriter
        {
            ITypeSymbol _finalizableType;

            public CreateFinalizerMethodWriter(ITypeSymbol finalizableType)
            {
                _finalizableType = finalizableType;
            }

            protected override void Write()
            {
                Builder.AppendLine($$"""
protected HandledObjectFinalizer createFinalizer()
{
    return new {{_finalizableType.Name}}Finalizer();
}
""");
            }
        }

        class ClassFinalizerWriter : CodeWriter
        {
            BlockSyntax _block;
            ITypeSymbol _finalizableType;
            JavaCodeConversionContext _context;

            public ClassFinalizerWriter(BlockSyntax block, ITypeSymbol finalizableType, JavaCodeConversionContext context)
            {
                _block = block;
                _finalizableType = finalizableType;
                _context = context;
            }

            protected override void Write()
            {
                Builder.AppendLine($"static class {_finalizableType.Name}Finalizer extends HandledObjectFinalizer");
                using (Builder.Block())
                {
                    Builder.AppendLine("public void freeHandle(long handle)");
                    using (Builder.Block())
                    {
                        Builder.Append(_block, _context, true).AppendLine();
                    }
                }
            }
        }
    }
}
