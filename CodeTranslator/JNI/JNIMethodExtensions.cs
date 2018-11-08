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
        public static string GetJNIType(this TypeSyntax type, bool isRef, ICompilationContextProvider provider)
        {
            var symbol = type.GetTypeSymbol(provider);
            return getJNIType(type, symbol, isRef);
        }

        private static string getJNIType(TypeSyntax syntax, ITypeSymbol symbol, bool isRef)
        {
            if (symbol.TypeKind == TypeKind.Enum)
            {
                if (isRef)
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
            if (isRef)
                jniTypeName = getJNIByRefType(netFullName);
            else
                jniTypeName = getJNIType(syntax, netFullName);

            return jniTypeName + javaArraySuffix;
        }


        static string getJNIType(TypeSyntax syntax, string netFullName)
        {
            switch (netFullName)
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
                    return syntax.GetTypeIdentifier();
            }
        }

        static string getJNIByRefType(string netFullName)
        {
            switch (netFullName)
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
                    return "jLongBox"; // CHECK-ME
            }
        }
    }
}
