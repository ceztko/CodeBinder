// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace CodeTranslator.Shared.CSharp
{
    public abstract class CSharpLanguageConversion
        : LanguageConversion<CSharpSyntaxTreeContext, CSharpTypeContext>
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
        public void AddPartialType(CompilationContext compilation, CSharpTypeContext type)
        {
            _partialTypes.Add(type.TypeName, type);
            AddType(compilation, type, null);
        }

        public void AddPartialTypeChild(CompilationContext compilation, CSharpTypeContext child, CSharpTypeContext parent)
        {
            AddType(compilation, child, parent);
        }

        public bool TryGetPartialType(string typeName, out CSharpTypeContext type)
        {
            return _partialTypes.TryGetValue(typeName, out type);
        }

        public abstract TypeConversion<CSharpClassTypeContext> GetClassTypeConversion();

        public abstract TypeConversion<CSharpInterfaceTypeContext> GetInterfaceTypeConversion();

        public abstract TypeConversion<CSharpStructTypeContext> GetStructTypeConversion();

        public abstract TypeConversion<CSharpEnumTypeContext> GetEnumTypeConversion();
    }
}
