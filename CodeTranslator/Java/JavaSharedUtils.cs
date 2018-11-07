using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.Shared.Java
{
    public enum JavaPrimitiveType
    {
        Boolean,
        Character,
        Byte,
        Short,
        Integer,
        Float,
        Double,
    }

    public static class JavaSharedUtils
    {
        public static string ToJavaKeyword(JavaPrimitiveType type)
        {
            switch (type)
            {
                case JavaPrimitiveType.Boolean:
                    return "boolean";
                case JavaPrimitiveType.Character:
                    return "char";
                case JavaPrimitiveType.Byte:
                    return "byte";
                case JavaPrimitiveType.Short:
                    return "short";
                case JavaPrimitiveType.Integer:
                    return "int";
                case JavaPrimitiveType.Float:
                    return "float";
                case JavaPrimitiveType.Double:
                    return "double";
                default:
                    throw new Exception();
            }
        }

        public static JavaPrimitiveType ToPrimitiveType(string keyword)
        {
            switch (keyword)
            {
                case "boolean":
                    return JavaPrimitiveType.Boolean;
                case "char":
                    return JavaPrimitiveType.Character;
                case "byte":
                    return JavaPrimitiveType.Byte;
                case "short":
                    return JavaPrimitiveType.Short;
                case "int":
                    return JavaPrimitiveType.Integer;
                case "float":
                    return JavaPrimitiveType.Float;
                case "double":
                    return JavaPrimitiveType.Double;
                default:
                    throw new Exception();
            }
        }
    }
}
