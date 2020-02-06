using CodeBinder.Shared;
using CodeBinder.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Java
{
    class JavaVerbatimConversionWriter : JavaConversionWriterBase
    {
        public string ClassName { get; private set; }
        public string ClassCode { get; private set; }

        public JavaVerbatimConversionWriter(string className, string classCode)
        {
            ClassName = className;
            ClassCode = classCode;
        }

        protected override void write(CodeBuilder builder)
        {
            builder.Append("package").Space().Append(ConversionCSharpToJava.CodeBinderNamespace).EndOfStatement();
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
