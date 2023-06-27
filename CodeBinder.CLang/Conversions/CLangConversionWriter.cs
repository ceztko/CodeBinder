// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.CLang;

abstract class CLangConversionWriter : ConversionWriter
{
    public CLangCompilationContext Compilation { get; private set; }

    public CLangConversionWriter(CLangCompilationContext compilation)
    {
        Compilation = compilation;
    }
}
