// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
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
    abstract partial class JavaTypeConversion<TTypeContext> : CSharpTypeConversion<TTypeContext>
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
            get { return TypeContext.Node.GetName() + ".java"; }
        }

        public override string GeneratedPreamble
        {
            get { return "/* This file was generated. DO NOT EDIT! */"; }
        }

        public virtual IEnumerable<string> Imports
        {
            get { yield break; }
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
                builder.AppendLine();
                hasImports = true;
            }

            if (hasImports)
                builder.AppendLine();

            GetTypeWriter().Write(builder);
        }

        protected abstract TypeWriter GetTypeWriter();
    }

    abstract class TypeWriter : BaseWriter
    {
        protected TypeWriter(ISemanticModelProvider context)
            : base(context) { }

        protected override void Write()
        {
            var modifiers = Type.GetJavaModifiersString();
            if (modifiers != string.Empty)
            {
                Builder.Append(modifiers);
                Builder.Append(" ");
            }

            Builder.Append(Type.GetJavaTypeDeclaration());
            Builder.Append(" ");
            Builder.Append(TypeName);
            if (Type.BaseList != null)
                WriteTypeBaseList(Type.BaseList);
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

        protected void WriteTypeMembers(SyntaxList<MemberDeclarationSyntax> members)
        {
            bool first = true;
            foreach (var member in members)
            {
                if (first)
                    first = false;
                else
                    Builder.AppendLine();

                var kind = member.Kind();
                switch (kind)
                {
                    case SyntaxKind.ConstructorDeclaration:
                        new ConstructorWriter(member as ConstructorDeclarationSyntax, this).Write(Builder);
                        break;
                    case SyntaxKind.DestructorDeclaration:
                        new DestructorWriter(member as DestructorDeclarationSyntax, this).Write(Builder);
                        break;
                    case SyntaxKind.MethodDeclaration:
                        new MethodWriter(member as MethodDeclarationSyntax, this).Write(Builder);
                        break;
                    case SyntaxKind.PropertyDeclaration:
                    case SyntaxKind.IndexerDeclaration:
                        WriteProperty(member as BasePropertyDeclarationSyntax);
                        break;
                    case SyntaxKind.FieldDeclaration:
                        WriteField(member as FieldDeclarationSyntax);
                        break;
                    case SyntaxKind.InterfaceDeclaration:
                        new InterfaceTypeWriter(member as InterfaceDeclarationSyntax, this).Write(Builder);
                        break;
                    case SyntaxKind.ClassDeclaration:
                        new ClassTypeWriter(member as ClassDeclarationSyntax, this).Write(Builder);
                        break;
                    case SyntaxKind.StructKeyword:
                        new StructTypeWriter(member as StructDeclarationSyntax, this).Write(Builder);
                        break;
                    case SyntaxKind.EnumDeclaration:
                        new EnumTypeWriter(member as EnumDeclarationSyntax, this).Write(Builder);
                        break;
                    default:
                        throw new Exception();
                }
            }
        }

        void WriteField(FieldDeclarationSyntax field)
        {

        }

        void WriteProperty(BasePropertyDeclarationSyntax property)
        {

        }

        public virtual string TypeName
        {
            get { return Type.GetName(); }
        }

        public BaseTypeDeclarationSyntax Type
        {
            get { return GetBaseType(); }
        }

        protected abstract BaseTypeDeclarationSyntax GetBaseType();
    }

    abstract class TypeWriter<TBaseType> : TypeWriter
        where TBaseType : BaseTypeDeclarationSyntax
    {
        public new TBaseType Type { get; private set; }

        protected TypeWriter(TBaseType type, ISemanticModelProvider context)
            : base(context)
        {
            Type = type;
        }

        protected override BaseTypeDeclarationSyntax GetBaseType()
        {
            return Type;
        }
    }
}
