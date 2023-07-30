// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.CLang;

public class CLangCompilationContext : CSharpCompilationContextBase<CLangModuleContext, ConversionCSharpToCLang>
{
    Dictionary<string, CLangModuleContextParent> _Modules;
    List<EnumDeclarationSyntax> _Enums;
    List<ClassDeclarationSyntax> _OpaqueTypes;
    List<StructDeclarationSyntax> _StructTypes;
    List<DelegateDeclarationSyntax> _Callbacks;

    internal CLangCompilationContext(ConversionCSharpToCLang conversion)
        : base(conversion)
    {
        _Modules = new Dictionary<string, CLangModuleContextParent>();
        _Enums = new List<EnumDeclarationSyntax>();
        _OpaqueTypes = new List<ClassDeclarationSyntax>();
        _StructTypes = new List<StructDeclarationSyntax>();
        _Callbacks = new List<DelegateDeclarationSyntax>();
    }

    public void AddModule(CLangModuleContextParent module)
    {
        _Modules.Add(module.Name, module);
        AddTypeContext(module, null);
    }

    public void AddModuleChild(CLangModuleContextChild module, CLangModuleContextParent parent)
    {
        AddTypeContext(module, parent);
    }

    public bool TryGetModule(string moduleName, [NotNullWhen(true)]out CLangModuleContextParent? module)
    {
        return _Modules.TryGetValue(moduleName, out module);
    }

    public void AddEnum(EnumDeclarationSyntax enm)
    {
        _Enums.Add(enm);
    }

    public void AddCallback(DelegateDeclarationSyntax callback)
    {
        _Callbacks.Add(callback);
    }

    public void AddType(ClassDeclarationSyntax type)
    {
        _OpaqueTypes.Add(type);
    }

    public void AddType(StructDeclarationSyntax type)
    {
        _StructTypes.Add(type);
    }

    protected override CSharpCollectionContextBase CreateCollectionContext()
    {
        return new CLangCollectionContext(this);
    }

    public IReadOnlyCollection<CLangModuleContextParent> Modules
    {
        get { return _Modules.Values; }
    }

    public IReadOnlyList<EnumDeclarationSyntax> Enums
    {
        get { return _Enums; }
    }

    public IReadOnlyList<ClassDeclarationSyntax> OpaqueTypes
    {
        get { return _OpaqueTypes; }
    }

    public IReadOnlyList<StructDeclarationSyntax> StructTypes
    {
        get { return _StructTypes; }
    }

    public IReadOnlyList<DelegateDeclarationSyntax> Callbacks
    {
        get { return _Callbacks; }
    }

    public override IEnumerable<IConversionWriter> DefaultConversions
    {
        get
        {
            yield return new CLangLibraryHeaderConversion(this);
            yield return new CLangLibDefsHeaderConversion(this);
            yield return new CLangTypesHeaderConversion(this);
            yield return new CLangMethodInitConversion(this);
        }
    }
}
