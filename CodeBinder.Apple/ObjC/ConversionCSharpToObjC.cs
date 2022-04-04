// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using System.Collections.Generic;
using CodeBinder.Shared;
using CodeBinder.Shared.CSharp;
using CodeBinder.Util;
using BinderPolicies = CodeBinder.Attributes.Policies;

namespace CodeBinder.Apple
{
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

        List<string> _policies;

        public ConversionCSharpToObjC()
        {
            // NOTE: Delegates are not yet fully supported, PassByRef doesn't work perfectly as well
            // _policies = new List<string>() { BinderPolicies.PassByRef, BinderPolicies.PassByRef, BinderPolicies.Delegates, BinderPolicies.ExplicitInterfaceImplementation };
            _policies = new List<string>() { BinderPolicies.ExplicitInterfaceImplementation };
            MethodsLowerCase = true;
        }

        protected override ObjCCompilationContext createCSharpCompilationContext()
        {
            return new ObjCCompilationContext(this);
        }

        public override IReadOnlyCollection<string> SupportedPolicies
        {
            get { return _policies; }
        }

        public override bool NeedNamespaceMapping => false;

        public override bool DiscardNative => true;

        public bool SkipBody { get; set; }

        public bool MethodsLowerCase { get; set; }

        public override IReadOnlyList<string> PreprocessorDefinitions
        {
            get { return new string[] { "OBJECTIVEC", "APPLE" }; }
        }

        public override IEnumerable<IConversionWriter> DefaultConversions
        {
            get
            {
                yield return new ObjCBaseTypesHeaderConversion();
                yield return new StringConversionWriter(nameof(ObjCResources.CBOCInterop_h).ToObjCHeaderFilename(), () => ObjCResources.CBOCInterop_h) { BasePath = InternalBasePath, GeneratedPreamble = SourcePreamble };
                yield return new StringConversionWriter(nameof(ObjCResources.CBOCBinderUtils_h).ToObjCHeaderFilename(), () => ObjCResources.CBOCBinderUtils_h) { BasePath = InternalBasePath, GeneratedPreamble = SourcePreamble };
                yield return new StringConversionWriter(nameof(ObjCResources.cboclibdefs_h).ToObjCHeaderFilename(), () => ObjCResources.cboclibdefs_h) { BasePath = SupportBasePath, GeneratedPreamble = SourcePreamble };
                yield return new StringConversionWriter(nameof(ObjCClasses.CBException_h).ToObjCHeaderFilename(), () => ObjCClasses.CBException_h) { BasePath = SupportBasePath, GeneratedPreamble = SourcePreamble };
                yield return new StringConversionWriter(nameof(ObjCClasses.CBException_mm).ToObjCImplementationFilename(), () => ObjCClasses.CBException_mm) { BasePath = SupportBasePath, GeneratedPreamble = SourcePreamble };
                yield return new StringConversionWriter(nameof(ObjCClasses.CBIEqualityCompararer_h).ToObjCHeaderFilename(), () => ObjCClasses.CBIEqualityCompararer_h) { BasePath = SupportBasePath, GeneratedPreamble = SourcePreamble };
                yield return new StringConversionWriter(nameof(ObjCClasses.CBIReadOnlyList_h).ToObjCHeaderFilename(), () => ObjCClasses.CBIReadOnlyList_h) { BasePath = SupportBasePath, GeneratedPreamble = SourcePreamble };
                yield return new StringConversionWriter(nameof(ObjCClasses.CBIDisposable_h).ToObjCHeaderFilename(), () => ObjCClasses.CBIDisposable_h) { BasePath = SupportBasePath, GeneratedPreamble = SourcePreamble };
                yield return new StringConversionWriter(nameof(ObjCClasses.CBKeyValuePair_h).ToObjCHeaderFilename(), () => ObjCClasses.CBKeyValuePair_h) { BasePath = SupportBasePath, GeneratedPreamble = SourcePreamble };
                yield return new StringConversionWriter(nameof(ObjCClasses.CBKeyValuePair_mm).ToObjCImplementationFilename(), () => ObjCClasses.CBKeyValuePair_mm) { BasePath = SupportBasePath, GeneratedPreamble = SourcePreamble };
                yield return new StringConversionWriter(nameof(ObjCClasses.CBHandleRef_h).ToObjCHeaderFilename(), () => ObjCClasses.CBHandleRef_h) { BasePath = SupportBasePath, GeneratedPreamble = SourcePreamble };
                yield return new StringConversionWriter(nameof(ObjCClasses.CBHandleRef_mm).ToObjCImplementationFilename(), () => ObjCClasses.CBHandleRef_mm) { BasePath = SupportBasePath, GeneratedPreamble = SourcePreamble };
                foreach (var type in ObjCUtils.GetInteropTypes())
                {
                    yield return new ObjCArrayBoxWriter(type, false);
                    yield return new ObjCArrayBoxWriter(type, true);
                }
            }
        }
    }
}
