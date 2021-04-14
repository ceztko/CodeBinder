// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;
using CodeBinder.Shared.CSharp;
using CodeBinder.Util;
using CodeBinder.Shared;

namespace CodeBinder.JNI
{
    static class JNIMethodExtensions
    {
        public static string GetJNIMethodName(this MethodDeclarationSyntax method, JNIModuleContext module)
        {
            return getJNIMethodName(method.GetName(), method, module);
        }

        static string getJNIMethodName(string methodName, MethodDeclarationSyntax method, JNIModuleContext module)
        {
            var parentType = method.Parent!.GetDeclaredSymbol(module)!;
            StringBuilder builder = new StringBuilder();
            string mappedns = module.Compilation.Conversion.NamespaceMapping.GetMappedNamespace(method.GetContainingNamespace(module),
                NamespaceNormalization.LowerCase);
            builder.Append("Java_").Append(mappedns.Replace('.', '_')).Append("_")
                .Append(parentType.GetQualifiedName().Replace('.', '_')).Append("_").Append(methodName);
            return builder.ToString();
        }

        public static string GetJNIType(this ParameterSyntax parameter, ICompilationContextProvider provider)
        {
            var symbol = parameter.Type!.GetTypeSymbol(provider);
            bool isByRef = parameter.IsRef() || parameter.IsOut();
            return getJNIType(symbol, isByRef);
        }

        public static string GetJNIReturnType(this MethodDeclarationSyntax method, ICompilationContextProvider provider)
        {
            var symbol = method.ReturnType.GetTypeSymbol(provider);
            return getJNIType(symbol, false);
        }

        private static string getJNIType(ITypeSymbol symbol, bool isByRef)
        {
            string typeName = symbol.GetFullName();
            if (symbol.TypeKind == TypeKind.Enum)
            {
                if (isByRef)
                    return "jIntegerBox";
                else
                    return "jint";
            }

            string jniTypeSuffix = string.Empty;
            if (symbol.TypeKind == TypeKind.Array)
            {
                var arrayType = (IArrayTypeSymbol)symbol;
                typeName = arrayType.ElementType.GetFullName();
                jniTypeSuffix = "Array";
            }

            string jniTypeName;
            if (isByRef)
                jniTypeName = getJNIByRefType(typeName, symbol);
            else
                jniTypeName = getJNIType(typeName, symbol);

            return jniTypeName + jniTypeSuffix;
        }

        static string getJNIType(string typeName, ITypeSymbol symbol)
        {
            switch (typeName)
            {
                case "System.Void":
                    return "void";
                case "System.Object":
                    return "jobject";
                case "CodeBinder.cbstring":
                    return "jstring";
                case "System.Runtime.InteropServices.HandleRef":
                    return "jHandleRef";
                case "System.UIntPtr":
                case "System.IntPtr":
                    return "jptr";
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
                    if (symbol == null || symbol.TypeKind == TypeKind.Class)
                        return "jobject";
                    else
                        throw new Exception("Unsupported by type " + typeName);
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
                case "CodeBinder.cbstring":
                    return "jStringBox";
                default:
                {
                    if (symbol?.TypeKind == TypeKind.Struct)
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

        public static CodeBuilder CommaSeparator(this CodeBuilder builder)
        {
            return builder.Append(", ");
        }

        public static CodeBuilder CommaSeparator(this CodeBuilder builder, ref bool first)
        {
            if (first)
                first = false;
            else
                return builder.CommaSeparator();

            return builder;
        }

        public static CodeBuilder Block(this CodeBuilder builder, bool appendLine = true)
        {
            builder.AppendLine("{");
            return builder.Indent("}", appendLine);
        }

        public static CodeBuilder ParameterList(this CodeBuilder builder, bool multiLine = false)
        {
            if (multiLine)
            {
                builder.AppendLine("(");
                return builder.Indent(")");
            }
            else
            {
                builder.Append("(");
                return builder.Using(")");
            }
        }

        /// <param name="childIstance">False to use in using directive, true to use in a single line</param>
        public static CodeBuilder Parenthesized(this CodeBuilder builder, bool childIstance = true)
        {
            builder.Append("(");
            if (childIstance)
                return builder.UsingChild(")");
            else
                return builder.Using(")");
        }

        /// <param name="childIstance">False to use in using directive, true to use in a single line</param>
        public static CodeBuilder AngleBracketed(this CodeBuilder builder, bool childIstance = true)
        {
            builder.Append("<");
            if (childIstance)
                return builder.UsingChild(">");
            else
                return builder.Using(">");
        }
    }
}
