// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.Apple;

class ObjCClassContext : CSharpClassTypeContext<ObjCCompilationContext>
{
    public ObjCClassContext(ClassDeclarationSyntax node, ObjCCompilationContext compilation)
        : base(node, compilation)
    {
    }

    protected override IEnumerable<TypeConversion<ObjCClassContext>> GetConversions()
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

class ObjCStructContext : CSharpStructTypeContext<ObjCCompilationContext>
{
    public ObjCStructContext(StructDeclarationSyntax node, ObjCCompilationContext compilation)
        : base(node, compilation)
    {
    }

    protected override IEnumerable<TypeConversion<ObjCStructContext>> GetConversions()
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

class ObjCInterfaceContext : CSharpInterfaceTypeContext<ObjCCompilationContext>
{
    public ObjCInterfaceContext(InterfaceDeclarationSyntax node, ObjCCompilationContext compilation)
        : base(node, compilation)
    {
    }

    protected override IEnumerable<TypeConversion<ObjCInterfaceContext>> GetConversions()
    {
        yield return new ObjCInterfaceConversionHeader(this, Compilation.Conversion);
    }
}
