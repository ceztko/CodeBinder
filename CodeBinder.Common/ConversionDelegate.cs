﻿// SPDX-FileCopyrightText: (C) 2018 Francesco Pretto<ceztko@gmail.com>
// SPDX-FileCopyrightText: (C) 2017-2018 ICSharpCode
// SPDX-License-Identifier: MIT
using System.IO;
using System.Text;

namespace CodeBinder;

/// <summary>
/// Fully contextualized class to write a conversion item to a stream, coupled to IConversionBuilder
/// </summary>
/// <seealso cref="IConversionWriter"/>
public class ConversionDelegate
{
    IConversionWriter _builder;

    public string? SourcePath { get; private set; }

    internal ConversionDelegate(IConversionWriter builder)
        : this(null, builder) { }

    internal ConversionDelegate(string? sourcePath, IConversionWriter builder)
    {
        SourcePath = sourcePath;
        _builder = builder;
    }

    public string TargetFileName => _builder.FileName;

    public string? TargetBasePath => _builder.BasePath;

    /// <summary>
    /// Use UTF8 Bom. null means automatic, depending on conversion default
    /// </summary>
    public bool? UseUTF8Bom => _builder.UseUTF8Bom;

    public bool Skip => _builder.Skip;

    public void Write(Stream stream, Encoding encoding)
    {
        var writer = new StreamWriter(stream, encoding);
        write(writer);
    }

    public string ToFullString()
    {
        var writer = new StringWriter();
        write(writer);
        return writer.ToString();
    }

    private void write(TextWriter writer)
    {
        var codeBuilder = new CodeBuilder(writer);
        _builder.Write(codeBuilder);
        // NOTE: Flush the writer: needed when writing to filestream
        writer.Flush();
    }
}
