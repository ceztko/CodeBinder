// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.Shared;

public interface ICompilationContextProvider
{
    CompilationContext Compilation { get; }
}
