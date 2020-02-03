using CodeBinder.Shared;
using CodeBinder.Shared.CSharp;
using CodeBinder.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.JNI
{
    abstract class MethodWriter : CodeWriter<MethodDeclarationSyntax, JNIModuleConversion>
    {
        protected MethodWriter(MethodDeclarationSyntax method, JNIModuleConversion module)
            : base(method, module) { }

        public static MethodWriter Create(MethodDeclarationSyntax method,
            JNIModuleConversion module)
        {
            return new SyntaxSignatureMethodWriter(method, module);
        }

        protected override void Write()
        {
            Builder.Append("JNIEXPORT").Space();
            Builder.Append(ReturnType).Space();
            Builder.Append("JNICALL").Space();
            Builder.Append(MethodName).AppendLine("(");
            using (Builder.Indent())
            {
                Builder.Append("JNIEnv *, jclass");
                WriteParameters();
                Builder.Append(")").EndOfLine();
            }
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
                foreach (var parameter in Item.ParameterList.Parameters)
                    WriteParameter(parameter);
            }

            private void WriteParameter(ParameterSyntax parameter)
            {
                Builder.CommaSeparator();
                Builder.Append(parameter.GetJNIType(Context)).Space();
                Builder.Append(parameter.Identifier.Text);
            }

            public override string ReturnType
            {
                get { return Item.GetJNIReturnType(Context); }
            }

            public override string MethodName
            {
                get { return Item.GetJNIMethodName(Context.Context); }
            }
        }
    }
}
