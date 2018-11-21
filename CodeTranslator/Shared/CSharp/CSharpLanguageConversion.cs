// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace CodeTranslator.Shared.CSharp
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
            // Verify if the given parent is actually a partial type and
            // use that istead if so
            if (parent != null)
            {
                string parentQualifiedName = parent.Node.GetQualifiedName(compilation);
                if (_partialTypes.TryGetValue(parentQualifiedName, out var parentPartialType))
                    parent = parentPartialType;
            }

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

        public abstract TypeConversion<CSharpClassTypeContext> GetClassTypeConversion();

        public abstract TypeConversion<CSharpInterfaceTypeContext> GetInterfaceTypeConversion();

        public abstract TypeConversion<CSharpStructTypeContext> GetStructTypeConversion();

        public abstract TypeConversion<CSharpEnumTypeContext> GetEnumTypeConversion();
    }
}
