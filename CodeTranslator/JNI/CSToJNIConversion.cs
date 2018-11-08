// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using System;
using System.Collections.Generic;
using System.Linq;
using CodeTranslator.Shared;
using CodeTranslator.JNI.Resources;
using CodeTranslator.Util;

namespace CodeTranslator.JNI
{
    public class CSToJNIConversion : LanguageConversion<JNISyntaxTreeContext, JNIModuleContext>
    {
        Dictionary<string, JNIModuleContextParent> _modules;

        public CSToJNIConversion()
        {
            _modules = new Dictionary<string, JNIModuleContextParent>();
        }

        /// <summary>Base namespace of the package, to be set outside</summary>
        public string BaseNamespace { get; set; }

        public void AddModule(JNIModuleContextParent module)
        {
            module.LanguageConversion = this;
            _modules.Add(module.Name, module);
            AddType(module, null);
        }

        public void AddModule(JNIModuleContextChild module, JNIModuleContextParent parent)
        {
            module.LanguageConversion = this;
            AddType(module, parent);
        }

        public bool TryGetModule(string moduleName, out JNIModuleContextParent module)
        {
            return _modules.TryGetValue(moduleName, out module);
        }

        protected override JNISyntaxTreeContext getSyntaxTreeContext()
        {
            return new JNISyntaxTreeContext(this);
        }

        public override IEnumerable<ConversionBuilder> DefaultConversions
        {
            get
            {
                yield return new StringConversionBuilder("JNITypes.cpp", () => JNIResources.JNITypes_cpp);
                yield return new StringConversionBuilder("JNITypesPrivate.h", () => JNIResources.JNITypesPrivate);
                yield return new StringConversionBuilder("JNITypes.h", () => JNIResources.JNITypes_h);
                yield return new StringConversionBuilder("JNIBoxes.cpp", () => JNIResources.JNIBoxes_cpp);
                yield return new StringConversionBuilder("JNIBoxes.h", () => JNIResources.JNIBoxes_h);
                yield return new StringConversionBuilder("JNIBoxesTemplate.h", () => JNIResources.JNIBoxesTemplate);
            }
        }
    }
}
