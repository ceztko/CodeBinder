using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace CodeBinder.Shared.CSharp
{
    public class CSharpCompilationContext : CompilationContext<CSharpBaseTypeContext, CSharpSyntaxTreeContext, CSharpNodeVisitor, CSharpLanguageConversion>
    {
        Dictionary<string, CSharpTypeContext> _partialTypes;

        public CSharpCompilationContext(CSharpLanguageConversion conversion)
            : base(conversion)
        {
            _partialTypes = new Dictionary<string, CSharpTypeContext>();
        }

        protected override CSharpSyntaxTreeContext createSyntaxTreeContext()
        {
            return new CSharpSyntaxTreeContext(this);
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
}
