// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using System;
using System.Collections.Generic;
using System.Linq;
using CodeBinder.Attributes;
using CodeBinder.Shared;
using CodeBinder.Util;

namespace CodeBinder.CLang
{
    /// <summary>
    /// Basic CSharp language conversion
    /// </summary>
    /// <remarks>Inherit this class to provide custom contexts</remarks>
    [ConversionLanguageName(LanguageName)]
    public class ConversionCSharpToCLang : LanguageConversion<CLangCompilationContext, CLangSyntaxTreeContext, CLangModuleContext>
    {
        internal const string SourcePreamble = "/* This file was generated. DO NOT EDIT! */";
        public const string LanguageName = "C";

        public ConversionCSharpToCLang() { }

        public override bool IsNative => true;

        public override IReadOnlyCollection<string> SupportedPolicies => new[] { Policies.Delegates };

        protected override CLangCompilationContext createCompilationContext()
        {
            return new CLangCompilationContextImpl(this);
        }

        public override IReadOnlyList<string> PreprocessorDefinitions
        {
            get { return new string[] { "CLang" }; }
        }

        internal const string BaseTypesHeader = "CBBaseTypes.h";

        public override IEnumerable<IConversionWriter> DefaultConversions
        {
            get
            {
                yield return new StringConversionWriter(BaseTypesHeader, () => CLangResources.CBBaseTypes_h) { GeneratedPreamble = SourcePreamble };
                yield return new StringConversionWriter("CBInterop.h", () => CLangResources.CBInterop_h) { GeneratedPreamble = SourcePreamble };
                yield return new StringConversionWriter("CBInterop.h", () => CLangResources.CBInteropInternal_h) { BasePath = "Internal", GeneratedPreamble = SourcePreamble };
            }
        }
    }
}
