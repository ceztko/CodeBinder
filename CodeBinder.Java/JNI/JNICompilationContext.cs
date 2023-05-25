// Copyright(c) 2020 Francesco Pretto
// This file is subject to the MIT license
using CodeBinder.Shared;
using CodeBinder.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace CodeBinder.JNI
{
    public class JNICompilationContext : CompilationContext<JNIModuleContext, ConversionCSharpToJNI>
    {
        Dictionary<string, JNIModuleContextParent> _modules;

        public JNICompilationContext(ConversionCSharpToJNI conversion)
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

        protected override INodeVisitor CreateVisitor()
        {
            return new JNINodeVisitor(this);
        }

        public IEnumerable<JNIModuleContextParent> Modules
        {
            get { return _modules.Values; }
        }
    }
}
