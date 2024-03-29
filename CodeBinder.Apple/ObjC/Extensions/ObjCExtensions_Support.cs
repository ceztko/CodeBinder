﻿// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.Apple;

class SymbolReplacement
{
    public string Name { get; set; } = string.Empty;
    public SymbolReplacementKind Kind { get; set; }

    // Valid only for properties
    public string SetterName { get; set; } = string.Empty;

    // Valid only for methods/properties
    public string? ReturnType { get; set; } = null;
}

enum SymbolReplacementKind
{
    Method,
    StaticMethod,
    Field,
    Literal,
    Property
}
