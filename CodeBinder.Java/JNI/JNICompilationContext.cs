// Copyright(c) 2020 Francesco Pretto
// This file is subject to the MIT license
using CodeBinder.Shared;
using CodeBinder.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace CodeBinder.JNI
{
    public class JNICompilationContext : CompilationContext<JNIModuleContext, ConversionCSharpToJNI>
    {
        Dictionary<string, JNIModuleContextParent> _modules;

        public new ConversionCSharpToJNI Conversion { get; private set; }

        public JNICompilationContext(ConversionCSharpToJNI conversion)
        {
            _modules = new Dictionary<string, JNIModuleContextParent>();
            Conversion = conversion;
        }

        public void AddModule(CompilationContext compilation, JNIModuleContextParent module)
        {
            _modules.Add(module.Name, module);
            AddType(module, null);
        }

        public void AddModuleChild(CompilationContext compilation, JNIModuleContextChild module, JNIModuleContextParent parent)
        {
            AddType(module, parent);
        }

        public bool TryGetModule(string moduleName, [NotNullWhen(true)]out JNIModuleContextParent? module)
        {
            return _modules.TryGetValue(moduleName, out module);
        }

        protected override ConversionCSharpToJNI getLanguageConversion() => Conversion;

        protected override INodeVisitor CreateVisitor()
        {
            return new JNINodeVisitor(this);
        }

        public IEnumerable<JNIModuleContextParent> Modules
        {
            get { return _modules.Values; }
        }

        public override IEnumerable<IConversionWriter> DefaultConversions
        {
            get
            {
                yield return new JNIMethodInitConversion(this);
            }
        }
    }
}
