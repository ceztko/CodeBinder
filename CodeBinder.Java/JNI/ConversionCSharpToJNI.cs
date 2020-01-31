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
    [ConversionLanguageName("JNI")]
    public class ConversionCSharpToJNI : LanguageConversion<JNICompilationContext, JNISyntaxTreeContext, JNIModuleContext>
    {
        internal const string SourcePreamble = "/* This file was generated. DO NOT EDIT! */";

        public ConversionCSharpToJNI() { }

        protected override JNICompilationContext createCompilationContext()
        {
            return new JNICompilationContext(this);
        }

        public override IReadOnlyList<string> PreprocessorDefinitions
        {
            get { return new string[] { "JAVA", "JNI" }; }
        }

        public override IEnumerable<IConversionBuilder> DefaultConversions
        {
            get
            {
                yield return new StringConversionBuilder("JNIShared.h", () => JNIResources.JNIShared_h) { BasePath = "Internal", GeneratedPreamble = SourcePreamble };
                yield return new StringConversionBuilder("JNIShared.cpp", () => JNIResources.JNIShared_cpp) { BasePath = "Internal", GeneratedPreamble = SourcePreamble };
                yield return new StringConversionBuilder("JNITypes.cpp", () => JNIResources.JNITypes_cpp) { BasePath = "Internal", GeneratedPreamble = SourcePreamble };
                yield return new StringConversionBuilder("JNITypesPrivate.h", () => JNIResources.JNITypesPrivate_h) { BasePath = "Internal", GeneratedPreamble = SourcePreamble };
                yield return new StringConversionBuilder("JNITypesPrivate.cpp", () => JNIResources.JNITypesPrivate_cpp) { BasePath = "Internal", GeneratedPreamble = SourcePreamble };
                yield return new StringConversionBuilder("JNITypes.h", () => JNIResources.JNITypes_h) { BasePath = "Internal", GeneratedPreamble = SourcePreamble };
                yield return new StringConversionBuilder("JNIBoxes.cpp", () => JNIResources.JNIBoxes_cpp) { BasePath = "Internal", GeneratedPreamble = SourcePreamble };
                yield return new StringConversionBuilder("JNIBoxes.h", () => JNIResources.JNIBoxes_h) { BasePath = "Internal", GeneratedPreamble = SourcePreamble };
                yield return new StringConversionBuilder("JNIBoxesTemplate.h", () => JNIResources.JNIBoxesTemplate_h) { BasePath = "Internal", GeneratedPreamble = SourcePreamble };
                yield return new StringConversionBuilder("JNIBinderUtils.cpp", () => JNIResources.JNIBinderUtils_cpp) { BasePath = "Internal", GeneratedPreamble = SourcePreamble };
            }
        }
    }
}
