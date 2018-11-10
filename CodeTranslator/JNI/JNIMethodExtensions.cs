// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;
using CodeTranslator.Shared.CSharp;
using CodeTranslator.Util;
using CodeTranslator.Shared;

namespace CodeTranslator.JNI
{
    static class JNIMethodExtensions
    {
        public static string GetJNITypeName(ref this MethodParameterInfo parameter)
        {
            ITypeSymbol typeSymbol;
            string typeName = parameter.GetTypeName(out typeSymbol);
            return getJNIType(typeName, typeSymbol);
        }

        public static string GetJNIType(this TypeSyntax type, bool isByRef, ICompilationContextProvider provider)
        {
            var symbol = type.GetTypeSymbol(provider);
            return getJNIType(type, symbol, isByRef);
        }

        private static string getJNIType(TypeSyntax syntax, ITypeSymbol symbol, bool isByRef)
        {
            if (symbol.TypeKind == TypeKind.Enum)
            {
                if (isByRef)
                    return "jIntegerBox";
                else
                    return "jint";
            }

            string netFullName;
            string javaArraySuffix;
            if (symbol.TypeKind == TypeKind.Array)
            {
                var arrayType = symbol as IArrayTypeSymbol;

                netFullName = arrayType.ElementType.GetFullName();
                javaArraySuffix = "Array";
            }
            else
            {
                netFullName = symbol.GetFullName();
                javaArraySuffix = string.Empty;
            }

            string jniTypeName;
            if (isByRef)
                jniTypeName = getJNIByRefType(netFullName, symbol);
            else
                jniTypeName = getJNIType(netFullName, symbol);

            return jniTypeName + javaArraySuffix;
        }


        static string getJNIType(string typeName, ITypeSymbol symbol)
        {
            switch (typeName)
            {
                case "System.Void":
                    return "void";
                case "System.Object":
                    return "jobject";
                case "System.String":
                    return "jstring";
                case "System.IntPtr":
                    return "jlong";
                case "System.Boolean":
                    return "jboolean";
                case "System.Char":
                    return "jchar";
                case "System.Byte":
                    return "jbyte";
                case "System.SByte":
                    return "jbyte";
                case "System.Int16":
                    return "jshort";
                case "System.UInt16":
                    return "jshort";
                case "System.Int32":
                    return "jint";
                case "System.UInt32":
                    return "jint";
                case "System.Int64":
                    return "jlong";
                case "System.UInt64":
                    return "jlong";
                case "System.Single":
                    return "jfloat";
                case "System.Double":
                    return "jdouble";
                default:
                {
                    if (symbol == null)
                    {
                        // This case happen with attribute types that are provided as strings
                        return typeName;
                    }

                    if (symbol.TypeKind == TypeKind.Class)
                        return "jobject";
                    else
                        throw new Exception("Unsupported by ref type " + typeName);
                }
            }
        }

        static string getJNIByRefType(string typeName, ITypeSymbol symbol)
        {
            switch (typeName)
            {
                case "System.IntPtr":
                    return "jLongBox";
                case "System.Boolean":
                    return "jBooleanBox";
                case "System.Char":
                    return "jCharacterBox";
                case "System.Byte":
                    return "jByteBox";
                case "System.SByte":
                    return "jByteBox";
                case "System.Int16":
                    return "jShortBox";
                case "System.UInt16":
                    return "jShortBox";
                case "System.Int32":
                    return "jIntegerBox";
                case "System.UInt32":
                    return "jIntegerBox";
                case "System.Int64":
                    return "jLongBox";
                case "System.UInt64":
                    return "jLongBox";
                case "System.Single":
                    return "jFloatBox";
                case "System.Double":
                    return "jDoubleBox";
                case "System.String":
                    return "jStringBox";
                default:
                {
                    if (symbol.TypeKind == TypeKind.Struct)
                        return "jlong";
                    else
                        throw new Exception("Unsupported by ref type " + typeName); 
                }
            }
        }

        public static CodeBuilder EndOfLine(this CodeBuilder builder)
        {
            return builder.AppendLine(";");
        }

        public static CodeBuilder Space(this CodeBuilder builder)
        {
            return builder.Append(" ");
        }
    }
}
