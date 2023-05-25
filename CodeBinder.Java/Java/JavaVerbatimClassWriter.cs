// Copyright(c) 2020 Francesco Pretto
// This file is subject to the MIT license
using CodeBinder.Shared;
using CodeBinder.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Java
{
    class JavaVerbatimConversionWriter : JavaConversionWriterBase
    {
        public string ClassName { get; private set; }
        public string ClassCode { get; private set; }

        public string? Namespace { get; private set; }

        public JavaVerbatimConversionWriter(string className, string classCode, string? ns = null)
        {
            ClassName = className;
            ClassCode = classCode;
            Namespace = ns;
        }

        protected override void write(CodeBuilder builder)
        {
            builder.Append("package").Space().Append(Namespace ?? ConversionCSharpToJava.CodeBinderNamespace).EndOfStatement();
            builder.AppendLine();
            builder.Append(ClassCode);
        }

        protected override string GetBasePath()
        {
            return ConversionCSharpToJava.CodeBinderNamespace;
        }

        protected override string GetFileName() => $"{ClassName}.java";
    }
}
