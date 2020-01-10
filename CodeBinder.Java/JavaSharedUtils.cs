using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Shared.Java
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

    [Flags]
    enum JavaTypeFlags
    {
        None = 0,
        NativeMethod = 1,
        IsByRef = 2,
    }

    public static class JavaUtils
    {
        public static string GetJavaBoxType(string typeName)
        {
            string ret;
            if (GetJavaBoxType(typeName, out ret))
                return ret;
            else
                throw new Exception("Unsupported java box type for " + typeName);
        }

        public static bool GetJavaBoxType(string typeName, out string boxTypeName)
        {
            switch (typeName)
            {
                case "System.IntPtr":
                    boxTypeName = "Long";
                    return true;
                case "System.Boolean":
                    boxTypeName = "Boolean";
                    return true;
                case "System.Char":
                    boxTypeName = "Character";
                    return true;
                case "System.Byte":
                case "System.SByte":
                    boxTypeName = "Byte";
                    return true;
                case "System.Int16":
                case "System.UInt16":
                    boxTypeName = "Short";
                    return true;
                case "System.Int32":
                case "System.UInt32":
                    boxTypeName = "Integer";
                    return true;
                case "System.Int64":
                case "System.UInt64":
                    boxTypeName = "Long";
                    return true;
                case "System.Single":
                    boxTypeName = "Float";
                    return true;
                case "System.Double":
                    boxTypeName = "Double";
                    return true;
                default:
                    boxTypeName = null;
                    return false;
            }
        }

        public static string GetJavaRefBoxType(string typeName)
        {
            string ret;
            if (GetJavaRefBoxType(typeName, out ret))
                return ret;
            else
                throw new Exception("Unsupported java ref box type for " + typeName);
        }

        public static bool GetJavaRefBoxType(string typeName, out string boxTypeName)
        {
            switch (typeName)
            {
                case "CodeBinder.PString":
                case "System.String":
                    boxTypeName = "StringBox";
                    return true;
                case "System.IntPtr":
                    boxTypeName = "LongBox";
                    return true;
                case "System.Boolean":
                    boxTypeName = "BooleanBox";
                    return true;
                case "System.Char":
                    boxTypeName = "CharacterBox";
                    return true;
                case "System.Byte":
                case "System.SByte":
                    boxTypeName = "ByteBox";
                    return true;
                case "System.Int16":
                case "System.UInt16":
                    boxTypeName = "ShortBox";
                    return true;
                case "System.Int32":
                case "System.UInt32":
                    boxTypeName = "IntegerBox";
                    return true;
                case "System.Int64":
                case "System.UInt64":
                    boxTypeName = "LongBox";
                    return true;
                case "System.Single":
                    boxTypeName = "FloatBox";
                    return true;
                case "System.Double":
                    boxTypeName = "DoubleBox";
                    return true;
                default:
                    boxTypeName = null;
                    return false;
            }
        }

        public static string ToJavaType(this JavaInteropType type)
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
                    return "String";
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
