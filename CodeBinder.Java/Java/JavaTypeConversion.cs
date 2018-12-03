// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using CodeBinder.Attributes;
using CodeBinder.Shared;
using CodeBinder.Shared.CSharp;
using CodeBinder.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace CodeBinder.Java
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

        public IEnumerable<string> Imports
        {
            get
            {
                yield return "java.util.*";
                foreach (var import in OtherImports)
                    yield return import;

            }
        }

        public virtual IEnumerable<string> OtherImports
        {
            get { return GetImports(TypeContext.Node); }
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

        protected IEnumerable<string> GetImports(SyntaxNode node)
        {
            var attributes = node.GetAttributes(this);
            foreach (var attribute in attributes)
            {
                if (attribute.IsAttribute<ImportAttribute>())
                {
                    Debug.Assert(attribute.ConstructorArguments.Length == 1);
                    var constructorParam = attribute.ConstructorArguments[0];
                    yield return (string)constructorParam.Value;
                }
            }
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
            return new InterfaceTypeWriter(TypeContext.Node, TypeContext.ComputePartialDeclarationsTree(),
                new JavaCodeConversionContext(this, Conversion));
        }
    }

    class JavaClassConversion : JavaTypeConversion<CSharpClassTypeContext>
    {
        public JavaClassConversion(CSToJavaConversion conversion)
            : base(conversion) { }

        protected override CodeWriter GetTypeWriter()
        {
            return new ClassTypeWriter(TypeContext.Node, TypeContext.ComputePartialDeclarationsTree(),
                new JavaCodeConversionContext(this, Conversion));
        }
    }

    class JavaStructConversion : JavaTypeConversion<CSharpStructTypeContext>
    {
        public JavaStructConversion(CSToJavaConversion conversion)
            : base(conversion) { }

        protected override CodeWriter GetTypeWriter()
        {
            return new StructTypeWriter(TypeContext.Node, TypeContext.ComputePartialDeclarationsTree(),
                new JavaCodeConversionContext(this, Conversion));
        }
    }

    class JavaEnumConversion : JavaBaseTypeConversion<CSharpEnumTypeContext>
    {
        public JavaEnumConversion(CSToJavaConversion conversion)
            : base(conversion) { }

        protected override CodeWriter GetTypeWriter()
        {
            return new EnumTypeWriter(TypeContext.Node,
                new JavaCodeConversionContext(this, Conversion));
        }
    }
}
