using CodeTranslator.Shared;
using CodeTranslator.Shared.CSharp;
using CodeTranslator.Util;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using CodeTranslator.Shared.Java;
using Microsoft.CodeAnalysis;
using System.Diagnostics;

namespace CodeTranslator.Java
{
    static partial class JavaWriterExtension
    {
        public static CodeBuilder Append(this CodeBuilder builder, ConstructorInitializerSyntax syntax, ICompilationContextProvider context)
        {
            switch (syntax.ThisOrBaseKeyword.Kind())
            {
                case SyntaxKind.ThisKeyword:
                    builder.Append("this");
                    break;
                case SyntaxKind.BaseKeyword:
                    builder.Append("super");
                    break;
                default:
                    throw new Exception();
            }

            using (builder.ParameterList())
            {
                bool first = true;
                foreach (var arg in syntax.ArgumentList.Arguments)
                {
                    if (first)
                        first = false;
                    else
                        builder.CommaSeparator();

                    builder.Append(arg.Expression, context);
                }
            }

            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, CSharpTypeParameters typeParameters, ICompilationContextProvider context)
        {
            Debug.Assert(typeParameters.Count != 0);
            using (builder.TypeParameterList(typeParameters.Count > 1))
            {
                foreach (var parameter in typeParameters)
                {
                    builder.Append(parameter.Type.Identifier.Text);
                    if (parameter.Constraints != null)
                    {
                        builder.Space();
                        writeTypeConstraints(builder, parameter.Constraints, context);
                    }

                    if (typeParameters.Count > 1)
                        builder.AppendLine();
                }
            }

            return builder;
        }

        static void writeTypeConstraints(CodeBuilder builder,
            TypeParameterConstraintClauseSyntax constraints,
            ICompilationContextProvider context)
        {
            bool first = true;
            foreach (var constraint in constraints.Constraints)
            {
                if (first)
                    first = false;
                else
                    builder.Space().Append("&").Space();

                builder.Append(constraint, context);
            }
        }

        public static CodeBuilder Append(this CodeBuilder builder,
            TypeParameterConstraintSyntax syntax, ICompilationContextProvider context)
        {
            switch (syntax.Kind())
            {
                case SyntaxKind.TypeConstraint:
                {
                    var typeContraints = syntax as TypeConstraintSyntax;
                    string javaTypeName = typeContraints.Type.GetJavaType(context, out var isInterface);

                    builder.Append(isInterface ? "implements" : "extends").Space().Append(javaTypeName);
                    break;
                }
                default:
                    throw new Exception("Unsupported type constraint");
            }

            return builder;
        }
    }
}
