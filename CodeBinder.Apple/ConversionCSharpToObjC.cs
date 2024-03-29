﻿// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT
using CodeBinder.Attributes;
using BinderPolicies = CodeBinder.Attributes.Features;

namespace CodeBinder.Apple;

[ConversionLanguageName("ObjectiveC")]
public class ConversionCSharpToObjC : CSharpLanguageConversion<ObjCCompilationContext>
{
    internal const string SourcePreamble = "/* This file was generated. DO NOT EDIT! */";

    internal const string HeaderExtension = "h";
    internal const string ImplementationExtension = "mm";
    internal const string ConversionPrefix = "OC";
    internal const string TypesHeader = "OCTypes.h";
    internal const string BaseTypesHeader = "CBOCBaseTypes.h";
    internal const string InternalBasePath = "Internal";
    internal const string SupportBasePath = "Support";

    public ConversionCSharpToObjC() { }

    public override string GetMethodBaseName(IMethodSymbol symbol)
    {
        if (symbol.MethodKind == MethodKind.Constructor)
            return "init";
        else
            return base.GetMethodBaseName(symbol);
    }

    protected override ObjCCompilationContext CreateCSharpCompilationContext()
    {
        return new ObjCCompilationContext(this);
    }

    // NOTE: Delegates are not yet fully supported, PassByRef doesn't work perfectly as well
    // _policies = new List<string>() { BinderPolicies.PassByRef, BinderPolicies.PassByRef, BinderPolicies.Delegates };
    public override IReadOnlyCollection<string> SupportedPolicies =>
        new string[] { BinderPolicies.InstanceFinalizers};

    public override OverloadFeature? OverloadFeatures => OverloadFeature.ParameterArity;

    public override bool NeedNamespaceMapping => false;

    public override bool DiscardNative => true;

    public bool SkipBody { get; set; }

    public override MethodCasing MethodCasing => MethodCasing.LowerCamelCase;

    public override IReadOnlyList<string> PreprocessorDefinitions
    {
        get { return new string[] { "OBJECTIVEC", "APPLE" }; }
    }

    public override IEnumerable<IConversionWriter> DefaultConversions
    {
        get
        {
            yield return new ObjCBaseTypesHeaderConversion();
            yield return new StringConversionWriter(nameof(ObjCResources.CBHandledObject_Internal_h).ToObjCHeaderFilename(), () => ObjCResources.CBHandledObject_Internal_h) { BasePath = InternalBasePath, GeneratedPreamble = SourcePreamble };
            yield return new StringConversionWriter(nameof(ObjCResources.CBHandledObject_mm).ToObjCImplementationFilename(), () => ObjCResources.CBHandledObject_mm) { BasePath = SupportBasePath, GeneratedPreamble = SourcePreamble };
            yield return new StringConversionWriter(nameof(ObjCResources.CBOCInterop_h).ToObjCHeaderFilename(), () => ObjCResources.CBOCInterop_h) { BasePath = InternalBasePath, GeneratedPreamble = SourcePreamble };
            yield return new StringConversionWriter(nameof(ObjCResources.CBOCBinderUtils_h).ToObjCHeaderFilename(), () => ObjCResources.CBOCBinderUtils_h) { BasePath = InternalBasePath, GeneratedPreamble = SourcePreamble };
            yield return new StringConversionWriter(nameof(ObjCResources.CBOCBinderUtils_mm).ToObjCImplementationFilename(), () => ObjCResources.CBOCBinderUtils_mm) { BasePath = InternalBasePath, GeneratedPreamble = SourcePreamble };
            yield return new StringConversionWriter(nameof(ObjCClasses.CBException_mm).ToObjCImplementationFilename(), () => ObjCClasses.CBException_mm) { BasePath = SupportBasePath, GeneratedPreamble = SourcePreamble };
            yield return new StringConversionWriter(nameof(ObjCClasses.CBIEqualityCompararer_h).ToObjCHeaderFilename(), () => ObjCClasses.CBIEqualityCompararer_h) { BasePath = SupportBasePath, GeneratedPreamble = SourcePreamble };
            yield return new StringConversionWriter(nameof(ObjCClasses.CBIReadOnlyList_h).ToObjCHeaderFilename(), () => ObjCClasses.CBIReadOnlyList_h) { BasePath = SupportBasePath, GeneratedPreamble = SourcePreamble };
            yield return new StringConversionWriter(nameof(ObjCClasses.CBIDisposable_h).ToObjCHeaderFilename(), () => ObjCClasses.CBIDisposable_h) { BasePath = SupportBasePath, GeneratedPreamble = SourcePreamble };
            yield return new StringConversionWriter(nameof(ObjCClasses.CBKeyValuePair_mm).ToObjCImplementationFilename(), () => ObjCClasses.CBKeyValuePair_mm) { BasePath = SupportBasePath, GeneratedPreamble = SourcePreamble };
            yield return new StringConversionWriter(nameof(ObjCClasses.CBHandleRef_mm).ToObjCImplementationFilename(), () => ObjCClasses.CBHandleRef_mm) { BasePath = SupportBasePath, GeneratedPreamble = SourcePreamble };
        }
    }
}
