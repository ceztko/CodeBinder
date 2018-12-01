using CodeBinder.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Java
{
    class JavaBinderUtilsBuilder : ConversionBuilder
    {
        CSToJavaConversion _conversion;

        public JavaBinderUtilsBuilder(CSToJavaConversion conversion)
        {
            _conversion = conversion;
        }

        public override void Write(CodeBuilder builder)
        {
            builder.Append("package").Space().Append(_conversion.BaseNamespace).EndOfStatement();
            builder.AppendLine();
            builder.Append(ClassCode);
        }

        public override string FileName
        {
            get { return "BinderUtils.java"; }
        }

        const string ClassCode =
@"class BinderUtils
{
    // Simulates as operator https://stackoverflow.com/a/148949/213871
    public static <T> T as(Object obj, Class<T> clazz)
    {
        if (clazz.isInstance(obj))
            return clazz.cast(obj);

        return null;
    }
}
";
    }
}
