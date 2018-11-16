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

namespace CodeTranslator.Java
{
    static partial class JavaWriterExtension
    {
        public static void Append(this CodeBuilder builder,
            TypeParameterListSyntax typeParameterList,
            SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses,
            ICompilationContextProvider context)
        {
            var merged = mergeTypeConstraint(typeParameterList.Parameters, constraintClauses);
            using (builder.TypeParameterList())
            {
                bool first = true;
                foreach (var pair in merged)
                {
                    if (first)
                        first = true;
                    else
                        builder.AppendLine();

                    builder.Append(pair.Type.Identifier.Text);
                    if (pair.Constraints != null)
                    {
                        builder.Space();
                        writeTypeConstraints(builder, pair.Constraints, context);
                    }
                }
            }
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

        private static (TypeParameterSyntax Type, TypeParameterConstraintClauseSyntax Constraints)[] mergeTypeConstraint(
            SeparatedSyntaxList<TypeParameterSyntax> typeParameters,
            SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses)
        {
            var ret = new (TypeParameterSyntax Type, TypeParameterConstraintClauseSyntax Constraint)[typeParameters.Count];
            for (int i = 0; i < typeParameters.Count; i++)
            {
                var type = typeParameters[i];
                var constraints = constraintClauses.FirstOrDefault((element) => element.Name.Identifier.Text == type.Identifier.Text);
                ret[i] = (type, constraints);
            }
            return ret;
        }

        public static CodeBuilder Append(this CodeBuilder builder, TypeParameterConstraintSyntax syntax, ICompilationContextProvider context)
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

        public static CodeBuilder EndOfStatement(this CodeBuilder builder)
        {
            return builder.AppendLine(";");
        }

        public static CodeBuilder SemiColonSeparator(this CodeBuilder builder)
        {
            return builder.Append("; ");
        }

        public static CodeBuilder Colon(this CodeBuilder builder)
        {
            return builder.Append(":");
        }

        public static CodeBuilder SemiColon(this CodeBuilder builder)
        {
            return builder.Append(";");
        }

        public static CodeBuilder CommaSeparator(this CodeBuilder builder)
        {
            return builder.Append(", ");
        }

        public static CodeBuilder Space(this CodeBuilder builder)
        {
            return builder.Append(" ");
        }

        public static CodeBuilder Parenthesized(this CodeBuilder builder, Action parenthesized)
        {
            builder.Append("(");
            parenthesized();
            return builder.Append(")");
        }

        public static CodeBuilder Parenthesized(this CodeBuilder builder)
        {
            builder.Append("(");
            return builder.UsingChild(")");
        }

        public static CodeBuilder Block(this CodeBuilder builder, bool appendLine = true)
        {
            builder.AppendLine("{");
            return builder.Indent("}", appendLine);
        }

        public static CodeBuilder ParameterList(this CodeBuilder builder)
        {
            builder.AppendLine("(");
            return builder.Indent(")");
        }

        public static CodeBuilder TypeParameterList(this CodeBuilder builder)
        {
            builder.AppendLine("<");
            return builder.Indent(">");
        }
    }
}
