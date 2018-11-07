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
        /// <summary>Base namespace of the package, to be set outside</summary>
        public string BaseNamespace { get; set; }

        protected override JNISyntaxTreeContext getSyntaxTreeContext(CompilationContext compilation)
        {
            return new JNISyntaxTreeContext(compilation, this);
        }

        public override IEnumerable<ConversionBuilder> DefaultConversions
        {
            get
            {
                yield return new StringConversionBuilder("JNITypes.h", () => JNIResources.JNITypes);
                yield return new StringConversionBuilder("JNITypesPrivate.h", () => JNIResources.JNITypesPrivate);
                yield return new StringConversionBuilder("JNIBoxes.cpp", () => JNIResources.JNIBoxes_cpp);
                yield return new StringConversionBuilder("JNIBoxes.h", () => JNIResources.JNIBoxes_h);
                yield return new StringConversionBuilder("JNIBoxesTemplate.h", () => JNIResources.JNIBoxesTemplate);
            }
        }
    }
}
