// SPDX-FileCopyrightText: (C) 2023 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.JavaScript.TypeScript;

public static class TypeScriptUtils
{
    public static string GetBoxType(string typeName)
    {
        string? ret;
        if (TryGetBoxType(typeName, out ret))
            return ret;
        else
            throw new Exception("Unsupported java box type for " + typeName);
    }

    public static bool TryGetBoxType(string typeName, [NotNullWhen(true)]out string? boxTypeName)
    {
        switch (typeName)
        {
            case "System.Boolean":
                boxTypeName = "boolean | null";
                return true;
            case "System.IntPtr":
            case "System.Byte":
            case "System.SByte":
            case "System.Int16":
            case "System.UInt16":
            case "System.Int32":
            case "System.UInt32":
            case "System.Single":
            case "System.Double":
                boxTypeName = "number | null";
                return true;
            case "System.Int64":
            case "System.UInt64":
                boxTypeName = "bigint | null";
                return true;
            default:
                boxTypeName = null;
                return false;
        }
    }

    public static string GetRefBoxType(string typeName)
    {
        string? ret;
        if (TryGetRefBoxType(typeName, out ret))
            return ret;
        else
            throw new Exception("Unsupported java ref box type for " + typeName);
    }

    public static bool TryGetRefBoxType(string typeName, [NotNullWhen(true)]out string? boxTypeName)
    {
        switch (typeName)
        {
            case "CodeBinder.cbstring":
            case "System.String":
                boxTypeName = "StringRefBox";
                return true;
            case "CodeBinder.cbbool":
            case "System.Boolean":
                boxTypeName = "BooleanRefBox";
                return true;
            case "System.IntPtr":
            case "System.Byte":
            case "System.SByte":
            case "System.Int16":
            case "System.UInt16":
            case "System.Int32":
            case "System.UInt32":
            case "System.Single":
            case "System.Double":
                boxTypeName = "NumberRefBox";
                return true;
            case "System.Int64":
            case "System.UInt64":
                boxTypeName = "BigIntRefBox";
                return true;
            default:
                boxTypeName = null;
                return false;
        }
    }

    public static string ToJavaType(this JavaScriptInteropType type)
    {
        switch (type)
        {
            case JavaScriptInteropType.Boolean:
                return "boolean";
            case JavaScriptInteropType.Number:
                return "number";
            case JavaScriptInteropType.BigInt:
                return "bigint";
            case JavaScriptInteropType.String:
                return "string";
            default:
                throw new NotSupportedException();
        }
    }

    public static JavaScriptInteropType ToJavaInteropType(string javaKeyword)
    {
        switch (javaKeyword)
        {
            case "boolean":
                return JavaScriptInteropType.Boolean;
            case "number":
                return JavaScriptInteropType.Number;
            case "bigint":
                return JavaScriptInteropType.BigInt;
            case "string":
                return JavaScriptInteropType.String;
            default:
                throw new NotSupportedException();
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
