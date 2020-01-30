using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace CodeBinder.Shared.CSharp
{
    /// <summary>
    /// Basic CSharp compilation context
    /// </summary>
    public abstract class CSharpCompilationContext :
        CompilationContext<CSharpBaseTypeContext, CSharpSyntaxTreeContext, CSharpLanguageConversion>
    {
        Dictionary<string, CSharpTypeContext> _partialTypes;

        protected CSharpCompilationContext()
        {
            _partialTypes = new Dictionary<string, CSharpTypeContext>();
        }

        protected override CSharpSyntaxTreeContext createSyntaxTreeContext()
        {
            return new CSharpSyntaxTreeContextImpl(this);
        }

        public void AddPartialType(string qualifiedName, CompilationContext compilation, CSharpTypeContext type, CSharpBaseTypeContext? parent)
        {
            if (_partialTypes.TryGetValue(qualifiedName, out var partialType))
            {
                partialType.AddPartialDeclaration(type);
            }
            else
            {
                type.AddPartialDeclaration(type);
                _partialTypes.Add(qualifiedName, type);
                AddType(type, parent);
            }
        }

        public bool TryGetPartialType(string qualifiedName, [NotNullWhen(true)]out CSharpTypeContext? partialType)
        {
            return _partialTypes.TryGetValue(qualifiedName, out partialType);
        }
    }

    sealed class CSharpCompilationContextImpl : CSharpCompilationContext
    {
        public new CSharpLanguageConversion Conversion { get; private set; }

        public CSharpCompilationContextImpl(CSharpLanguageConversion conversion)
        {
            Conversion = conversion;
        }

        protected override CSharpLanguageConversion GetLanguageConversion() => Conversion;
    }
}
