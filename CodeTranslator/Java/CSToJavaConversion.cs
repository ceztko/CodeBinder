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
                yield return new PrimitiveBoxBuilder(JavaPrimitiveType.Boolean);
                yield return new PrimitiveBoxBuilder(JavaPrimitiveType.Character);
                yield return new PrimitiveBoxBuilder(JavaPrimitiveType.Byte);
                yield return new PrimitiveBoxBuilder(JavaPrimitiveType.Short);
                yield return new PrimitiveBoxBuilder(JavaPrimitiveType.Integer);
                yield return new PrimitiveBoxBuilder(JavaPrimitiveType.Long);
                yield return new PrimitiveBoxBuilder(JavaPrimitiveType.Float);
                yield return new PrimitiveBoxBuilder(JavaPrimitiveType.Double);
            }
        }
    }
}
