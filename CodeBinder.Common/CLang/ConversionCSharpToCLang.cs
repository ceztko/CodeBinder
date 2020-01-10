// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using System;
using System.Collections.Generic;
using System.Linq;
using CodeBinder.Shared;
using CodeBinder.CLang.Resources;
using CodeBinder.Util;

namespace CodeBinder.CLang
{
    public class ConversionCSharpToCLang : LanguageConversion<CLangCompilationContext, CLangSyntaxTreeContext, CLangModuleContext>
    {
        internal const string SourcePreamble = "/* This file was generated. DO NOT EDIT! */";

        public ConversionCSharpToCLang()
        {
        }

        protected override CLangCompilationContext createCompilationContext()
        {
            return new CLangCompilationContext(this);
        }

        public override IReadOnlyList<string> PreprocessorDefinitions
        {
            get { return new string[] { "CLang" }; }
        }

        public override IEnumerable<IConversionBuilder> DefaultConversions
        {
            get
            {
                yield return new StringConversionBuilder("BaseTypes.h", () => CLangResources.BaseTypes_h) { BasePath = "Internal" };
                yield return new StringConversionBuilder("cstrings.h", () => CLangResources.cstrings_h) { BasePath = "Internal" };
            }
        }

        
    }
}
