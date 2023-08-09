// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.NativeAOT;

abstract class NativeAOTConversionWriter : ConversionWriter
{
    public NativeAOTCompilationContext Compilation { get; private set; }

    public NativeAOTConversionWriter(NativeAOTCompilationContext compilation)
    {
        Compilation = compilation;
    }
}
