// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using System;
using System.Collections.Generic;
using System.Linq;
using CodeTranslator.Shared;

namespace CodeTranslator.JNI
{
    public class CSToJNIConversion : LanguageConversion<JNISyntaxTreeContext, JNIModuleContext>
    {
        /// <summary>Base namespace of the package, to be set outside</summary>
        public string BaseNamespace { get; set; }

        protected override JNISyntaxTreeContext getSyntaxTreeContext(CompilationContext compilation)
        {
            return new JNISyntaxTreeContext(compilation, this);
        }
    }
}
