// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using System;
using System.Collections.Generic;
using CodeBinder.Shared;
using CodeBinder.Shared.CSharp;
using CodeBinder.Java.Shared;
using CodeBinder.Util;
using Microsoft.CodeAnalysis;

namespace CodeBinder.Java
{
    [ConversionLanguageName("Java")]
    public class ConversionCSharpToJava : CSharpLanguageConversion
    {
        internal const string CodeBinderNamespace = "CodeBinder";

        public bool SkipBody { get; set; }

        public bool MethodsLowerCase { get; set; }

        internal const string SourcePreamble = "/* This file was generated. DO NOT EDIT! */";

        public ConversionCSharpToJava()
        {
            MethodsLowerCase = true;
        }

        public override TypeConversion<CSharpClassTypeContext> CreateConversion(CSharpClassTypeContext cls)
        {
            return new JavaClassConversion(cls, this);
        }

        public override TypeConversion<CSharpInterfaceTypeContext> CreateConversion(CSharpInterfaceTypeContext iface)
        {
            return new JavaInterfaceConversion(iface, this);
        }

        public override TypeConversion<CSharpStructTypeContext> CreateConversion(CSharpStructTypeContext str)
        {
            return new JavaStructConversion(str, this);
        }

        public override TypeConversion<CSharpEnumTypeContext> CreateConversion(CSharpEnumTypeContext enm)
        {
            return new JavaEnumConversion(enm, this);
        }

        public override IReadOnlyList<string> PreprocessorDefinitions
        {
            get { return new string[] { "JAVA" }; }
        }

        public override bool UseUTF8Bom
        {
            get { return false; }
        }

        public override IEnumerable<IConversionBuilder> DefaultConversions
        {
            get
            {
                yield return new JavaClassBuilder(this, nameof(JavaClasses.BinderUtils), JavaClasses.BinderUtils);
                yield return new JavaClassBuilder(this, nameof(JavaClasses.HandleRef), JavaClasses.HandleRef);
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
