// SPDX-FileCopyrightText: (C) 2023 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.JavaScript.NAPI;

public class NAPICompilationContext : CSharpCompilationContextBase<NAPIModuleContext, ConversionCSharpToNAPI>
{
    Dictionary<string, JNIModuleContextParent> _modules;

    public NAPICompilationContext(ConversionCSharpToNAPI conversion)
        : base(conversion)
    {
        _modules = new Dictionary<string, JNIModuleContextParent>();
    }

    public void AddModule(CompilationContext compilation, JNIModuleContextParent module)
    {
        _modules.Add(module.Name, module);
        AddTypeContext(module, null);
    }

    public void AddModuleChild(CompilationContext compilation, JNIModuleContextChild module, JNIModuleContextParent parent)
    {
        AddTypeContext(module, parent);
    }

    public bool TryGetModule(string moduleName, [NotNullWhen(true)]out JNIModuleContextParent? module)
    {
        return _modules.TryGetValue(moduleName, out module);
    }

    protected override CSharpCollectionContextBase CreateCollectionContext()
    {
        return new NAPICollectionContext(this);
    }

    public IEnumerable<JNIModuleContextParent> Modules
    {
        get { return _modules.Values; }
    }

    public override IEnumerable<IConversionWriter> DefaultConversions
    {
        get
        {
            yield return new NAPIMethodInitConversion(this);
        }
    }
}
