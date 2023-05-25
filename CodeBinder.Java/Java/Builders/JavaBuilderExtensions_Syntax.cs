// Copyright(c) 2020 Francesco Pretto
// This file is subject to the MIT license
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
using System.Diagnostics;

namespace CodeBinder.Java
{
    static partial class JavaBuilderExtension
    {
        public static CodeBuilder Append(this CodeBuilder builder, ConstructorInitializerSyntax syntax, JavaCodeConversionContext context)
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

            builder.Append(syntax.ArgumentList, context);
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, CSharpTypeParameters typeParameters, JavaCodeConversionContext context)
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
            JavaCodeConversionContext context)
        {
            bool first = true;
            foreach (var constraint in constraints.Constraints)
                builder.AmpSeparator(ref first).Append(constraint, context);
        }

        public static CodeBuilder Append(this CodeBuilder builder,
            TypeParameterConstraintSyntax syntax, JavaCodeConversionContext context)
        {
            switch (syntax.Kind())
            {
                case SyntaxKind.TypeConstraint:
                {
                    var typeContraints = (TypeConstraintSyntax)syntax;
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
