using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.Shared.Java
{
    public enum JavaInteropType
    {
        Boolean,
        Character,
        Byte,
        Short,
        Integer,
        Long,
        Float,
        Double,
        String,
    }

    public static class JavaSharedUtils
    {
        public static string ToJavaKeyword(JavaInteropType type)
        {
            switch (type)
            {
                case JavaInteropType.Boolean:
                    return "boolean";
                case JavaInteropType.Character:
                    return "char";
                case JavaInteropType.Byte:
                    return "byte";
                case JavaInteropType.Short:
                    return "short";
                case JavaInteropType.Integer:
                    return "int";
                case JavaInteropType.Long:
                    return "long";
                case JavaInteropType.Float:
                    return "float";
                case JavaInteropType.Double:
                    return "double";
                case JavaInteropType.String:
                    return "string";
                default:
                    throw new Exception();
            }
        }

        public static JavaInteropType ToInteropType(string javaKeyword)
        {
            switch (javaKeyword)
            {
                case "boolean":
                    return JavaInteropType.Boolean;
                case "char":
                    return JavaInteropType.Character;
                case "byte":
                    return JavaInteropType.Byte;
                case "short":
                    return JavaInteropType.Short;
                case "int":
                    return JavaInteropType.Integer;
                case "long":
                    return JavaInteropType.Long;
                case "float":
                    return JavaInteropType.Float;
                case "double":
                    return JavaInteropType.Double;
                case "string":
                    return JavaInteropType.String;
                default:
                    throw new Exception();
            }
        }
    }
}
