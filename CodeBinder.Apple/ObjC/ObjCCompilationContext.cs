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
            yield return new StringConversionWriter(nameof(ObjCResources.CBHandledObject_h).ToObjCHeaderFilename(),
                () => ObjCClasses.GetCBHandledObject(this)) { BasePath = ConversionCSharpToObjC.SupportBasePath, GeneratedPreamble = ConversionCSharpToObjC.SourcePreamble };
            yield return new StringConversionWriter(nameof(ObjCClasses.CBKeyValuePair_h).ToObjCHeaderFilename(),
                () => ObjCClasses.GetCBKeyValuePair(this)) { BasePath = ConversionCSharpToObjC.SupportBasePath, GeneratedPreamble = ConversionCSharpToObjC.SourcePreamble };
            yield return new StringConversionWriter(nameof(ObjCClasses.CBHandleRef_h).ToObjCHeaderFilename(),
                () => ObjCClasses.GetCBHandleRef(this)) { BasePath = ConversionCSharpToObjC.SupportBasePath, GeneratedPreamble = ConversionCSharpToObjC.SourcePreamble };
            yield return new StringConversionWriter(nameof(ObjCClasses.CBException_h).ToObjCHeaderFilename(),
                () => ObjCClasses.GetCBException(this)) { BasePath = ConversionCSharpToObjC.SupportBasePath, GeneratedPreamble = ConversionCSharpToObjC.SourcePreamble };
            foreach (var type in ObjCUtils.GetInteropTypes())
            {
                yield return new ObjCArrayBoxWriter(this, type, false);
                yield return new ObjCArrayBoxWriter(this, type, true);
            }
        }
    }
}
