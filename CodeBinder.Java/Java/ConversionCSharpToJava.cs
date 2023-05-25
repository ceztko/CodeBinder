// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using System;
using System.Collections.Generic;
using CodeBinder.Shared;
using CodeBinder.Shared.CSharp;
using CodeBinder.Java.Shared;
using CodeBinder.Utils;
using Microsoft.CodeAnalysis;
using CodeBinder.Attributes;

namespace CodeBinder.Java
{
    [ConversionLanguageName("Java")]
    public class ConversionCSharpToJava : CSharpLanguageConversion
    {
        internal const string CodeBinderNamespace = "CodeBinder";

        internal const string SourcePreamble = "/* This file was generated. DO NOT EDIT! */";

        public ConversionCSharpToJava()
        {
        }

        public bool SkipBody { get; set; }

        public override MethodCasing MethodCasing => MethodCasing.LowerCamelCase;

        public override IReadOnlyCollection<string> SupportedPolicies => new string[] { Policies.GarbageCollection, Policies.InstanceFinalizers };

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

        public override IEnumerable<TypeConversion<CSharpDelegateTypeContext>> GetConversions(CSharpDelegateTypeContext dlg)
        {
            yield break;
        }

        public override IReadOnlyList<string> PreprocessorDefinitions
        {
            get { return new string[] { "JAVA", "JVM" }; }
        }

        public override bool UseUTF8Bom
        {
            get { return false; }
        }

        public override IEnumerable<IConversionWriter> DefaultConversions
        {
            get
            {
                yield return new JavaVerbatimConversionWriter(nameof(JavaClasses.BinderUtils), JavaClasses.BinderUtils, "CodeBinder.Java");
                yield return new JavaVerbatimConversionWriter(nameof(JavaClasses.HandleRef), JavaClasses.HandleRef);
                yield return new JavaVerbatimConversionWriter(nameof(JavaClasses.NativeHandle), JavaClasses.NativeHandle);
                yield return new JavaVerbatimConversionWriter(nameof(JavaClasses.FinalizableObject), JavaClasses.FinalizableObject);
                yield return new JavaVerbatimConversionWriter(nameof(JavaClasses.HandledObjectBase), JavaClasses.HandledObjectBase);
                yield return new JavaVerbatimConversionWriter(nameof(JavaClasses.HandledObject), JavaClasses.HandledObject);
                yield return new JavaVerbatimConversionWriter(nameof(JavaClasses.HandledObjectFinalizer), JavaClasses.HandledObjectFinalizer);
                yield return new JavaVerbatimConversionWriter(nameof(JavaClasses.IObjectFinalizer), JavaClasses.IObjectFinalizer);
                yield return new JavaInteropBoxWriter(JavaInteropType.Boolean, this);
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
