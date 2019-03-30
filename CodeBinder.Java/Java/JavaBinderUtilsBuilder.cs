using CodeBinder.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Java
{
    class JavaBinderUtilsBuilder : JavaConversionBuilder
    {
        public JavaBinderUtilsBuilder(CSToJavaConversion conversion)
            : base(conversion) { }

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
            get { return "BinderUtils.java"; }
        }

        const string ClassCode =
@"public class BinderUtils
{
    // Simulates as operator https://stackoverflow.com/a/148949/213871
    public static <T> T as(Object obj, Class<T> clazz)
    {
        if (clazz.isInstance(obj))
            return clazz.cast(obj);

        return null;
    }

    public static boolean equals(String lhs, String rhs)
    {
        if (lhs == null)
        {
            if (rhs == null)
                return true;
            else
                return false;
        }
        else
        {
            return lhs.equals(rhs);
        }
    }
}
";
    }
}
