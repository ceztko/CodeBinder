// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.Java;

abstract class JavaCodeWriter<TItem> : CodeWriter<TItem, JavaCodeConversionContext>
{
    protected JavaCodeWriter(TItem item, JavaCodeConversionContext context)
        : base(item, context) { }
}

// NOTE: This class is needed since the conversion doesn't use a custom CompilationContext
// so we can't propagate CSharCompilationContext to access the ConversionCSharpToJava
public class JavaCodeConversionContext : ICompilationProvider
{
    public CompilationContext Compilation { get; private set; }

    public ConversionCSharpToJava Conversion { get; private set; }

    public JavaCodeConversionContext(CompilationContext provider, ConversionCSharpToJava conversion)
    {
        Compilation = provider;
        Conversion = conversion;
    }

    CompilationProvider ICompilationProvider.Compilation => Compilation;
}
