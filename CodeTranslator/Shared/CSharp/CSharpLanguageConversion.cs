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
        Dictionary<string, CSharpBaseTypeContext> _partialTypes;

        public CSharpLanguageConversion()
        {
            _partialTypes = new Dictionary<string, CSharpBaseTypeContext>();
        }

        protected override CSharpSyntaxTreeContext getSyntaxTreeContext()
        {
            return new CSharpSyntaxTreeContext(this);
        }

        // FIXME: The following (AddPartialType, AddPartialTypeChild, TryGetPartialType) should be part of CompilationContext
        public void AddPartialType(string qualifiedName, CompilationContext compilation, CSharpBaseTypeContext type)
        {
            _partialTypes.Add(qualifiedName, type);
            AddType(compilation, type, null);
        }

        public void AddPartialTypeChild(CompilationContext compilation, CSharpBaseTypeContext child, CSharpBaseTypeContext parent)
        {
            AddType(compilation, child, parent);
        }

        public bool TryGetPartialType(string typeName, out CSharpBaseTypeContext type)
        {
            return _partialTypes.TryGetValue(typeName, out type);
        }

        public abstract TypeConversion<CSharpClassTypeContext> GetClassTypeConversion();

        public abstract TypeConversion<CSharpInterfaceTypeContext> GetInterfaceTypeConversion();

        public abstract TypeConversion<CSharpStructTypeContext> GetStructTypeConversion();

        public abstract TypeConversion<CSharpEnumTypeContext> GetEnumTypeConversion();
    }
}
