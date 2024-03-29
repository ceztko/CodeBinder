﻿// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.Utils;

public sealed class StringConversionWriter : ConversionWriter
{
    Func<string> m_resourceString;

    public new string FileName { get; private set; }

    public new string? GeneratedPreamble { get; set; }

    public new string? BasePath { get; set; }

    public new bool? UseUTF8Bom { get; set; }

    public StringConversionWriter(string filename, Func<string> resourceString)
    {
        FileName = filename;
        m_resourceString = resourceString;
    }

    protected override string GetFileName() => FileName;

    protected override string? GetBasePath() => BasePath;

    protected sealed override string? GetGeneratedPreamble() => GeneratedPreamble;

    protected override bool? GetUseUTF8Bom() => UseUTF8Bom;

    protected override void write(CodeBuilder builder)
    {
        builder.Append(m_resourceString());
    }
}
