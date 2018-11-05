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
            get { return "/* This file was generated. DO NOT EDIT! */"; }
        }

        public virtual IEnumerable<string> Imports
        {
            get { yield return "Java.util.*"; }
        }

        public sealed override void Write(CodeBuilder builder)
        {
            builder.Append("package ");
            builder.Append(Namespace);
            builder.AppendLine(";");
            builder.AppendLine();
            bool hasImports = false;
            foreach (var import in Imports)
            {
                builder.Append("import ").AppendLine(import);
                hasImports = true;
            }

            if (hasImports)
                builder.AppendLine();

            GetTypeWriter().Write(builder);
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
            {
                Builder.Append(modifiers);
                Builder.Append(" ");
            }

            Builder.Append(Syntax.GetJavaTypeDeclaration());
            Builder.Append(" ");
            Builder.Append(TypeName);
            if (Syntax.BaseList != null)
                WriteTypeBaseList(Syntax.BaseList);
            using (Builder.Append(" ").BeginBlock())
            {
                WriteTypeMembers();
            }
        }

        protected abstract void WriteTypeMembers();

        private void WriteTypeBaseList(BaseListSyntax baseList)
        {
            Builder.Append(": ");

            bool first = true;
            foreach (var type in baseList.Types)
            {
                if (first)
                    first = false;
                else
                    Builder.Append(", ");

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
                Builder.Append("implements ");
            else
                Builder.Append("extends ");

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

                var kind = member.Kind();
                ISyntaxWriter writer;
                switch (kind)
                {
                    case SyntaxKind.ConstructorDeclaration:
                        writer = new ConstructorWriter(member as ConstructorDeclarationSyntax, this);
                        break;
                    case SyntaxKind.DestructorDeclaration:
                        writer = new DestructorWriter(member as DestructorDeclarationSyntax, this);
                        break;
                    case SyntaxKind.MethodDeclaration:
                        writer = new MethodWriter(member as MethodDeclarationSyntax, IsInterface, this);
                        break;
                    case SyntaxKind.PropertyDeclaration:
                        writer = new PropertyWriter(member as PropertyDeclarationSyntax, this);
                        break;
                    case SyntaxKind.IndexerDeclaration:
                        writer = new IndexerWriter(member as IndexerDeclarationSyntax, this);
                        break;
                    case SyntaxKind.FieldDeclaration:
                        writer = new FieldWriter(member as FieldDeclarationSyntax, this);
                        break;
                    case SyntaxKind.InterfaceDeclaration:
                        writer = new InterfaceTypeWriter(member as InterfaceDeclarationSyntax, this);
                        break;
                    case SyntaxKind.ClassDeclaration:
                        writer = new ClassTypeWriter(member as ClassDeclarationSyntax, this);
                        break;
                    case SyntaxKind.StructKeyword:
                        writer = new StructTypeWriter(member as StructDeclarationSyntax, this);
                        break;
                    case SyntaxKind.EnumDeclaration:
                        writer = new EnumTypeWriter(member as EnumDeclarationSyntax, this);
                        break;
                    case SyntaxKind.OperatorDeclaration:
                        // TODO
                        continue;
                    default:
                        throw new Exception();
                }

                if (writer != null)
                    writer.Write(Builder);
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
