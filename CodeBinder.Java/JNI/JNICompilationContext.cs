// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.JNI;

public class JNICompilationContext : CSharpCompilationContextBase<JNIModuleContext, ConversionCSharpToJNI>
{
    Dictionary<string, JNIModuleContextParent> _modules;

    public JNICompilationContext(ConversionCSharpToJNI conversion)
        : base(conversion)
    {
        _modules = new Dictionary<string, JNIModuleContextParent>();
    }

    public void AddModule(CompilationContext compilation, JNIModuleContextParent module)
    {
        _ = compilation;
        _modules.Add(module.Name, module);
        AddTypeContext(module, null);
    }

    public void AddModuleChild(CompilationContext compilation, JNIModuleContextChild module, JNIModuleContextParent parent)
    {
        _ = compilation;
        AddTypeContext(module, parent);
    }

    public bool TryGetModule(string moduleName, [NotNullWhen(true)]out JNIModuleContextParent? module)
    {
        return _modules.TryGetValue(moduleName, out module);
    }

    protected override CSharpCollectionContextBase CreateCollectionContext()
    {
        return new JNICollectionContext(this);
    }

    public IEnumerable<JNIModuleContextParent> Modules
    {
        get { return _modules.Values; }
    }
}
