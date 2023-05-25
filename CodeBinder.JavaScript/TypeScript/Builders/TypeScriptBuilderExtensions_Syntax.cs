// SPDX-FileCopyrightText: (C) 2023 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.JavaScript.TypeScript;

static partial class TypeScriptBuilderExtension
{
    public static CodeBuilder Append(this CodeBuilder builder, ConstructorInitializerSyntax syntax, bool wrapperMethod, TypeScriptCompilationContext context)
    {
        if (wrapperMethod)
        {
            var symbol = syntax.Parent!.GetDeclaredSymbol<IMethodSymbol>(context);
            builder.Append("return new").Space().Append(symbol.ContainingType.Name);
        }
        else
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
        }

        builder.Append(syntax.ArgumentList, context);
        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, CSharpTypeParameters typeParameters, TypeScriptCompilationContext context)
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
        TypeScriptCompilationContext context)
    {
        bool first = true;
        foreach (var constraint in constraints.Constraints)
            builder.AmpSeparator(ref first).Append(constraint, context);
    }

    public static CodeBuilder Append(this CodeBuilder builder,
        TypeParameterConstraintSyntax syntax, TypeScriptCompilationContext context)
    {
        switch (syntax.Kind())
        {
            case SyntaxKind.TypeConstraint:
            {
                var typeContraints = (TypeConstraintSyntax)syntax;
                string javaTypeName = typeContraints.Type.GetTypeScriptType(context, out var isInterface);
                builder.Append(isInterface ? "implements" : "extends").Space().Append(javaTypeName);
                break;
            }
            default:
                throw new Exception("Unsupported type constraint");
        }

        return builder;
    }
}
