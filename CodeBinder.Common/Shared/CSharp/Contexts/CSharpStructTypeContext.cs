using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Shared.CSharp
{
    /// <summary>
    /// Struct syntax context
    /// </summary>
    /// <remarks>Inherit this class if needed to extend a struct context</remarks>
    public abstract class CSharpStructTypeContext<TCompilationContext, TTypeContext>
        : CSharpStructTypeContext, ITypeContext<TCompilationContext>
        where TCompilationContext : CSharpCompilationContext
        where TTypeContext : CSharpStructTypeContext
    {
        public CSharpStructTypeContext(StructDeclarationSyntax node)
            : base(node) { }

        public new TCompilationContext Compilation => getCSharpCompilationContext();

        protected abstract TCompilationContext getCSharpCompilationContext();

        protected override CSharpCompilationContext GetCSharpCompilationContext() => getCSharpCompilationContext();

        protected abstract IEnumerable<TypeConversion<TTypeContext>> getConversions();

        protected override IEnumerable<TypeConversion> GetConversions() => getConversions();
    }

    public abstract class CSharpStructTypeContext : CSharpTypeContext<StructDeclarationSyntax>
    {
        protected CSharpStructTypeContext(StructDeclarationSyntax node)
            : base(node) { }

        protected override IEnumerable<TypeConversion> GetConversions()
        {
            return Compilation.Conversion.GetConversions(this);
        }
    }

    public sealed class CSharpStructTypeContextImpl : CSharpStructTypeContext
    {
        public new CSharpCompilationContext Compilation { get; private set; }

        public CSharpStructTypeContextImpl(StructDeclarationSyntax node, CSharpCompilationContext compilation)
            : base(node)
        {
            Compilation = compilation;
        }

        protected override CSharpCompilationContext GetCSharpCompilationContext() => Compilation;
    }
}
