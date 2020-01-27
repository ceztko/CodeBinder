using CodeBinder.Shared.Java;
using CodeBinder.Util;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace CodeBinder.Java
{
    public static class JavaUtils
    {
        delegate bool ModifierGetter(SyntaxKind modifier, [NotNullWhen(true)]out string? javaModifier);

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

        public static string GetFieldModifiersString(IEnumerable<SyntaxKind> modifiers)
        {
            return getJavaModifiersString(modifiers, tryGetJavaFieldModifier);
        }

        public static string GetTypeModifiersString(IEnumerable<SyntaxKind> modifiers)
        {
            return getJavaModifiersString(modifiers, tryGetJavaTypeModifier);
        }

        public static string GetMethodModifiersString(IEnumerable<SyntaxKind> modifiers)
        {
            return getJavaModifiersString(modifiers, tryGetMethodModifier);
        }

        public static string GetPropertyModifiersString(IEnumerable<SyntaxKind> modifiers)
        {
            return getJavaModifiersString(modifiers, tryGetMethodModifier);
        }

        private static string getJavaModifiersString(IEnumerable<SyntaxKind> modifiers, ModifierGetter getJavaModifier)
        {
            var builder = new CodeBuilder();
            bool first = true;
            foreach (var modifier in modifiers)
            {
                string? javaModifier;
                if (!getJavaModifier(modifier, out javaModifier))
                    continue;

                builder.Space(ref first).Append(javaModifier);
            }

            return builder.ToString();
        }

        private static bool tryGetJavaFieldModifier(SyntaxKind modifier, [NotNullWhen(true)]out string? javaModifier)
        {
            switch (modifier)
            {
                case SyntaxKind.PublicKeyword:
                    javaModifier = "public";
                    return true;
                case SyntaxKind.ProtectedKeyword:
                    javaModifier = "protected";
                    return true;
                case SyntaxKind.PrivateKeyword:
                    javaModifier = "private";
                    return true;
                case SyntaxKind.StaticKeyword:
                    javaModifier = "static";
                    return true;
                case SyntaxKind.ReadOnlyKeyword:
                    javaModifier = "final";
                    return true;
                case SyntaxKind.ConstKeyword:
                    javaModifier = "final";
                    return true;
                case SyntaxKind.NewKeyword:
                    javaModifier = null;
                    return false;
                case SyntaxKind.InternalKeyword:
                    javaModifier = null;
                    return false;
                default:
                    throw new Exception();
            }
        }

        private static bool tryGetJavaTypeModifier(SyntaxKind modifier, [NotNullWhen(true)]out string? javaModifier)
        {
            switch (modifier)
            {
                case SyntaxKind.PublicKeyword:
                    javaModifier = "public";
                    return true;
                case SyntaxKind.ProtectedKeyword:
                    javaModifier = "protected";
                    return true;
                case SyntaxKind.PrivateKeyword:
                    javaModifier = "private";
                    return true;
                case SyntaxKind.SealedKeyword:
                    javaModifier = "final";
                    return true;
                case SyntaxKind.AbstractKeyword:
                    javaModifier = "abstract";
                    return true;
                case SyntaxKind.InternalKeyword:
                    javaModifier = null;
                    return false;
                case SyntaxKind.StaticKeyword:
                    javaModifier = null;
                    return false;
                default:
                    throw new Exception();
            }
        }

        private static bool tryGetMethodModifier(SyntaxKind modifier, [NotNullWhen(true)]out string? javaModifier)
        {
            switch (modifier)
            {
                case SyntaxKind.PublicKeyword:
                    javaModifier = "public";
                    return true;
                case SyntaxKind.ProtectedKeyword:
                    javaModifier = "protected";
                    return true;
                case SyntaxKind.PrivateKeyword:
                    javaModifier = "private";
                    return true;
                case SyntaxKind.StaticKeyword:
                    javaModifier = "static";
                    return true;
                case SyntaxKind.SealedKeyword:
                    javaModifier = "final";
                    return true;
                case SyntaxKind.ExternKeyword:
                    javaModifier = "native";
                    return true;
                case SyntaxKind.AbstractKeyword:
                    javaModifier = "abstract";
                    return true;
                case SyntaxKind.PartialKeyword:
                    javaModifier = null;
                    return false;
                case SyntaxKind.NewKeyword:
                    javaModifier = null;
                    return false;
                case SyntaxKind.InternalKeyword:
                    javaModifier = null;
                    return false;
                case SyntaxKind.VirtualKeyword:
                    javaModifier = null;
                    return false;
                case SyntaxKind.OverrideKeyword:
                    javaModifier = null;
                    return false;
                default:
                    throw new Exception();
            }
        }

        private static bool tryGetPropertyModifier(SyntaxKind modifier, [NotNullWhen(true)]out string? javaModifier)
        {
            switch (modifier)
            {
                case SyntaxKind.PublicKeyword:
                    javaModifier = "public";
                    return true;
                case SyntaxKind.ProtectedKeyword:
                    javaModifier = "protected";
                    return true;
                case SyntaxKind.PrivateKeyword:
                    javaModifier = "private";
                    return true;
                case SyntaxKind.StaticKeyword:
                    javaModifier = "static";
                    return true;
                case SyntaxKind.SealedKeyword:
                    javaModifier = "final";
                    return true;
                case SyntaxKind.AbstractKeyword:
                    javaModifier = "abstract";
                    return true;
                case SyntaxKind.NewKeyword:
                    javaModifier = null;
                    return false;
                case SyntaxKind.InternalKeyword:
                    javaModifier = null;
                    return false;
                case SyntaxKind.VirtualKeyword:
                    javaModifier = null;
                    return false;
                case SyntaxKind.OverrideKeyword:
                    javaModifier = null;
                    return false;
                default:
                    throw new Exception();
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

        public static JavaInteropType ToJavaInteropType(string javaKeyword)
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
