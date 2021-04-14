// Copyright(c) 2020 Francesco Pretto
// This file is subject to the MIT license
using CodeBinder.Shared;
using CodeBinder.Shared.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Apple
{
    class ObjCClassContext : CSharpClassTypeContext<ObjCCompilationContext, ObjCClassContext>
    {
        public new ObjCCompilationContext Compilation { get; private set; }

        public ObjCClassContext(ClassDeclarationSyntax node, ObjCCompilationContext compilation)
            : base(node)
        {
            Compilation = compilation;
        }

        protected override ObjCCompilationContext getCSharpCompilationContext() => Compilation;

        protected override IEnumerable<TypeConversion<ObjCClassContext>> getConversions()
        {
            if (Node.HasAccessibility(Accessibility.Public, Compilation))
            {
                yield return new ObjCClassConversionHeader(this, Compilation.Conversion, ObjCHeaderType.Public);
                yield return new ObjCClassConversionHeader(this, Compilation.Conversion, ObjCHeaderType.Internal);
                yield return new ObjCClassConversionImplementation(this, Compilation.Conversion, ObjImplementationType.PublicType);
            }
            else
            {
                yield return new ObjCClassConversionHeader(this, Compilation.Conversion, ObjCHeaderType.InternalOnly);
                yield return new ObjCClassConversionImplementation(this, Compilation.Conversion, ObjImplementationType.InternalType);
            }


        }
    }

    class ObjCStructContext : CSharpStructTypeContext<ObjCCompilationContext, ObjCStructContext>
    {
        public new ObjCCompilationContext Compilation { get; private set; }

        public ObjCStructContext(StructDeclarationSyntax node, ObjCCompilationContext compilation)
            : base(node)
        {
            Compilation = compilation;
        }

        protected override ObjCCompilationContext getCSharpCompilationContext() => Compilation;

        protected override IEnumerable<TypeConversion<ObjCStructContext>> getConversions()
        {
            if (Node.HasAccessibility(Accessibility.Public, Compilation))
            {
                yield return new ObjCStructConversionHeader(this, Compilation.Conversion, ObjCHeaderType.Public);
                yield return new ObjCStructConversionHeader(this, Compilation.Conversion, ObjCHeaderType.Internal);
                yield return new ObjCStructConversionImplementation(this, Compilation.Conversion, ObjImplementationType.PublicType);
            }
            else
            {
                yield return new ObjCStructConversionHeader(this, Compilation.Conversion, ObjCHeaderType.InternalOnly);
                yield return new ObjCStructConversionImplementation(this, Compilation.Conversion, ObjImplementationType.InternalType);
            }
        }
    }

    class ObjCInterfaceContext : CSharpInterfaceTypeContext<ObjCCompilationContext, ObjCInterfaceContext>
    {
        public new ObjCCompilationContext Compilation { get; private set; }

        public ObjCInterfaceContext(InterfaceDeclarationSyntax node, ObjCCompilationContext compilation)
            : base(node)
        {
            Compilation = compilation;
        }

        protected override ObjCCompilationContext getCSharpCompilationContext() => Compilation;

        protected override IEnumerable<TypeConversion<ObjCInterfaceContext>> getConversions()
        {
            yield return new ObjCInterfaceConversionHeader(this, Compilation.Conversion);
        }
    }
}
