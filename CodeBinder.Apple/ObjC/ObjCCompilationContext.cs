// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.Apple;

public class ObjCCompilationContext : CSharpCompilationContext<ConversionCSharpToObjC>
{
    public string CLangLibraryHeaderName => $"{LibraryName}.h";

    public string ObjCLibraryHeaderName => $"{ObjCLibraryName}.h";

    public string ObjCLibraryName => $"OC{LibraryName}";

    public ObjCCompilationContext(ConversionCSharpToObjC conversion)
        : base(conversion)

    {
    }

    protected override CSharpClassTypeContext CreateContext(ClassDeclarationSyntax cls)
    {
        return new ObjCClassContext(cls, this);
    }

    protected override CSharpInterfaceTypeContext CreateContext(InterfaceDeclarationSyntax iface)
    {
        return new ObjCInterfaceContext(iface, this);
    }

    protected override CSharpStructTypeContext CreateContext(StructDeclarationSyntax str)
    {
        return new ObjCStructContext(str, this);
    }

    public override IEnumerable<IConversionWriter> DefaultConversions
    {
        get
        {
            yield return new ObjCLibDefsHeaderConversion(this);
            yield return new ObjCTypesHeaderConversion(this, true);
            yield return new ObjCTypesHeaderConversion(this, false);
            yield return new ObjCLibraryHeaderConversion(this, true);
            yield return new ObjCLibraryHeaderConversion(this, false);
        }
    }
}
