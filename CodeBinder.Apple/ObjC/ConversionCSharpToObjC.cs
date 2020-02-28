// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using System.Collections.Generic;
using CodeBinder.Shared;
using CodeBinder.Shared.CSharp;
using CodeBinder.Util;
using BinderPolicies = CodeBinder.Attributes.Policies;

namespace CodeBinder.Apple
{
    [ConversionLanguageName("ObjetiveC")]
    public class ConversionCSharpToObjC : CSharpLanguageConversion<ObjCCompilationContext>
    {
        internal const string SourcePreamble = "/* This file was generated. DO NOT EDIT! */";

        internal const string HeaderExtension = "h";
        internal const string ImplementationExtension = "mm";
        internal const string ConversionPrefix = "OC";
        internal const string TypesHeader = "OCTypes.h";
        internal const string BaseTypesHeader = "CBBaseTypes.h";
        internal const string InternalBasePath = "Internal";

        List<string> _policies;

        public ConversionCSharpToObjC()
        {
            _policies = new List<string>() { /* BinderPolicies.PassByRef, */ BinderPolicies.Delegates };
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
                yield return new StringConversionWriter(nameof(ObjCClasses.CBException_h).ToObjCHeaderFilename(), () => ObjCClasses.CBException_h) { BasePath = InternalBasePath, GeneratedPreamble = SourcePreamble };
                yield return new StringConversionWriter(nameof(ObjCClasses.CBException_mm).ToObjCImplementationFilename(), () => ObjCClasses.CBException_mm) { BasePath = InternalBasePath, GeneratedPreamble = SourcePreamble };
                yield return new StringConversionWriter(nameof(ObjCClasses.CBIEqualityCompararer_h).ToObjCHeaderFilename(), () => ObjCClasses.CBIEqualityCompararer_h) { BasePath = InternalBasePath, GeneratedPreamble = SourcePreamble };
                yield return new StringConversionWriter(nameof(ObjCClasses.CBIReadOnlyList_h).ToObjCHeaderFilename(), () => ObjCClasses.CBIReadOnlyList_h) { BasePath = InternalBasePath, GeneratedPreamble = SourcePreamble };
                yield return new StringConversionWriter(nameof(ObjCClasses.CBIDisposable_h).ToObjCHeaderFilename(), () => ObjCClasses.CBIDisposable_h) { BasePath = InternalBasePath, GeneratedPreamble = SourcePreamble };
                yield return new StringConversionWriter(nameof(ObjCClasses.CBKeyValuePair_h).ToObjCHeaderFilename(), () => ObjCClasses.CBKeyValuePair_h) { BasePath = InternalBasePath, GeneratedPreamble = SourcePreamble };
                yield return new StringConversionWriter(nameof(ObjCClasses.CBKeyValuePair_mm).ToObjCImplementationFilename(), () => ObjCClasses.CBKeyValuePair_mm) { BasePath = InternalBasePath, GeneratedPreamble = SourcePreamble };
                yield return new StringConversionWriter(nameof(ObjCClasses.CBHandleRef_h).ToObjCHeaderFilename(), () => ObjCClasses.CBHandleRef_h) { BasePath = InternalBasePath, GeneratedPreamble = SourcePreamble };
                yield return new StringConversionWriter(nameof(ObjCClasses.CBHandleRef_mm).ToObjCImplementationFilename(), () => ObjCClasses.CBHandleRef_mm) { BasePath = InternalBasePath, GeneratedPreamble = SourcePreamble };
                foreach (var type in ObjCUtils.GetInteropTypes())
                {
                    yield return new ObjCArrayBoxWriter(type, true);
                    yield return new ObjCArrayBoxWriter(type, false);
                }

                ////yield return new StringConversionBuilder($"{nameof(ObjCClasses.BinderUtils)}.h", () => ObjCClasses.BinderUtils) { BasePath = InternalBasePath, GeneratedPreamble = SourcePreamble };
            }
        }
    }
}
