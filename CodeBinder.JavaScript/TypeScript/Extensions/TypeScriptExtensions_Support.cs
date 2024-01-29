// SPDX-FileCopyrightText: (C) 2023 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.JavaScript.TypeScript;

class SymbolReplacement
{
    public string Name { get; set; } = string.Empty;
    public string SetterName { get; set; } = string.Empty;
    public SymbolReplacementKind Kind { get; set; }
    public bool Negate { get; set; }
}

enum SymbolReplacementKind
{
    Method,
    StaticMethod,
    Field,
    Property,
    Literal
}
