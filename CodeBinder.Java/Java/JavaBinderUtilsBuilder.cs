using CodeBinder.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Java
{
    class JavaClassBuilder : JavaConversionBuilder
    {
        public string ClassName { get; private set; }
        public string ClassCode { get; private set; }

        public JavaClassBuilder(ConversionCSharpToJava conversion, string className, string classCode)
            : base(conversion)
        {
            ClassName = className;
            ClassCode = classCode;
        }

        public override void write(CodeBuilder builder)
        {
            builder.Append("package").Space().Append(ConversionCSharpToJava.CodeBinderNamespace).EndOfStatement();
            builder.AppendLine();
            builder.Append(ClassCode);
        }

        protected override string GetBasePath()
        {
            return ConversionCSharpToJava.CodeBinderNamespace;
        }

        public override string FileName
        {
            get { return ClassName + ".java"; }
        }
    }
}
