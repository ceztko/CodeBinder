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
using System.Text;

namespace CodeBinder.Java
{
    abstract partial class JavaBaseTypeConversion<TTypeContext>
        : CSharpTypeConversion<TTypeContext, ConversionCSharpToJava>
        where TTypeContext : CSharpBaseTypeContext
    {
        protected JavaBaseTypeConversion(TTypeContext context, ConversionCSharpToJava conversion)
            : base(context, conversion) { }

        public string Namespace
        {
            get
            {

                return Conversion.NamespaceMapping.GetMappedNamespace(
                    Context.Node.GetContainingNamespace(this),
                        NamespaceNormalization.LowerCase);
            }
        }

        public override string? BasePath
        {
            get
            {
                return Namespace.Replace('.', Path.DirectorySeparatorChar);
            }
        }

        public override string FileName
        {
            get { return Context.Node.GetName() + ".java"; }
        }

        public override string GeneratedPreamble
        {
            get { return ConversionCSharpToJava.SourcePreamble; }
        }

        public IEnumerable<string> Imports
        {
            get
            {
                yield return "java.util.*";
                yield return ConversionCSharpToJava.CodeBinderNamespace + ".*";
                var splittedNs = Namespace?.Split('.');
                if (splittedNs != null)
                {
                    var builder = new StringBuilder();
                    bool first = true;
                    for (int i = 0; i < splittedNs.Length - 1; i++)
                    {
                        if (first)
                            first = false;
                        else
                            builder.Append(".");

                        builder.Append(splittedNs[i]);
                        yield return builder.ToString() + ".*";
                    }
                }

                foreach (var import in OtherImports)
                    yield return import;

            }
        }

        public virtual IEnumerable<string> OtherImports
        {
            get { return GetImports(Context.Node); }
        }

        public sealed override void Write(CodeBuilder builder)
        {
            string? ns = Namespace;
            if (ns != null)
                builder.Append("package").Space().Append(ns).EndOfStatement();

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
                    yield return attribute.GetConstructorArgument<string>(0);
            }
        }

        protected abstract CodeWriter GetTypeWriter();
    }

    abstract partial class JavaTypeConversion<TTypeContext> : JavaBaseTypeConversion<TTypeContext>
        where TTypeContext : CSharpTypeContext
    {
        protected JavaTypeConversion(TTypeContext context, ConversionCSharpToJava conversion)
            : base(context, conversion) { }
    }

    class JavaInterfaceConversion : JavaTypeConversion<CSharpInterfaceTypeContext>
    {
        public JavaInterfaceConversion(CSharpInterfaceTypeContext iface, ConversionCSharpToJava conversion)
            : base(iface, conversion) { }

        protected override CodeWriter GetTypeWriter()
        {
            return new InterfaceTypeWriter(Context.Node, Context.ComputePartialDeclarationsTree(),
                new JavaCodeConversionContext(this, Conversion));
        }
    }

    class JavaClassConversion : JavaTypeConversion<CSharpClassTypeContext>
    {
        public JavaClassConversion(CSharpClassTypeContext cls, ConversionCSharpToJava conversion)
            : base(cls, conversion) { }

        protected override CodeWriter GetTypeWriter()
        {
            return new ClassTypeWriter(Context.Node, Context.ComputePartialDeclarationsTree(),
                new JavaCodeConversionContext(this, Conversion));
        }
    }

    class JavaStructConversion : JavaTypeConversion<CSharpStructTypeContext>
    {
        public JavaStructConversion(CSharpStructTypeContext str, ConversionCSharpToJava conversion)
            : base(str, conversion) { }

        protected override CodeWriter GetTypeWriter()
        {
            return new StructTypeWriter(Context.Node, Context.ComputePartialDeclarationsTree(),
                new JavaCodeConversionContext(this, Conversion));
        }
    }

    class JavaEnumConversion : JavaBaseTypeConversion<CSharpEnumTypeContext>
    {
        public JavaEnumConversion(CSharpEnumTypeContext enm, ConversionCSharpToJava conversion)
            : base(enm, conversion) { }

        protected override CodeWriter GetTypeWriter()
        {
            return new EnumTypeWriter(Context.Node,
                new JavaCodeConversionContext(this, Conversion));
        }
    }
}
