// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT
using System.Collections.ObjectModel;

namespace CodeBinder.Shared.CSharp;

public class CSharpTypeParameters : ReadOnlyCollection<CSharpTypeParameter>
{
    public CSharpTypeParameters(IList<CSharpTypeParameter> list)
        : base(list) { }
}

public class CSharpTypeParameter
{
    public CSharpTypeParameter(TypeParameterSyntax type, TypeParameterConstraintClauseSyntax? constraints)
    {
        Type = type;
        Constraints = constraints;
    }

    public TypeParameterSyntax Type { get; private set; }
    public TypeParameterConstraintClauseSyntax? Constraints { get; private set; }
}
