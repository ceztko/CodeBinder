// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace CodeBinder.Shared.CSharp
{
    public abstract class CSharpLanguageConversion
        : LanguageConversion<CSharpSyntaxTreeContext, CSharpBaseTypeContext>
    {
        // FIXME: this should be part of CompilationContext. CompilationContext must me made generic
        Dictionary<string, CSharpTypeContext> _partialTypes;

        public CSharpLanguageConversion()
        {
            _partialTypes = new Dictionary<string, CSharpTypeContext>();
        }

        protected override CSharpSyntaxTreeContext getSyntaxTreeContext()
        {
            return new CSharpSyntaxTreeContext(this);
        }

        // FIXME: The following (AddPartialType, AddPartialTypeChild, TryGetPartialType) should be part of CompilationContext
        public void AddPartialType(string qualifiedName, CompilationContext compilation, CSharpTypeContext type, CSharpBaseTypeContext parent)
        {
            if (_partialTypes.TryGetValue(qualifiedName, out var partialType))
            {
                partialType.AddPartialDeclaration(type);
            }
            else
            {
                type.AddPartialDeclaration(type);
                _partialTypes.Add(qualifiedName, type);
                AddType(compilation, type, parent);
            }
        }

        public bool TryGetPartialType(string qualifiedName, out CSharpTypeContext partialType)
        {
            return _partialTypes.TryGetValue(qualifiedName, out partialType);
        }

        public abstract TypeConversion<CSharpClassTypeContext> GetClassTypeConversion();

        public abstract TypeConversion<CSharpInterfaceTypeContext> GetInterfaceTypeConversion();

        public abstract TypeConversion<CSharpStructTypeContext> GetStructTypeConversion();

        public abstract TypeConversion<CSharpEnumTypeContext> GetEnumTypeConversion();
    }
}
