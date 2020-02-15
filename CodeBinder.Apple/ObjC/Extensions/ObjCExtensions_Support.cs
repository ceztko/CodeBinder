using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Apple
{
    class SymbolReplacement
    {
        public string Name { get; set; } = string.Empty;
        public SymbolReplacementKind Kind { get; set; }
        public string SetterName { get; set; } = string.Empty;
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
