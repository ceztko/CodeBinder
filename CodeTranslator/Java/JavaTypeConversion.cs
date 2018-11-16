// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using CodeTranslator.Attributes;
using CodeTranslator.Shared;
using CodeTranslator.Shared.CSharp;
using CodeTranslator.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace CodeTranslator.Java
{
    abstract partial class JavaTypeConversion<TTypeContext> : CSharpTypeConversion<TTypeContext, CSToJavaConversion>
        where TTypeContext : CSharpTypeContext
    {
        public string Namespace { get; private set; }
        string _Basepath;

        protected JavaTypeConversion(CSToJavaConversion conversion)
            : base(conversion)
        {
            Namespace = conversion.BaseNamespace;
            _Basepath = string.IsNullOrEmpty(Namespace) ? null : Namespace.Replace('.', Path.DirectorySeparatorChar);
        }

        public override string BasePath
        {
            get { return _Basepath; }
        }

        public override string FileName
        {
            get { return TypeContext.Node.GetName() + ".java"; }
        }

        public override string GeneratedPreamble
        {
            get { return CSToJavaConversion.GeneratedPreamble; }
        }

        public virtual IEnumerable<string> Imports
        {
            get
            {
                yield return "Java.util.*";
                yield return "codetranslator.utils";
            }
        }

        public sealed override void Write(CodeBuilder builder)
        {
            builder.Append("package").Space().Append(Namespace).EndOfStatement();
            bool hasImports = false;
            foreach (var import in Imports)
            {
                builder.Append("import").Space().AppendLine(import);
                hasImports = true;
            }

            if (hasImports)
                builder.AppendLine();

            builder.Append(GetTypeWriter());
        }

        protected abstract ContextWriter GetTypeWriter();
    }

    abstract class TypeWriter<TBaseType> : ContextWriter<TBaseType>
        where TBaseType: BaseTypeDeclarationSyntax
    {
        protected TypeWriter(TBaseType syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            var modifiers = Context.GetJavaModifiersString();
            if (!string.IsNullOrEmpty(modifiers))
                Builder.Append(modifiers).Space();

            Builder.Append(Context.GetJavaTypeDeclaration()).Space();
            Builder.Append(TypeName);
            if (Arity > 0)
            {
                Builder.Space();
                WriteTypeParameters();
            }

            if (Context.BaseList != null)
            {
                Builder.Space();
                WriteTypeBaseList(Context.BaseList);
            }
            Builder.AppendLine();
            using (Builder.Block())
            {
                WriteTypeMembers();
            }
        }

        protected virtual void WriteTypeParameters() { }

        protected abstract void WriteTypeMembers();

        private void WriteTypeBaseList(BaseListSyntax baseList)
        {
            bool first = true;
            foreach (var type in baseList.Types)
            {
                if (first)
                    first = false;
                else
                    Builder.CommaSeparator();

                WriteBaseType(type);
            }
        }

        private void WriteBaseType(BaseTypeSyntax type)
        {
            string typeName;
            bool isInterface;
            if (!IsKnownType(type, out typeName, out isInterface))
            {
                typeName = type.GetName();
                var typeInfo = type.GetTypeInfo(this);
                isInterface = typeInfo.ConvertedType.TypeKind == TypeKind.Interface;
            }

            if (isInterface)
                Builder.Append("implements").Space();
            else
                Builder.Append("extends").Space();

            Builder.Append(typeName);
        }

        private bool IsKnownType(BaseTypeSyntax type, out string convertedKnowType, out bool isInterface)
        {
            var fullname = type.GetFullName(this);
            switch (fullname)
            {
                case "System.IDisposable":
                {
                    convertedKnowType = "AutoCloseable";
                    isInterface = true;
                    return true;
                }
                default:
                {
                    convertedKnowType = null;
                    isInterface = false;
                    return false;
                }
            }
        }

        protected void WriteTypeMembers(SyntaxList<MemberDeclarationSyntax> members)
        {
            bool first = true;
            foreach (var member in members)
            {
                if (member.HasAttribute<IgnoreAttribute>(this))
                    continue;

                foreach (var writer in member.GetWriters(this))
                {
                    if (first)
                        first = false;
                    else
                        Builder.AppendLine();

                    Builder.Append(writer);
                }
            }
        }

        protected void WriteTypeParameters(TypeParameterListSyntax typeParameterList,
            SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses)
        {
            var merged = mergeTypeConstraint(typeParameterList.Parameters, constraintClauses);
            using (Builder.TypeParameterList())
            {
                bool first = true;
                foreach (var pair in merged)
                {
                    if (first)
                        first = true;
                    else
                        Builder.AppendLine();

                    Builder.Append(pair.Type.Identifier.Text);
                    if (pair.Constraints != null)
                    {
                        Builder.Space();
                        writeTypeConstraints(pair.Constraints);
                    }
                }
            }
        }

        private void writeTypeConstraints(TypeParameterConstraintClauseSyntax constraints)
        {
            bool first = true;
            foreach (var constraint in constraints.Constraints)
            {
                if (first)
                    first = false;
                else
                    Builder.Space().Append("&").Space();

                Builder.Append(constraint, this);
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

        public virtual int Arity
        {
            get { return 0; }
        }

        public virtual bool IsInterface
        {
            get { return false; }
        }

        public virtual string TypeName
        {
            get { return Context.GetName(); }
        }
    }
}
