﻿// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT
using System.Linq;

namespace CodeBinder.Apple;

static class ObjCUtils
{
    delegate bool ModifierGetter(SyntaxKind modifier, [NotNullWhen(true)]out string? modifierStr);

    public static string GetBoxType(string typeName)
    {
        string? ret;
        if (TryGetBoxType(typeName, out ret))
            return ret;
        else
            throw new Exception("Unsupported ObjectiveC box type for " + typeName);
    }

    public static IEnumerable<ObjCInteropType> GetInteropTypes()
    {
        return Enum.GetValues(typeof(ObjCInteropType)).Cast<ObjCInteropType>();
    }

    public static ObjCFileType ToFileType(this ObjCHeaderType type)
    {
        return type switch
        {
            ObjCHeaderType.Public => ObjCFileType.PublicHeader,
            ObjCHeaderType.Internal => ObjCFileType.InternalHeader,
            ObjCHeaderType.InternalOnly => ObjCFileType.InternalOnlyHeader,
            _ => throw new NotSupportedException(),
        };
    }

    public static string GetNSNumberAccessProperty(this ITypeSymbol symbol)
    {
        if (TryGetNSNumberAccessProperty(symbol, out var accessProperty))
            return accessProperty;
        else
            throw new Exception($"No available NSNumber property access for {symbol}");
    }

    /// <summary>
    /// Get numeric access method for NSNumber
    /// https://developer.apple.com/documentation/foundation/nsnumber?language=objc
    /// </summary>
    public static bool TryGetNSNumberAccessProperty(this ITypeSymbol symbol, [NotNullWhen(true)]out string? accessProperty)
    {
        string fullname = symbol.GetFullName();
        switch (fullname)
        {
            case "CodeBinder.Apple.NSUInteger":
                accessProperty = "unsignedIntegerValue";
                return true;
            case "CodeBinder.Apple.NSInteger":
                accessProperty = "integerValue";
                return true;
            case "System.IntPtr":
                accessProperty = "integerValue";
                return true;
            case "System.UIntPtr":
                accessProperty = "unsignedIntegerValue";
                return true;
            case "System.Boolean":
                accessProperty = "boolValue";
                return true;
            case "System.Byte":
                accessProperty = "unsignedCharValue";
                return true;
            case "System.SByte":
                accessProperty = "charValue";
                return true;
            case "System.UInt16":
                accessProperty = "unsignedShortValue";
                return true;
            case "System.Int16":
                accessProperty = "shortValue";
                return true;
            case "System.UInt32":
                accessProperty = "unsignedIntValue";
                return true;
            case "System.Int32":
                accessProperty = "intValue";
                return true;
            case "System.UInt64":
                accessProperty = "unsignedLongLongValue";
                return true;
            case "System.Int64":
                accessProperty = "longLongValue";
                return true;
            case "System.Single":
                accessProperty = "floatValue";
                return true;
            case "System.Double":
                accessProperty = "doubleValue";
                return true;
            default:
                accessProperty = null;
                return false;
        }
    }

    public static string GetNSNumberInitMethod(this ITypeSymbol symbol)
    {
        if (TryGetNSNumberInitMethod(symbol, out var accessProperty))
            return accessProperty;
        else
            throw new Exception($"No available NSNumber property access for {symbol}");
    }

    /// <summary>
    /// Get numeric access method for NSNumber
    /// https://developer.apple.com/documentation/foundation/nsnumber?language=objc
    /// </summary>
    public static bool TryGetNSNumberInitMethod(this ITypeSymbol symbol, [NotNullWhen(true)]out string? accessProperty)
    {
        string fullname = symbol.GetFullName();
        switch (fullname)
        {
            case "CodeBinder.Apple.NSUInteger":
                accessProperty = "numberWithUnsignedInteger";
                return true;
            case "CodeBinder.Apple.NSInteger":
                accessProperty = "numberWithInteger";
                return true;
            case "System.IntPtr":
                accessProperty = "numberWithInteger";
                return true;
            case "System.UIntPtr":
                accessProperty = "numberWithUnsignedInteger";
                return true;
            case "System.Boolean":
                accessProperty = "numberWithBool";
                return true;
            case "System.Byte":
                accessProperty = "numberWithUnsignedChar";
                return true;
            case "System.SByte":
                accessProperty = "numberWithChar";
                return true;
            case "System.UInt16":
                accessProperty = "numberWithUnsignedShort";
                return true;
            case "System.Int16":
                accessProperty = "numberWithShort";
                return true;
            case "System.UInt32":
                accessProperty = "numberWithUnsignedInt";
                return true;
            case "System.Int32":
                accessProperty = "numberWithInt";
                return true;
            case "System.UInt64":
                accessProperty = "numberWithUnsignedLongLong";
                return true;
            case "System.Int64":
                accessProperty = "numberWithLongLong";
                return true;
            case "System.Single":
                accessProperty = "numberWithFloat";
                return true;
            case "System.Double":
                accessProperty = "numberWithDouble";
                return true;
            default:
                accessProperty = null;
                return false;
        }
    }

    /// <summary>
    /// Get ObjC primitive value types
    /// </summary>
    public static bool IsObjCPrimitiveType(this ITypeSymbol typeSymbol)
    {
        if (TryGetObjCPrimitiveType(typeSymbol, out var typeName))
            return true;
        else
            return false;
    }

    /// <summary>
    /// Get ObjC primitive value types
    /// </summary>
    public static string GetObjCPrimitiveType(this ITypeSymbol typeSymbol)
    {
        if (TryGetObjCPrimitiveType(typeSymbol, out var typeName))
            return typeName;
        else
            throw new Exception("Not a primitive type");
    }

    /// <summary>
    /// Get ObjC primitive value types
    /// </summary>
    public static bool TryGetObjCPrimitiveType(this ITypeSymbol typeSymbol, [NotNullWhen(true)]out string? typeName)
    {
        return TryGetObjCPrimitiveType(typeSymbol.GetFullName(), out typeName);
    }

    /// <summary>
    /// Get ObjC primitive value types
    /// </summary>
    public static bool TryGetObjCPrimitiveType(string fullTypeName, [NotNullWhen(true)]out string? typeName)
    {
        switch (fullTypeName)
        {
            case "CodeBinder.Apple.NSUInteger":
                typeName = "NSUInteger";
                return true;
            case "CodeBinder.Apple.NSInteger":
                typeName = "NSInteger";
                return true;
            case "CodeBinder.cbbool":
                typeName = "cbbool";
                return true;
            case "CodeBinder.cbstring": // NOTE: We assume it's always eagerly casted to NSString
                typeName = "NSString*";
                return true;
            case "System.UIntPtr":
                typeName = "void *";
                return true;
            case "System.IntPtr":
                typeName = "void *";
                return true;
            case "System.Boolean":
                typeName = "BOOL";
                return true;
            case "System.Byte":
                typeName = "uint8_t";
                return true;
            case "System.SByte":
                typeName = "int8_t";
                return true;
            case "System.UInt16":
                typeName = "uint16_t";
                return true;
            case "System.Int16":
                typeName = "int16_t";
                return true;
            case "System.UInt32":
                typeName = "uint32_t";
                return true;
            case "System.Int32":
                typeName = "int32_t";
                return true;
            case "System.UInt64":
                typeName = "uint64_t";
                return true;
            case "System.Int64":
                typeName = "int64_t";
                return true;
            case "System.Single":
                typeName = "float";
                return true;
            case "System.Double":
                typeName = "double";
                return true;
            default:
                typeName = null;
                return false;
        }
    }

    public static bool TryGetBoxType(string typeName, [NotNullWhen(true)]out string? boxTypeName)
    {
        switch (typeName)
        {
            case "CodeBinder.Apple.NSUInteger":
            case "CodeBinder.Apple.NSInteger":
            case "System.UIntPtr":
            case "System.IntPtr":
            case "System.Boolean":
            case "System.Byte":
            case "System.SByte":
            case "System.Int16":
            case "System.UInt16":
            case "System.Int32":
            case "System.UInt32":
            case "System.Int64":
            case "System.UInt64":
            case "System.Single":
            case "System.Double":
                boxTypeName = "NSNumber";
                return true;
            default:
                boxTypeName = null;
                return false;
        }
    }

    public static bool TryGetArrayBoxType(string typeName, [NotNullWhen(true)]out string? boxTypeName)
    {
        switch (typeName)
        {
            case "CodeBinder.Apple.NSUInteger":
                boxTypeName = "CBNSUIntegerArray";
                return true;
            case "CodeBinder.Apple.NSInteger":
                boxTypeName = "CBNSIntegerArray";
                return true;
            case "System.UIntPtr":
                boxTypeName = "CBPtrArray";
                return true;
            case "System.IntPtr":
                boxTypeName = "CBPtrArray";
                return true;
            case "System.Boolean":
                boxTypeName = "CBBoolArray";
                return true;
            case "System.Byte":
                boxTypeName = "CBUInt8Array";
                return true;
            case "System.SByte":
                boxTypeName = "CBInt8Array";
                return true;
            case "System.UInt16":
                boxTypeName = "CBUInt16Array";
                return true;
            case "System.Int16":
                boxTypeName = "CBInt16Array";
                return true;
            case "System.UInt32":
                boxTypeName = "CBUInt32Array";
                return true;
            case "System.Int32":
                boxTypeName = "CBInt32Array";
                return true;
            case "System.UInt64":
                boxTypeName = "CBUInt64Array";
                return true;
            case "System.Int64":
                boxTypeName = "CBInt64Array";
                return true;
            case "System.Single":
                boxTypeName = "CBFloatArray";
                return true;
            case "System.Double":
                boxTypeName = "CBDoubleArray";
                return true;
            default:
                boxTypeName = null;
                return false;
        }
    }

    public static string ToArrayBoxTypeName(this ObjCInteropType type)
    {
        return type switch
        {
            ObjCInteropType.NSUInteger => "CBNSUIntegerArray",
            ObjCInteropType.NSInteger => "CBNSIntegerArray",
            ObjCInteropType.UIntPtr => "CBPtrArray",
            ObjCInteropType.IntPtr => "CBPtrArray",
            ObjCInteropType.Boolean => "CBBoolArray",
            ObjCInteropType.Char => "CBCharArray",
            ObjCInteropType.Byte => "CBUInt8Array",
            ObjCInteropType.SByte => "CBInt8Array",
            ObjCInteropType.UInt16 => "CBUInt16Array",
            ObjCInteropType.Int16 => "CBInt16Array",
            ObjCInteropType.UInt32 => "CBUInt32Array",
            ObjCInteropType.Int32 => "CBInt32Array",
            ObjCInteropType.UInt64 => "CBUInt64Array",
            ObjCInteropType.Int64 => "CBInt64Array",
            ObjCInteropType.Single => "CBFloatArray",
            ObjCInteropType.Double => "CBDoubleArray",
            _ => throw new NotSupportedException(),
        };
    }

    public static string ToArrayTypeName(this ObjCInteropType type, bool isConst)
    {
        if (isConst)
        {
            return type switch
            {
                ObjCInteropType.NSUInteger => "const NSUInteger *",
                ObjCInteropType.NSInteger => "const NSInteger *",
                ObjCInteropType.UIntPtr => "void * const *",
                ObjCInteropType.IntPtr => "void * const *",
                ObjCInteropType.Boolean => "const BOOL *",
                ObjCInteropType.Char => "const char *",
                ObjCInteropType.Byte => "const uint8_t *",
                ObjCInteropType.SByte => "const int8_t *",
                ObjCInteropType.UInt16 => "const uint16_t *",
                ObjCInteropType.Int16 => "const int16_t *",
                ObjCInteropType.UInt32 => "const uint32_t *",
                ObjCInteropType.Int32 => "const int32_t *",
                ObjCInteropType.UInt64 => "const uint64_t *",
                ObjCInteropType.Int64 => "const int64_t *",
                ObjCInteropType.Single => "const float *",
                ObjCInteropType.Double => "const double *",
                _ => throw new NotSupportedException(),
            };
        }
        else
        {
            return type switch
            {
                ObjCInteropType.NSUInteger => "NSUInteger *",
                ObjCInteropType.NSInteger => "NSInteger *",
                ObjCInteropType.UIntPtr => "void **",
                ObjCInteropType.IntPtr => "void **",
                ObjCInteropType.Boolean => "BOOL *",
                ObjCInteropType.Char => "char *",
                ObjCInteropType.Byte => "uint8_t *",
                ObjCInteropType.SByte => "int8_t *",
                ObjCInteropType.UInt16 => "uint16_t *",
                ObjCInteropType.Int16 => "int16_t *",
                ObjCInteropType.UInt32 => "uint32_t *",
                ObjCInteropType.Int32 => "int32_t *",
                ObjCInteropType.UInt64 => "uint64_t *",
                ObjCInteropType.Int64 => "int64_t *",
                ObjCInteropType.Single => "float *",
                ObjCInteropType.Double => "double *",
                _ => throw new NotSupportedException(),
            };
        }
    }

    public static string ToTypeName(this ObjCInteropType type)
    {
        switch (type)
        {
            case ObjCInteropType.NSUInteger:
                return "NSUInteger";
            case ObjCInteropType.NSInteger:
                return "NSInteger";
            case ObjCInteropType.UIntPtr:
                return "void *";
            case ObjCInteropType.IntPtr:
                return "void *";
            case ObjCInteropType.Boolean:
                return "BOOL";
            case ObjCInteropType.Char:
                return "char";
            case ObjCInteropType.Byte:
                return "uint8_t";
            case ObjCInteropType.SByte:
                return "int8_t";
            case ObjCInteropType.UInt16:
                return "uint16_t";
            case ObjCInteropType.Int16:
                return "int16_t";
            case ObjCInteropType.UInt32:
                return "uint32_t";
            case ObjCInteropType.Int32:
                return "int32_t";
            case ObjCInteropType.UInt64:
                return "uint64_t";
            case ObjCInteropType.Int64:
                return "int64_t";
            case ObjCInteropType.Single:
                return "float";
            case ObjCInteropType.Double:
                return "double";
            default:
                throw new Exception();
        }
    }

    public static string GetFieldModifiersString(Accessibility accessibility)
    {
        switch (accessibility)
        {
            case Accessibility.Public:
                return "@public";
            case Accessibility.Protected:
                return "@protected";
            case Accessibility.Private:
                return "@private";
            case Accessibility.Internal:
                return "@package";
            case Accessibility.ProtectedAndInternal:
                return "@public";
            default:
                throw new Exception("Unsupported accessibility " + accessibility);
        }
    }

    public static string GetMethodModifiersString(IEnumerable<SyntaxKind> modifiers)
    {
        return getObjcModifiersString(modifiers, tryGetObjCMethodModifier);
    }

    public static string GetPropertyModifiersString(IEnumerable<SyntaxKind> modifiers)
    {
        return getObjcModifiersString(modifiers, tryGetObjCMethodModifier);
    }

    private static string getObjcModifiersString(IEnumerable<SyntaxKind> modifiers, ModifierGetter getModifierString)
    {
        var builder = new CodeBuilder();
        bool first = true;
        foreach (var modifier in modifiers)
        {
            string? modifierStr;
            if (!getModifierString(modifier, out modifierStr))
                continue;

            builder.Space(ref first).Append(modifierStr!);
        }

        return builder.ToString();
    }

    private static bool tryGetObjCTypeModifier(SyntaxKind modifier, [NotNullWhen(true)]out string? modifierSTr)
    {
        switch (modifier)
        {
            case SyntaxKind.PublicKeyword:
                modifierSTr = "public";
                return true;
            case SyntaxKind.ProtectedKeyword:
                modifierSTr = "protected";
                return true;
            case SyntaxKind.PrivateKeyword:
                modifierSTr = "private";
                return true;
            case SyntaxKind.SealedKeyword:
                modifierSTr = "final";
                return true;
            case SyntaxKind.AbstractKeyword:
                modifierSTr = "abstract";
                return true;
            case SyntaxKind.InternalKeyword:
                modifierSTr = null;
                return false;
            case SyntaxKind.StaticKeyword:
                modifierSTr = null;
                return false;
            default:
                throw new Exception();
        }
    }

    private static bool tryGetObjCMethodModifier(SyntaxKind modifier, [NotNullWhen(true)]out string? modifierStr)
    {
        switch (modifier)
        {
            case SyntaxKind.PublicKeyword:
                modifierStr = "public";
                return true;
            case SyntaxKind.ProtectedKeyword:
                modifierStr = "protected";
                return true;
            case SyntaxKind.PrivateKeyword:
                modifierStr = "private";
                return true;
            case SyntaxKind.StaticKeyword:
                modifierStr = "static";
                return true;
            case SyntaxKind.SealedKeyword:
                modifierStr = "final";
                return true;
            case SyntaxKind.ExternKeyword:
                modifierStr = "native";
                return true;
            case SyntaxKind.AbstractKeyword:
                modifierStr = "abstract";
                return true;
            case SyntaxKind.PartialKeyword:
                modifierStr = null;
                return false;
            case SyntaxKind.NewKeyword:
                modifierStr = null;
                return false;
            case SyntaxKind.InternalKeyword:
                modifierStr = null;
                return false;
            case SyntaxKind.VirtualKeyword:
                modifierStr = null;
                return false;
            case SyntaxKind.OverrideKeyword:
                modifierStr = null;
                return false;
            default:
                throw new Exception();
        }
    }

    private static bool tryGetObjCPropertyModifier(SyntaxKind modifier, [NotNullWhen(true)]out string? modifierStr)
    {
        switch (modifier)
        {
            case SyntaxKind.PublicKeyword:
                modifierStr = "public";
                return true;
            case SyntaxKind.ProtectedKeyword:
                modifierStr = "protected";
                return true;
            case SyntaxKind.PrivateKeyword:
                modifierStr = "private";
                return true;
            case SyntaxKind.StaticKeyword:
                modifierStr = "static";
                return true;
            case SyntaxKind.SealedKeyword:
                modifierStr = "final";
                return true;
            case SyntaxKind.AbstractKeyword:
                modifierStr = "abstract";
                return true;
            case SyntaxKind.NewKeyword:
                modifierStr = null;
                return false;
            case SyntaxKind.InternalKeyword:
                modifierStr = null;
                return false;
            case SyntaxKind.VirtualKeyword:
                modifierStr = null;
                return false;
            case SyntaxKind.OverrideKeyword:
                modifierStr = null;
                return false;
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

enum ParenthesisDirection
{
    Left,
    Right
}

enum ParenthesisType
{
    Round,
    Square,
    Angle,
    Brace,
}

enum ObjCInteropType
{
    NSUInteger, // This is an arithmetic type, differently than UIntPtr
    NSInteger,  // This is an arithmetic type, differently than IntPtr
    UIntPtr,
    IntPtr,
    Boolean,
    Char,
    Byte,
    SByte,
    UInt16,
    Int16,
    UInt32,
    Int32,
    UInt64,
    Int64,
    Single,
    Double,
}

enum ObjCTypeUsageKind
{
    /// <summary>
    /// Type declaration, generic parameters
    /// </summary>
    Normal,
    /// <summary>
    /// Local, member, method parameter declarations
    /// </summary>
    Declaration,
    /// <summary>
    /// Parameter declarations by ref
    /// </summary>
    DeclarationByRef,
    /// <summary>
    /// Pointer type only, if supported
    /// </summary>
    Pointer
}
