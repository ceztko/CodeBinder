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
            return getJNIType(symbol, isRef);
        }

        private static string getJNIType(ITypeSymbol type, bool isRef)
        {
            if (type.TypeKind == TypeKind.Enum)
                return "jint";

            string typeName;
            string javaArraySuffix;
            if (type.TypeKind == TypeKind.Array)
            {
                var arrayType = type as IArrayTypeSymbol;

                typeName = arrayType.ElementType.GetFullName();
                javaArraySuffix = "Array";
            }
            else
            {
                typeName = type.GetFullName();
                javaArraySuffix = string.Empty;
            }

            string javaTypeName;
            switch (typeName)
            {
                case "System.Void":
                {
                    javaTypeName = "void";
                    break;
                }
                case "System.Object":
                {
                    javaTypeName = "jobject";
                    break;
                }
                case "System.IntPtr":
                {
                    javaTypeName = "jlong";
                    break;
                }
                case "System.Boolean":
                {
                    javaTypeName = "jboolean";
                    break;
                }
                case "System.Char":
                {
                    javaTypeName = "jchar";
                    break;
                }
                case "System.String":
                {
                    javaTypeName = "jstring";
                    break;
                }
                case "System.Byte":
                {
                    javaTypeName = "jbyte";
                    break;
                }
                case "System.SByte":
                {
                    javaTypeName = "jbyte";
                    break;
                }
                case "System.Int16":
                {
                    javaTypeName = "jshort";
                    break;
                }
                case "System.UInt16":
                {
                    javaTypeName = "jshort";
                    break;
                }
                case "System.Int32":
                {
                    javaTypeName = "jint";
                    break;
                }
                case "System.UInt32":
                {
                    javaTypeName = "jint";
                    break;
                }
                case "System.Int64":
                {
                    javaTypeName = "jlong";
                    break;
                }
                case "System.UInt64":
                {
                    javaTypeName = "jlong";
                    break;
                }
                case "System.Single":
                {
                    javaTypeName =  "jfloat";
                    break;
                }
                case "System.Double":
                {
                    javaTypeName = "jdouble";
                    break;
                }
                default:
                {
                    if (isRef)
                        javaTypeName = "jlong";
                    else
                        javaTypeName = typeName;
                    break;
                }
            }

            return javaTypeName + javaArraySuffix;
        }
    }
}
