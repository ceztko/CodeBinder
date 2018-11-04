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
                return "jint";

            string fullTypeName;
            string javaArraySuffix;
            if (symbol.TypeKind == TypeKind.Array)
            {
                var arrayType = symbol as IArrayTypeSymbol;

                fullTypeName = arrayType.ElementType.GetFullName();
                javaArraySuffix = "Array";
            }
            else
            {
                fullTypeName = symbol.GetFullName();
                javaArraySuffix = string.Empty;
            }

            string jniTypeName;
            switch (fullTypeName)
            {
                case "System.Void":
                {
                    jniTypeName = "void";
                    break;
                }
                case "System.Object":
                {
                    jniTypeName = "jobject";
                    break;
                }
                case "System.IntPtr":
                {
                    jniTypeName = "jlong";
                    break;
                }
                case "System.Boolean":
                {
                    jniTypeName = "jboolean";
                    break;
                }
                case "System.Char":
                {
                    jniTypeName = "jchar";
                    break;
                }
                case "System.String":
                {
                    jniTypeName = "jstring";
                    break;
                }
                case "System.Byte":
                {
                    jniTypeName = "jbyte";
                    break;
                }
                case "System.SByte":
                {
                    jniTypeName = "jbyte";
                    break;
                }
                case "System.Int16":
                {
                    jniTypeName = "jshort";
                    break;
                }
                case "System.UInt16":
                {
                    jniTypeName = "jshort";
                    break;
                }
                case "System.Int32":
                {
                    jniTypeName = "jint";
                    break;
                }
                case "System.UInt32":
                {
                    jniTypeName = "jint";
                    break;
                }
                case "System.Int64":
                {
                    jniTypeName = "jlong";
                    break;
                }
                case "System.UInt64":
                {
                    jniTypeName = "jlong";
                    break;
                }
                case "System.Single":
                {
                    jniTypeName =  "jfloat";
                    break;
                }
                case "System.Double":
                {
                    jniTypeName = "jdouble";
                    break;
                }
                default:
                {
                    if (isRef)
                        jniTypeName = "jlong";
                    else
                        jniTypeName = syntax.GetTypeIdentifier();
                    break;
                }
            }

            return jniTypeName + javaArraySuffix;
        }
    }
}
