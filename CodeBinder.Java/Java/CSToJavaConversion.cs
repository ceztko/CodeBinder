// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using System;
using System.Collections.Generic;
using CodeBinder.Shared;
using CodeBinder.Shared.CSharp;
using CodeBinder.Shared.Java;
using CodeBinder.Util;
using Microsoft.CodeAnalysis;

namespace CodeBinder.Java
{
    public class CSToJavaConversion : CSharpLanguageConversion
    {
        internal const bool SkipBody = false;

        internal const string SourcePreamble = "/* This file was generated. DO NOT EDIT! */";

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

        public override IEnumerable<string> CondictionaCompilationSymbols
        {
            get { yield return "JAVA"; }
        }

        public override bool UseUTF8Bom
        {
            get { return false; }
        }

        public override IEnumerable<ConversionBuilder> DefaultConversions
        {
            get
            {
                yield return new InteropBoxBuilder(JavaInteropType.Boolean, this);
                yield return new InteropBoxBuilder(JavaInteropType.Character, this);
                yield return new InteropBoxBuilder(JavaInteropType.Byte, this);
                yield return new InteropBoxBuilder(JavaInteropType.Short, this);
                yield return new InteropBoxBuilder(JavaInteropType.Integer, this);
                yield return new InteropBoxBuilder(JavaInteropType.Long, this);
                yield return new InteropBoxBuilder(JavaInteropType.Float, this);
                yield return new InteropBoxBuilder(JavaInteropType.Double, this);
                yield return new InteropBoxBuilder(JavaInteropType.String, this);
            }
        }
    }
}
