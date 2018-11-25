using CodeBinder.Shared;
using CodeBinder.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.JNI
{
    public class JNICompilationContext : CompilationContext<JNISyntaxTreeContext, JNIModuleContext, CSToJNIConversion>
    {
        Dictionary<string, JNIModuleContextParent> _modules;

        public JNICompilationContext(CSToJNIConversion conversion)
            : base(conversion)
        {
            _modules = new Dictionary<string, JNIModuleContextParent>();
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

        public bool TryGetModule(string moduleName, out JNIModuleContextParent module)
        {
            return _modules.TryGetValue(moduleName, out module);
        }

        protected override JNISyntaxTreeContext createSyntaxTreeContext()
        {
            return new JNISyntaxTreeContext(this);
        }

        public IEnumerable<JNIModuleContextParent> Modules
        {
            get { return _modules.Values; }
        }

        public override IEnumerable<ConversionBuilder> DefaultConversions
        {
            get
            {
                yield return new MethodInitWriter(this);
            }
        }
    }
}
