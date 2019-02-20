using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Java
{
    class SymbolReplacement
    {
        public string Name { get; set; }
        public string SetterName { get; set; }
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
