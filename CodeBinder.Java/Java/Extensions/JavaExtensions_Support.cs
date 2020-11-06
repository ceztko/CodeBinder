// Copyright(c) 2020 Francesco Pretto
// This file is subject to the MIT license
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Java
{
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
        Literal
    }
}
