// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
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
    abstract partial class JavaTypeConversion<TTypeContext> : CSharpTypeConversion<TTypeContext>, IJavaConversion
        where TTypeContext : CSharpTypeContext
    {
        string _Namespace;
        string _Basepath;

        public string Namespace
        {
            get { return _Namespace; }
            set
            {
                _Namespace = value;
                _Basepath = value.Replace('.', Path.DirectorySeparatorChar);
            }
        }

        public override string BasePath
        {
            get { return _Basepath; }
        }

        public override string FileName
        {
            get { return TypeName + ".java"; }
        }

        public virtual string TypeName
        {
            get { return TypeContext.Node.GetName(); }
        }

        public override string GeneratedPreamble
        {
            get { return "/* This file was generated. DO NOT EDIT! */"; }
        }

        public virtual IEnumerable<string> Imports
        {
            get { yield break; }
        }

        public sealed override void Write(IndentStringBuilder builder)
        {
            builder.Append("package ");
            builder.Append(Namespace);
            builder.AppendLine(";");
            builder.AppendLine();
            bool hasImports = false;
            foreach (var import in Imports)
            {
                builder.AppendLine();
                hasImports = true;
            }

            if (hasImports)
                builder.AppendLine();

            WriteType(builder, TypeContext.Node);
        }

        private void WriteType(IndentStringBuilder builder, BaseTypeDeclarationSyntax type)
        {
            var modifiers = type.GetJavaModifiersString();
            if (modifiers != string.Empty)
            {
                builder.Append(modifiers);
                builder.Append(" ");
            }

            builder.Append(type.GetJavaTypeDeclaration());
            builder.Append(" ");
            builder.Append(type.GetName());
            if (type.BaseList != null)
                WriteTypeBaseList(builder, type.BaseList);
            builder.AppendLine(" {");
            using (builder = builder.Indent())
            {
                WriteTypeMembers(builder, type);
            }

            builder.AppendLine("}");
        }

        protected virtual void WriteTypeMembers(IndentStringBuilder builder, BaseTypeDeclarationSyntax type)
        {
            WriteTypeMembers(builder, (type as TypeDeclarationSyntax).Members);
        }

        private void WriteTypeBaseList(IndentStringBuilder builder, BaseListSyntax baseList)
        {
            builder.Append(": ");

            bool first = true;
            foreach (var type in baseList.Types)
            {
                if (first)
                    first = false;
                else
                    builder.Append(", ");

                WriteBaseType(builder, type);
            }
        }

        private void WriteBaseType(IndentStringBuilder builder, BaseTypeSyntax type)
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
                builder.Append("implements ");
            else
                builder.Append("extends ");

            builder.Append(typeName);
        }

        private bool IsKnownType(BaseTypeSyntax type, out string convertedKnowType, out bool isInterface)
        {
            var fullname = type.GetFullMetadataName(this);
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

        private void WriteTypeMembers(IndentStringBuilder builder, SyntaxList<MemberDeclarationSyntax> members)
        {
            bool first = true;
            foreach (var member in members)
            {
                if (first)
                    first = false;
                else
                    builder.AppendLine();

                var kind = member.Kind();
                switch (kind)
                {
                    case SyntaxKind.ConstructorDeclaration:
                        new ConstructorWriter(member as ConstructorDeclarationSyntax, this).Write(builder);
                        break;
                    case SyntaxKind.DestructorDeclaration:
                        new DestructorWriter(member as DestructorDeclarationSyntax, this).Write(builder);
                        break;
                    case SyntaxKind.MethodDeclaration:
                        new MethodWriter(member as MethodDeclarationSyntax, this).Write(builder);
                        break;
                    case SyntaxKind.PropertyDeclaration:
                    case SyntaxKind.IndexerDeclaration:
                        WriteProperty(builder, member as BasePropertyDeclarationSyntax);
                        break;
                    case SyntaxKind.FieldDeclaration:
                        WriteField(builder, member as FieldDeclarationSyntax);
                        break;
                    case SyntaxKind.InterfaceDeclaration:
                    case SyntaxKind.ClassDeclaration:
                    case SyntaxKind.StructKeyword:
                    case SyntaxKind.EnumDeclaration:
                        WriteType(builder, member as BaseTypeDeclarationSyntax);
                        break;
                    default:
                        throw new Exception();
                }
            }
        }

        void WriteField(IndentStringBuilder builder, FieldDeclarationSyntax field)
        {

        }

        void WriteProperty(IndentStringBuilder builder, BasePropertyDeclarationSyntax property)
        {

        }
    }

    public interface IJavaConversion
    {
        string Namespace { get; set; }
    }
}
