// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.NativeAOT;

abstract class NAOTConversionWriter : ConversionWriter
{
    public NAOTCompilationContext Compilation { get; private set; }

    public NAOTConversionWriter(NAOTCompilationContext compilation)
    {
        Compilation = compilation;
    }
}
