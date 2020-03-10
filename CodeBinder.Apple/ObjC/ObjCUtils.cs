using CodeBinder.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace CodeBinder.Apple
{
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

        /// <summary>
        /// Simple type is primitive value types plus void
        /// </summary>
        public static string GetSimpleType(string typeName, ObjCTypeUsageKind usage = ObjCTypeUsageKind.Normal)
        {
            string? ret;
            if (TryGetSimpleType(typeName, usage, out ret))
                return ret;
            else
                throw new Exception($"Unsupported ObjectiveC type for {typeName}");
        }

        /// <summary>Primitive types as defined by https://docs.microsoft.com/en-us/dotnet/api/system.type.isprimitive
        /// </summary>
        /// <returns>Return true if the given symbol is a blittable non-structured system type</returns>
        public static bool IsCLRPrimitiveType(this ITypeSymbol symbol, [NotNullWhen(true)]out string? objcname)
        {
            switch (symbol.SpecialType)
            {
                case SpecialType.System_IntPtr:
                    objcname = "void *";
                    return true;
                case SpecialType.System_UIntPtr:
                    objcname = "void *";
                    return true;
                case SpecialType.System_Boolean:
                    objcname = "BOOL";
                    return true;
                case SpecialType.System_Char:
                    objcname = "char";
                    return true;
                case SpecialType.System_Byte:
                    objcname = "uint8_t";
                    return true;
                case SpecialType.System_SByte:
                    objcname = "int8_t";
                    return true;
                case SpecialType.System_UInt16:
                    objcname = "uint16_t";
                    return true;
                case SpecialType.System_Int16:
                    objcname = "int16_t";
                    return true;
                case SpecialType.System_UInt32:
                    objcname = "uint32_t";
                    return true;
                case SpecialType.System_Int32:
                    objcname = "int32_t";
                    return true;
                case SpecialType.System_UInt64:
                    objcname = "uint64_t";
                    return true;
                case SpecialType.System_Int64:
                    objcname = "int64_t";
                    return true;
                case SpecialType.System_Single:
                    objcname = "float";
                    return true;
                case SpecialType.System_Double:
                    objcname = "double";
                    return true;
                default:
                    objcname = null;
                    return false;
            }
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
            switch (symbol.SpecialType)
            {
                case SpecialType.System_IntPtr:
                    accessProperty = "integerValue";
                    return true;
                case SpecialType.System_UIntPtr:
                    accessProperty = "unsignedIntegerValue";
                    return true;
                case SpecialType.System_Boolean:
                    accessProperty = "boolValue";
                    return true;
                case SpecialType.System_Char:
                    accessProperty = "charValue";
                    return true;
                case SpecialType.System_Byte:
                    accessProperty = "unsignedCharValue";
                    return true;
                case SpecialType.System_SByte:
                    accessProperty = "charValue";
                    return true;
                case SpecialType.System_UInt16:
                    accessProperty = "unsignedShortValue";
                    return true;
                case SpecialType.System_Int16:
                    accessProperty = "shortValue";
                    return true;
                case SpecialType.System_UInt32:
                    accessProperty = "unsignedIntValue";
                    return true;
                case SpecialType.System_Int32:
                    accessProperty = "intValue";
                    return true;
                case SpecialType.System_UInt64:
                    accessProperty = "unsignedLongLongValue";
                    return true;
                case SpecialType.System_Int64:
                    accessProperty = "longLongValue";
                    return true;
                case SpecialType.System_Single:
                    accessProperty = "floatValue";
                    return true;
                case SpecialType.System_Double:
                    accessProperty = "doubleValue";
                    return true;
                default:
                    accessProperty = null;
                    return false;
            }
        }

        /// <summary>
        /// Simple type is primitive value types plus void
        /// </summary>
        public static bool TryGetSimpleType(string fullTypeName, [NotNullWhen(true)]out string? typeName)
        {
            return TryGetSimpleType(fullTypeName, ObjCTypeUsageKind.Normal, out typeName);
        }

        /// <summary>
        /// Simple type is primitive value types plus void
        /// </summary>
        public static bool TryGetSimpleType(string fullTypeName, ObjCTypeUsageKind usage, [NotNullWhen(true)]out string? typeName)
        {
            switch (fullTypeName)
            {
                case "System.Void":
                    typeName = "void";
                    return true;
                case "CodeBinder.Apple.NSUInteger":
                    typeName = "NSUInteger";
                    AdaptValueType(ref typeName, usage);
                    return true;
                case "CodeBinder.Apple.NSInteger":
                    typeName = "NSInteger";
                    AdaptValueType(ref typeName, usage);
                    return true;
                case "System.UIntPtr":
                    typeName = "void *";
                    AdaptValueType(ref typeName, usage);
                    return true;
                case "System.IntPtr":
                    typeName = "void *";
                    AdaptValueType(ref typeName, usage);
                    return true;
                case "System.Boolean":
                    typeName = "BOOL";
                    AdaptValueType(ref typeName, usage);
                    return true;
                case "System.Char":
                    typeName = "char";
                    AdaptValueType(ref typeName, usage);
                    return true;
                case "System.Byte":
                    typeName = "uint8_t";
                    AdaptValueType(ref typeName, usage);
                    return true;
                case "System.SByte":
                    typeName = "int8_t";
                    AdaptValueType(ref typeName, usage);
                    return true;
                case "System.UInt16":
                    typeName = "uint16_t";
                    AdaptValueType(ref typeName, usage);
                    return true;
                case "System.Int16":
                    typeName = "int16_t";
                    AdaptValueType(ref typeName, usage);
                    return true;
                case "System.UInt32":
                    typeName = "uint32_t";
                    AdaptValueType(ref typeName, usage);
                    return true;
                case "System.Int32":
                    typeName = "int32_t";
                    AdaptValueType(ref typeName, usage);
                    return true;
                case "System.UInt64":
                    typeName = "uint64_t";
                    AdaptValueType(ref typeName, usage);
                    return true;
                case "System.Int64":
                    typeName = "int64_t";
                    AdaptValueType(ref typeName, usage);
                    return true;
                case "System.Single":
                    typeName = "float";
                    AdaptValueType(ref typeName, usage);
                    return true;
                case "System.Double":
                    typeName = "double";
                    AdaptValueType(ref typeName, usage);
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
                case "System.Char":
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

        public static bool TryGeArrayBoxType(string typeName, [NotNullWhen(true)]out string? boxTypeName)
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
                case "System.Char":
                    boxTypeName = "CBCharArray";
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
                case "System.String":
                    boxTypeName = "CBStringArray";
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
                ObjCInteropType.String => "CBStringArray",
                _ => throw new NotSupportedException(),
            };
        }

        public static string ToArrayTypeName(this ObjCInteropType type)
        {
            switch (type)
            {
                case ObjCInteropType.NSUInteger:
                    return "NSUInteger *";
                case ObjCInteropType.NSInteger:
                    return "NSInteger *";
                case ObjCInteropType.UIntPtr:
                    return "void **";
                case ObjCInteropType.IntPtr:
                    return "void **";
                case ObjCInteropType.Boolean:
                    return "BOOL *";
                case ObjCInteropType.Char:
                    return "char *";
                case ObjCInteropType.Byte:
                    return "uint8_t *";
                case ObjCInteropType.SByte:
                    return "int8_t *";
                case ObjCInteropType.UInt16:
                    return "uint16_t *";
                case ObjCInteropType.Int16:
                    return "int16_t *";
                case ObjCInteropType.UInt32:
                    return "uint32_t *";
                case ObjCInteropType.Int32:
                    return "int32_t *";
                case ObjCInteropType.UInt64:
                    return "uint64_t *";
                case ObjCInteropType.Int64:
                    return "int64_t *";
                case ObjCInteropType.Single:
                    return "float *";
                case ObjCInteropType.Double:
                    return "double *";
                case ObjCInteropType.String:
                    return "NSString * __strong *";
                default:
                    throw new Exception();
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
                case ObjCInteropType.String:
                    return "NSString * __strong";
                default:
                    throw new Exception();
            }
        }

        /// <summary>
        /// Try fix the type based on the usage
        /// </summary>
        static void AdaptValueType(ref string type, ObjCTypeUsageKind usage)
        {
            switch (usage)
            {
                case ObjCTypeUsageKind.DeclarationByRef:
                {
                    type = $"{type} *";
                    return;
                }
                case ObjCTypeUsageKind.Declaration:
                case ObjCTypeUsageKind.Normal:
                    return;
                default:
                    throw new NotSupportedException();
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
        String,
    }

    enum ObjCTypeUsageKind
    {
        /// <summary>
        /// All other uses
        /// </summary>
        Normal,
        /// <summary>
        /// Local, member, parameter declarations
        /// </summary>
        Declaration,
        /// <summary>
        /// Parameter declarations by ref
        /// </summary>
        DeclarationByRef,
    }
}
