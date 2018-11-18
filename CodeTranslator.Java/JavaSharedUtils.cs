using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.Shared.Java
{
    public enum ParenthesisDirection
    {
        Left,
        Right
    }

    public enum ParenthesisType
    {
        Round,
        Square,
        Angle,
        Brace,
    }

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

    enum JavaTypeFlags
    {
        None = 0,
        NativeMethod,
        IsByRef
    }

    public static class JavaUtils
    {
        public static string ToJavaKeyword(this JavaInteropType type)
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

        public static string ToString(this ParenthesisType type, ParenthesisDirection direction)
        {
            switch (direction)
            {
                case ParenthesisDirection.Left:
                {
                    switch (type)
                    {
                        case ParenthesisType.Round:
                            return "(";
                        case ParenthesisType.Square:
                            return "[";
                        case ParenthesisType.Angle:
                            return "<";
                        case ParenthesisType.Brace:
                            return "{";
                        default:
                            throw new Exception();
                    }
                }
                case ParenthesisDirection.Right:
                {
                    switch (type)
                    {
                        case ParenthesisType.Round:
                            return ")";
                        case ParenthesisType.Square:
                            return "]";
                        case ParenthesisType.Angle:
                            return ">";
                        case ParenthesisType.Brace:
                            return "}";
                        default:
                            throw new Exception();
                    }
                }
                default:
                    throw new Exception();
            }
        }
    }
}
