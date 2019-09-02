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
        internal const string CodeBinderNamespace = "CodeBinder";

        public NamespaceMappingTree NamespaceMapping { get; private set; }

        public bool SkipBody { get; set; }

        public bool MethodsLowerCase { get; set; }

        internal const string SourcePreamble = "/* This file was generated. DO NOT EDIT! */";

        public CSToJavaConversion()
        {
            NamespaceMapping = new NamespaceMappingTree();
            MethodsLowerCase = true;
        }

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
