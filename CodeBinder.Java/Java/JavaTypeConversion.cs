// SPDX-FileCopyrightText: (C) 2018 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT
using System.IO;
using System.Text;

namespace CodeBinder.Java;

abstract partial class JavaBaseTypeConversion<TTypeContext>
    : CSharpTypeConversion<TTypeContext, ConversionCSharpToJava>
    where TTypeContext : CSharpMemberTypeContext
{
    protected JavaBaseTypeConversion(TTypeContext context, ConversionCSharpToJava conversion)
        : base(context, conversion) { }

    public string Namespace
    {
        get
        {
            return Context.Node.GetMappedNamespaceName(Conversion.NamespaceMapping,
                NamespaceNormalization.LowerCase, Context);
        }
    }

    protected override string? GetBasePath() => Namespace.Replace('.', Path.DirectorySeparatorChar);

    protected override string GetFileName() => $"{Context.Name}.java";

    protected override string GetGeneratedPreamble() => ConversionCSharpToJava.SourcePreamble;

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

    protected sealed override void write(CodeBuilder builder)
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
        return new JavaInterfaceWriter(Context.Node, Context.ComputePartialDeclarationsTree(),
            new JavaCodeConversionContext(Compilation, Conversion));
    }
}

class JavaClassConversion : JavaTypeConversion<CSharpClassTypeContext>
{
    public JavaClassConversion(CSharpClassTypeContext cls, ConversionCSharpToJava conversion)
        : base(cls, conversion) { }

    protected override CodeWriter GetTypeWriter()
    {
        return new JavaClassWriter(Context.Node, Context.ComputePartialDeclarationsTree(),
            new JavaCodeConversionContext(Compilation, Conversion));
    }
}

class JavaStructConversion : JavaTypeConversion<CSharpStructTypeContext>
{
    public JavaStructConversion(CSharpStructTypeContext str, ConversionCSharpToJava conversion)
        : base(str, conversion) { }

    protected override CodeWriter GetTypeWriter()
    {
        return new JavaStructWriter(Context.Node, Context.ComputePartialDeclarationsTree(),
            new JavaCodeConversionContext(Compilation, Conversion));
    }
}

class JavaEnumConversion : JavaBaseTypeConversion<CSharpEnumTypeContext>
{
    public JavaEnumConversion(CSharpEnumTypeContext enm, ConversionCSharpToJava conversion)
        : base(enm, conversion) { }

    protected override CodeWriter GetTypeWriter()
    {
        return new JavaEnumWriter(Context.Node,
            new JavaCodeConversionContext(Compilation, Conversion));
    }
}
