// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using CodeTranslator.Shared.CSharp;
using CodeTranslator.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CodeTranslator.Java
{
    abstract class JavaTypeConversion<TTypeContext> : CSharpTypeConversion<TTypeContext>, IJavaConversion
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

        public abstract string TypeDeclaration
        {
            get;
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

            WriteType(builder);
        }

        public void WriteType(IndentStringBuilder builder)
        {
            var modifiers = TypeContext.Node.GetJavaModifiersString();
            if (modifiers != string.Empty)
            {
                builder.Append(modifiers);
                builder.Append(" ");
            }

            builder.Append(TypeDeclaration);
            builder.Append(" ");
            builder.Append(TypeName);
            WriteTypeBaseList(builder);
            builder.AppendLine(" {");
            using (builder = builder.Indent())
            {
                WriteTypeBody(builder);
            }

            builder.AppendLine("}");
        }

        private void WriteTypeBaseList(IndentStringBuilder builder)
        {
            var baseList = TypeContext.Node.BaseList;
            if (baseList == null)
                return;

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

        public abstract void WriteTypeBody(IndentStringBuilder builder);
    }

    public interface IJavaConversion
    {
        string Namespace { get; set; }
    }
}
