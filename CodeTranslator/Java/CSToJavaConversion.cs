// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using System;
using System.Collections.Generic;
using CodeTranslator.Shared;
using CodeTranslator.Shared.CSharp;
using CodeTranslator.Shared.Java;
using CodeTranslator.Util;
using Microsoft.CodeAnalysis;

namespace CodeTranslator.Java
{
    public class CSToJavaConversion : CSharpLanguageConversion
    {
        internal const bool SkipBody = true;

        public const string GeneratedPreamble = "/* This file was generated. DO NOT EDIT! */";

        /// <summary>Base namespace of the package, to be set outside</summary>
        public string BaseNamespace { get; set; }

        public override TypeConversion<CSharpClassTypeContext> GetClassTypeConversion()
        {
            return new JavaClassConversion(this);
        }

        public override TypeConversion<CSharpInterfaceTypeContext> GetInterfaceTypeConversion()
        {
            return new JavaInterfaceConversion(this);
        }

        public override TypeConversion<CSharpStructTypeContext> GetStructTypeConversion()
        {
            return new JavaStructConversion(this);
        }

        public override TypeConversion<CSharpEnumTypeContext> GetEnumTypeConversion()
        {
            return new JavaEnumConversion(this);
        }

        public override IEnumerable<ConversionBuilder> DefaultConversions
        {
            get
            {
                yield return new InteropBoxBuilder(JavaInteropType.Boolean);
                yield return new InteropBoxBuilder(JavaInteropType.Character);
                yield return new InteropBoxBuilder(JavaInteropType.Byte);
                yield return new InteropBoxBuilder(JavaInteropType.Short);
                yield return new InteropBoxBuilder(JavaInteropType.Integer);
                yield return new InteropBoxBuilder(JavaInteropType.Long);
                yield return new InteropBoxBuilder(JavaInteropType.Float);
                yield return new InteropBoxBuilder(JavaInteropType.Double);
                yield return new InteropBoxBuilder(JavaInteropType.String);
            }
        }
    }
}
