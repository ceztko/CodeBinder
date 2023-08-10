// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.NativeAOT;

public class NAOTCompilationContext : CSharpCompilationContextBase<NAOTModuleContext, ConversionCSharpToNativeAOT>
{
    Dictionary<string, NAOTModuleContextParent> _Modules;
    List<EnumDeclarationSyntax> _Enums;
    List<ClassDeclarationSyntax> _OpaqueTypes;
    List<StructDeclarationSyntax> _StructTypes;
    List<DelegateDeclarationSyntax> _Callbacks;

    internal NAOTCompilationContext(ConversionCSharpToNativeAOT conversion)
        : base(conversion)
    {
        _Modules = new Dictionary<string, NAOTModuleContextParent>();
        _Enums = new List<EnumDeclarationSyntax>();
        _OpaqueTypes = new List<ClassDeclarationSyntax>();
        _StructTypes = new List<StructDeclarationSyntax>();
        _Callbacks = new List<DelegateDeclarationSyntax>();
    }

    public void AddModule(NAOTModuleContextParent module)
    {
        _Modules.Add(module.Name, module);
        AddTypeContext(module, null);
    }

    public void AddModuleChild(NAOTModuleContextChild module, NAOTModuleContextParent parent)
    {
        AddTypeContext(module, parent);
    }

    public bool TryGetModule(string moduleName, [NotNullWhen(true)]out NAOTModuleContextParent? module)
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

    public void AddOpaqueType(ClassDeclarationSyntax type)
    {
        _OpaqueTypes.Add(type);
    }

    public void AddType(StructDeclarationSyntax type)
    {
        _StructTypes.Add(type);
    }

    protected override CSharpCollectionContextBase CreateCollectionContext()
    {
        return new NAOTCollectionContext(this);
    }

    public IReadOnlyCollection<NAOTModuleContextParent> Modules
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
            if (Conversion.CreateTemplateProject)
            {
                yield return new StringConversionWriter($"{LibraryName}NAOT.csproj",
                    () => TemplateCSProj) { GeneratedPreamble = ConversionCSharpToNativeAOT.SourcePreamble };
            }
            else
            {
                yield return new NAOTTypesConversion(this);
                yield return new NAOTDelegatesConversion(this);
            }
        }
    }

    const string TemplateCSProj =
"""
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Library</OutputType>
    <TargetFramework>net7.0</TargetFramework>
    <LangVersion>preview</LangVersion>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <PublishAot>true</PublishAot>
    <AllowUnsafeBlocks>True</AllowUnsafeBlocks>
    <AssemblySearchPaths>$(AssemblySearchPaths);$(OutputPath)</AssemblySearchPaths>
  </PropertyGroup>

  <ItemGroup>
    <Reference Include="CodeBinder.Redist" />
  </ItemGroup>

</Project>
""";
}
