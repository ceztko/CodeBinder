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
    public class ConversionCSharpToJNI : LanguageConversion<JNICompilationContext, JNIModuleContext>
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

        public override IEnumerable<IConversionWriter> DefaultConversions
        {
            get
            {
                yield return new StringConversionWriter("JNIShared.h", () => JNIResources.JNIShared_h) { BasePath = "Internal", GeneratedPreamble = SourcePreamble };
                yield return new StringConversionWriter("JNIShared.cpp", () => JNIResources.JNIShared_cpp) { BasePath = "Internal", GeneratedPreamble = SourcePreamble };
                yield return new StringConversionWriter("JNITypes.cpp", () => JNIResources.JNITypes_cpp) { BasePath = "Internal", GeneratedPreamble = SourcePreamble };
                yield return new StringConversionWriter("JNITypesPrivate.h", () => JNIResources.JNITypesPrivate_h) { BasePath = "Internal", GeneratedPreamble = SourcePreamble };
                yield return new StringConversionWriter("JNITypesPrivate.cpp", () => JNIResources.JNITypesPrivate_cpp) { BasePath = "Internal", GeneratedPreamble = SourcePreamble };
                yield return new StringConversionWriter("JNITypes.h", () => JNIResources.JNITypes_h) { BasePath = "Internal", GeneratedPreamble = SourcePreamble };
                yield return new StringConversionWriter("JNIBoxes.cpp", () => JNIResources.JNIBoxes_cpp) { BasePath = "Internal", GeneratedPreamble = SourcePreamble };
                yield return new StringConversionWriter("JNIBoxes.h", () => JNIResources.JNIBoxes_h) { BasePath = "Internal", GeneratedPreamble = SourcePreamble };
                yield return new StringConversionWriter("JNIBinderUtils.cpp", () => JNIResources.JNIBinderUtils_cpp) { BasePath = "Internal", GeneratedPreamble = SourcePreamble };
                yield return new StringConversionWriter("JNICommon.h", () => JNIResources.JNICommon_h) { BasePath = "Internal", GeneratedPreamble = SourcePreamble };
                yield return new StringConversionWriter("JNICommon.cpp", () => JNIResources.JNICommon_cpp) { BasePath = "Internal", GeneratedPreamble = SourcePreamble };
            }
        }
    }
}
