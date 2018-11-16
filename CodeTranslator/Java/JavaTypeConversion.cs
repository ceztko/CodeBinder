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

        protected abstract CodeWriter GetTypeWriter();
    }

    abstract class TypeWriter<TBaseType> : CodeWriter<TBaseType>
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
                WriteBaseTypes(Context.BaseList);
            }
            Builder.AppendLine();
            using (Builder.Block())
            {
                WriteTypeMembers();
            }
        }

        protected virtual void WriteTypeParameters() { }

        protected abstract void WriteTypeMembers();

        private void WriteBaseTypes(BaseListSyntax baseList)
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
            string javaTypeName = type.Type.GetJavaType(this, out var isInterface);
            Builder.Append(isInterface ? "implements" : "extends").Space().Append(javaTypeName);
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
