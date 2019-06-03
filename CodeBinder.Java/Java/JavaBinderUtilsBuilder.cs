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

        public JavaClassBuilder(CSToJavaConversion conversion, string className, string classCode)
            : base(conversion)
        {
            ClassName = className;
            ClassCode = classCode;
        }

        public override void Write(CodeBuilder builder)
        {
            builder.Append("package").Space().Append(CSToJavaConversion.CodeBinderNamespace).EndOfStatement();
            builder.AppendLine();
            builder.Append(ClassCode);
        }

        public override string BasePath
        {
            get { return CSToJavaConversion.CodeBinderNamespace; }
        }

        public override string FileName
        {
            get { return ClassName + ".java"; }
        }
    }
}
