using CodeTranslator.Shared;
using CodeTranslator.Util;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CodeTranslator.Shared.CSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.Java
{
    abstract class MethodWriter<TMethod> : ContextWriter<TMethod>
        where TMethod : BaseMethodDeclarationSyntax
    {
        protected MethodWriter(TMethod method, ICompilationContextProvider context)
            : base(method, context) { }

        protected override void Write()
        {
            WriteModifiers();
            WriteReturnType();
            Builder.Append(MethodName);
            WriteParameters();
            WriteMethodBody();
        }

        protected virtual void WriteParameters()
        {
            if (Context.ParameterList.Parameters.Count == 0)
            {
                Builder.Append("()");
            }
            else
            {
                using (Builder.BeginParameterList())
                {
                    WriteParameters(Context.ParameterList);
                }
            }
        }

        protected virtual void WriteMethodBody()
        {
            if (Context.Body == null)
                Builder.EndOfLine();
            else
                Builder.Space().Append(new BlockStatementWriter(Context.Body, this));
        }

        protected virtual void WriteModifiers()
        {
            Builder.Append(Context.GetJavaModifiersString());
            Builder.Space();
        }

        private void WriteParameters(ParameterListSyntax list)
        {
            bool first = true;
            foreach (var parameter in list.Parameters)
            {
                if (first)
                    first = false;
                else
                    Builder.Append(",").AppendLine();

                WriteParameter(parameter);
            }
        }

        private void WriteParameter(ParameterSyntax parameter)
        {
            var flags = IsNative ? JavaTypeFlags.NativeMethod : JavaTypeFlags.None;
            bool isRef = parameter.IsRef() | parameter.IsOut();
            if (isRef)
                flags |= JavaTypeFlags.IsByRef;

            WriteType(parameter.Type, flags);
            Builder.Space().Append(parameter.Identifier.Text);
        }

        protected void WriteType(TypeSyntax type, JavaTypeFlags flags)
        {
            Builder.Append(type.GetJavaType(flags, this));
        }

        protected virtual void WriteReturnType() { }

        public abstract string MethodName { get; }

        public abstract bool IsNative { get; }
    }

    class MethodWriter : MethodWriter<MethodDeclarationSyntax>
    {
        public MethodWriter(MethodDeclarationSyntax method, ICompilationContextProvider context)
            : base(method, context) { }

        protected override void WriteModifiers()
        {
            if (IsParentInterface)
                return;

            base.WriteModifiers();
        }

        protected override void WriteReturnType()
        {
            WriteType(Context.ReturnType, IsNative ? JavaTypeFlags.NativeMethod : JavaTypeFlags.None);
            Builder.Space();
        }

        protected override void WriteMethodBody()
        {
            if (IsParentInterface)
                Builder.EndOfLine();
            else
                base.WriteMethodBody();
        }

        public bool IsParentInterface
        {
            get { return Context.Parent.Kind() == SyntaxKind.InterfaceDeclaration; }
        }

        public override string MethodName
        {
            get { return Context.GetName(); }
        }

        public override bool IsNative
        {
            get { return Context.IsNative(this); }
        }
    }

    class SignatureMethodWriter : MethodWriter<MethodDeclarationSyntax>
    {
        MethodSignatureInfo _signature;

        public SignatureMethodWriter(MethodSignatureInfo signature, MethodDeclarationSyntax method, ICompilationContextProvider context)
            : base(method, context)
        {
            _signature = signature;
        }

        protected override void WriteModifiers()
        {
            Builder.Append(_signature.GetJavaModifiersString()).Space();
        }

        protected override void WriteReturnType()
        {
            Builder.Append(_signature.ReturnType.GetJavaTypeName(JavaTypeFlags.NativeMethod)).Space();
        }

        protected override void WriteMethodBody()
        {
            Builder.EndOfLine();
        }

        protected override void WriteParameters()
        {
            for (int i = 0; i < _signature.Parameters.Length; i++)
                WriteParameter(ref _signature.Parameters[i]);
        }

        private void WriteParameter(ref MethodParameterInfo parameter)
        {
            Builder.Append(",").Space().Append(parameter.GetJavaTypeName(JavaTypeFlags.NativeMethod)).Space().Append(parameter.Name);
        }

        public override string MethodName
        {
            get { return _signature.MethodName; }
        }

        public override bool IsNative
        {
            get { return true; } // TODO: Check method native
        }
    }

    class ConstructorWriter : MethodWriter<ConstructorDeclarationSyntax>
    {
        public ConstructorWriter(ConstructorDeclarationSyntax method, ICompilationContextProvider context)
            : base(method, context) { }

        public override string MethodName
        {
            get { return (Context.Parent as BaseTypeDeclarationSyntax).GetName();}
        }

        public override bool IsNative
        {
            get { return false; }
        }
    }

    class DestructorWriter : MethodWriter<DestructorDeclarationSyntax>
    {
        public DestructorWriter(DestructorDeclarationSyntax method, ICompilationContextProvider context)
            : base(method, context) { }

        protected override void WriteModifiers()
        {
            Builder.Append("protected").Space();
        }

        protected override void WriteReturnType()
        {
            Builder.Append("void").Space();
        }

        public override string MethodName
        {
            get { return "finalize"; }
        }

        public override bool IsNative
        {
            get { return false; }
        }
    }
}
