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

namespace CodeTranslator.Java
{
    abstract partial class JavaBaseTypeConversion<TTypeContext> : CSharpTypeConversion<TTypeContext, CSToJavaConversion>
        where TTypeContext : CSharpBaseTypeContext
    {
        public string Namespace { get; private set; }
        string _Basepath;

        protected JavaBaseTypeConversion(CSToJavaConversion conversion)
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
            get { return CSToJavaConversion.SourcePreamble; }
        }

        public virtual IEnumerable<string> Imports
        {
            get { yield return "java.util.*"; }
        }

        public sealed override void Write(CodeBuilder builder)
        {
            builder.Append("package").Space().Append(Namespace).EndOfStatement();
            builder.AppendLine();
            bool hasImports = false;
            foreach (var import in Imports)
            {
                builder.Append("import").Space().Append(import).EndOfStatement();
                hasImports = true;
            }

            if (hasImports)
                builder.AppendLine();

            builder.Append(GetTypeWriter());
        }

        protected abstract CodeWriter GetTypeWriter();
    }

    abstract partial class JavaTypeConversion<TTypeContext> : JavaBaseTypeConversion<TTypeContext>
        where TTypeContext : CSharpTypeContext
    {
        protected JavaTypeConversion(CSToJavaConversion conversion)
            : base(conversion) { }
    }

    class JavaInterfaceConversion : JavaTypeConversion<CSharpInterfaceTypeContext>
    {
        public JavaInterfaceConversion(CSToJavaConversion conversion)
            : base(conversion) { }

        protected override CodeWriter GetTypeWriter()
        {
            return new InterfaceTypeWriter(TypeContext.Node, TypeContext.ComputePartialDeclarationsTree(), this);
        }
    }

    class JavaClassConversion : JavaTypeConversion<CSharpClassTypeContext>
    {
        public JavaClassConversion(CSToJavaConversion conversion)
            : base(conversion) { }

        protected override CodeWriter GetTypeWriter()
        {
            return new ClassTypeWriter(TypeContext.Node, TypeContext.ComputePartialDeclarationsTree(), this);
        }
    }

    class JavaStructConversion : JavaTypeConversion<CSharpStructTypeContext>
    {
        public JavaStructConversion(CSToJavaConversion conversion)
            : base(conversion) { }

        protected override CodeWriter GetTypeWriter()
        {
            return new StructTypeWriter(TypeContext.Node, TypeContext.ComputePartialDeclarationsTree(), this);
        }
    }

    class JavaEnumConversion : JavaBaseTypeConversion<CSharpEnumTypeContext>
    {
        public JavaEnumConversion(CSToJavaConversion conversion)
            : base(conversion) { }

        protected override CodeWriter GetTypeWriter()
        {
            return new EnumTypeWriter(TypeContext.Node, this);
        }
    }
}
