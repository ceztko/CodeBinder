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
using System.Diagnostics;

namespace CodeBinder.Apple
{
    static partial class ObjCBuilderExtension
    {
        public static CodeBuilder Append(this CodeBuilder builder, CSharpTypeParameters typeParameters, ObjCCompilationContext context)
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
            ObjCCompilationContext context)
        {
            string? classType = null;
            var protocols = new List<string>();
            foreach (var constraint in constraints.Constraints)
            {
                var typeInfo = (constraint as TypeConstraintSyntax)!.Type.GetObjCTypeInfo(context);
                if (typeInfo.Kind == ObjCTypeKind.Protocol)
                    protocols.Add(typeInfo.TypeName);
                else if (typeInfo.Kind == ObjCTypeKind.Class && classType == null)
                    classType = typeInfo.TypeName;
                else
                    throw new NotSupportedException();
            }

            if (classType == null)
                classType = "NSObject";

            builder.Append(":").Space().Append(classType);

            if (protocols.Count != 0)
            {
                builder.Append("<");
                bool first = true;

                foreach (var iface in protocols)
                {
                    if (first)
                        first = false;
                    else
                        builder.CommaSeparator();

                    builder.Append(iface);
                }

                builder.Append(">");
            }

            builder.Space().Append("*");
        }
    }
}
