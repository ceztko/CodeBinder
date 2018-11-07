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
            get { yield return "Java.util.*"; }
        }

        public sealed override void Write(CodeBuilder builder)
        {
            builder.Append("package").Space().Append(Namespace).EndOfLine();
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

        protected abstract ISyntaxWriter GetTypeWriter();
    }

    abstract class TypeWriter<TBaseType> : SyntaxWriter<TBaseType>
        where TBaseType: BaseTypeDeclarationSyntax
    {
        protected TypeWriter(TBaseType syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            var modifiers = Syntax.GetJavaModifiersString();
            if (modifiers != string.Empty)
                Builder.Append(modifiers).Space();

            Builder.Append(Syntax.GetJavaTypeDeclaration()).Space();
            Builder.Append(TypeName);
            if (Syntax.BaseList != null)
                WriteTypeBaseList(Syntax.BaseList);
            using (Builder.Space().BeginBlock())
            {
                WriteTypeMembers();
            }
        }

        protected abstract void WriteTypeMembers();

        private void WriteTypeBaseList(BaseListSyntax baseList)
        {
            Builder.Append(":").Space();

            bool first = true;
            foreach (var type in baseList.Types)
            {
                if (first)
                    first = false;
                else
                    Builder.Append(",").Space();

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
                if (first)
                    first = false;
                else
                    Builder.AppendLine();

                if (member.HasAttribute<IgnoreAttribute>(this))
                    continue;

                Builder.Append(member.GetWriter(this));
            }
        }

        public virtual bool IsInterface
        {
            get { return false; }
        }

        public virtual string TypeName
        {
            get { return Syntax.GetName(); }
        }
    }
}
