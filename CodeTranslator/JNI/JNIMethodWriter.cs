using CodeTranslator.Shared;
using CodeTranslator.Shared.CSharp;
using CodeTranslator.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.JNI
{
    abstract class MethodWriter : ContextWriter<(MethodDeclarationSyntax Method, JNIModuleConversion Module)>
    {
        protected MethodWriter(MethodDeclarationSyntax method, JNIModuleConversion module)
            : base((method, module), module) { }

        public static MethodWriter Create(MethodDeclarationSyntax method,
            JNIModuleConversion module)
        {
            return new SyntaxSignatureMethodWriter(method, module);
        }

        public static MethodWriter Create(MethodSignatureInfo signature, MethodDeclarationSyntax method,
            JNIModuleConversion module)
        {
            return new CustomSignatureMethodWriter(signature, method, module);
        }

        protected override void Write()
        {
            Builder.Append("JNIEXPORT").Space();
            Builder.Append(ReturnType).Space();
            Builder.Append("JNICALL").Space();
            Builder.Append("Java_").Append(Context.Module.Namespace.Replace('.', '_')).Append("_")
                .Append(Context.Module.JNIModuleName).Append("_").Append(MethodName).AppendLine("(");
            Builder.IncreaseIndent();
            Builder.Append("JNIEnv *, jclass");
            WriteParameters();
            Builder.Append(")").EndOfLine();
            Builder.DecreaseIndent();
        }

        public abstract string MethodName
        {
            get;
        }

        public abstract string ReturnType
        {
            get;
        }

        protected abstract void WriteParameters();

        class SyntaxSignatureMethodWriter : MethodWriter
        {
            public SyntaxSignatureMethodWriter(MethodDeclarationSyntax method, JNIModuleConversion module)
                : base(method, module) { }

            protected override void WriteParameters()
            {
                foreach (var parameter in Context.Method.ParameterList.Parameters)
                    WriteParameter(parameter);
            }

            private void WriteParameter(ParameterSyntax parameter)
            {
                Builder.Append(",").Space();
                bool isRef = parameter.IsRef() || parameter.IsOut();
                WriteType(parameter.Type, isRef);
                Builder.Append(parameter.Identifier.Text);
            }

            public override string ReturnType
            {
                get { return Context.Method.ReturnType.GetJNIType(false, this); }
            }

            public override string MethodName
            {
                get { return Context.Method.GetName(); }
            }

            private void WriteType(TypeSyntax type, bool isRef)
            {
                Builder.Append(type.GetJNIType(isRef, this)).Space();
            }
        }

        class CustomSignatureMethodWriter : MethodWriter
        {
            MethodSignatureInfo _signature;

            public CustomSignatureMethodWriter(MethodSignatureInfo signature, MethodDeclarationSyntax method, JNIModuleConversion module)
                : base(method, module)
            {
                _signature = signature;
            }

            protected override void WriteParameters()
            {
                for (int i = 0; i < _signature.Parameters.Length; i++)
                    WriteParameter(ref _signature.Parameters[i]);
            }

            private void WriteParameter(ref MethodParameterInfo parameter)
            {
                Builder.Append(",").Space().Append(parameter.GetJNITypeName()).Space().Append(parameter.Name);
            }

            public override string ReturnType
            {
                get { return _signature.ReturnType.GetJNITypeName(); }
            }

            public override string MethodName
            {
                get { return _signature.MethodName; }
            }
        }
    }
}
