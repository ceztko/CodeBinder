using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace CodeBinder.Shared.CSharp
{
    public class CSharpTypeParameters : ReadOnlyCollection<CSharpTypeParameter>
    {
        public CSharpTypeParameters(IList<CSharpTypeParameter> list)
            : base(list) { }
    }

    public class CSharpTypeParameter
    {
        public CSharpTypeParameter(TypeParameterSyntax type, TypeParameterConstraintClauseSyntax constraints)
        {
            Type = type;
            Constraints = constraints;
        }

        public TypeParameterSyntax Type { get; private set; }
        public TypeParameterConstraintClauseSyntax Constraints { get; private set; }
    }
}
