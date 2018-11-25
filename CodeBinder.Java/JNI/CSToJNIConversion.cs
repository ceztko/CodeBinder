// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using System;
using System.Collections.Generic;
using System.Linq;
using CodeBinder.Shared;
using CodeBinder.JNI.Resources;
using CodeBinder.Util;

namespace CodeBinder.JNI
{
    public class CSToJNIConversion : LanguageConversion<JNICompilationContext, JNISyntaxTreeContext, JNIModuleContext>
    {
        internal const string SourcePreamble = "/* This file was generated. DO NOT EDIT! */";

        /// <summary>Base namespace of the package, to be set outside</summary>
        public string BaseNamespace { get; set; }

        protected override JNICompilationContext createCompilationContext()
        {
            return new JNICompilationContext(this);
        }

        public override IEnumerable<string> CondictionaCompilationSymbols
        {
            get { yield return "JNI"; }
        }

        public override IEnumerable<ConversionBuilder> DefaultConversions
        {
            get
            {
                yield return new StringConversionBuilder("JNITypes.cpp", () => JNIResources.JNITypes_cpp);
                yield return new StringConversionBuilder("JNITypesPrivate.h", () => JNIResources.JNITypesPrivate_h);
                yield return new StringConversionBuilder("JNITypes.h", () => JNIResources.JNITypes_h);
                yield return new StringConversionBuilder("JNIBoxes.cpp", () => JNIResources.JNIBoxes_cpp);
                yield return new StringConversionBuilder("JNIBoxes.h", () => JNIResources.JNIBoxes_h);
                yield return new StringConversionBuilder("JNIBoxesTemplate.h", () => JNIResources.JNIBoxesTemplate_h);
            }
        }
    }
}
