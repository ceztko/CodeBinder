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

        public override IEnumerable<TypeConversion<CSharpClassTypeContext>> GetConversions(CSharpClassTypeContext cls)
        {
            yield return new JavaClassConversion(cls, this);
        }

        public override IEnumerable<TypeConversion<CSharpInterfaceTypeContext>> GetConversions(CSharpInterfaceTypeContext iface)
        {
            yield return new JavaInterfaceConversion(iface, this);
        }

        public override IEnumerable<TypeConversion<CSharpStructTypeContext>> GetConversions(CSharpStructTypeContext str)
        {
            yield return new JavaStructConversion(str, this);
        }

        public override IEnumerable<TypeConversion<CSharpEnumTypeContext>> GetConversions(CSharpEnumTypeContext enm)
        {
            yield return new JavaEnumConversion(enm, this);
        }

        public override IReadOnlyList<string> PreprocessorDefinitions
        {
            get { return new string[] { "JAVA" }; }
        }

        public override bool UseUTF8Bom
        {
            get { return false; }
        }

        public override IEnumerable<IConversionWriter> DefaultConversions
        {
            get
            {
                yield return new JavaVerbatimConversionWriter(nameof(JavaClasses.BinderUtils), JavaClasses.BinderUtils);
                yield return new JavaVerbatimConversionWriter(nameof(JavaClasses.HandleRef), JavaClasses.HandleRef);
                yield return new JavaVerbatimConversionWriter(nameof(JavaClasses.HandledObjectBase), JavaClasses.HandledObjectBase);
                yield return new JavaVerbatimConversionWriter(nameof(JavaClasses.HandledObject), JavaClasses.HandledObject);
                yield return new JavaInteropBoxWriter(JavaInteropType.Boolean, this);
                yield return new JavaInteropBoxWriter(JavaInteropType.Character, this);
                yield return new JavaInteropBoxWriter(JavaInteropType.Byte, this);
                yield return new JavaInteropBoxWriter(JavaInteropType.Short, this);
                yield return new JavaInteropBoxWriter(JavaInteropType.Integer, this);
                yield return new JavaInteropBoxWriter(JavaInteropType.Long, this);
                yield return new JavaInteropBoxWriter(JavaInteropType.Float, this);
                yield return new JavaInteropBoxWriter(JavaInteropType.Double, this);
                yield return new JavaInteropBoxWriter(JavaInteropType.String, this);
            }
        }
    }
}
