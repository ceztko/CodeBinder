// SPDX-FileCopyrightText: (C) 2018 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT
using CodeBinder.JNI.Resources;

namespace CodeBinder.JNI;

[ConversionLanguageName("JNI")]
public class ConversionCSharpToJNI : CSharpLanguageConversionBase<JNICompilationContext, JNIModuleContext>
{
    internal const string SourcePreamble = "/* This file was generated. DO NOT EDIT! */";

    public ConversionCSharpToJNI() { }

    protected override JNICompilationContext CreateCompilationContext()
    {
        return new JNICompilationContext(this);
    }

    public override IReadOnlyList<string> PreprocessorDefinitions
    {
        get { return new string[] { "JVM", "JNI", "JNI_JDK", "JNI_ANDROID" }; }
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
