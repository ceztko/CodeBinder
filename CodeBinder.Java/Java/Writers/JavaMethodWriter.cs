using CodeBinder.Shared;
using CodeBinder.Util;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CodeBinder.Shared.CSharp;
using System;
using System.Collections.Generic;
using System.Text;
using CodeBinder.Shared.Java;
using Microsoft.CodeAnalysis;

namespace CodeBinder.Java
{
    abstract class MethodWriter<TMethod> : JavaCodeWriter<TMethod>
        where TMethod : BaseMethodDeclarationSyntax
    {
        protected MethodWriter(TMethod method, JavaCodeConversionContext context)
            : base(method, context) { }

        protected override void Write()
        {
            WriteModifiers();
            if (Arity != 0)
                WriteTypeParameters();

            WriteReturnType();
            Builder.Append(MethodName);
            WriteParameters();
            WriteThrows();
            writeMethodBody();
        }

        protected virtual void WriteParameters()
        {
            if (Item.ParameterList.Parameters.Count == 0)
            {
                Builder.EmptyParameterList();
            }
            else if (Item.ParameterList.Parameters.Count == 1)
            {
                using (Builder.ParameterList())
                {
                    writeParameters(Item.ParameterList);
                }
            }
            else
            {
                using (Builder.Indent())
                {
                    using (Builder.ParameterList(true))
                    {
                        writeParameters(Item.ParameterList);
                        Builder.AppendLine();
                    }
                }
            }
        }

        protected virtual void WriteModifiers()
        {
            string modifiers = Item.GetJavaModifiersString();
            if (!string.IsNullOrEmpty(modifiers))
                Builder.Append(modifiers).Space();
        }

        protected void WriteType(TypeSyntax type, JavaTypeFlags flags)
        {
            Builder.Append(type.GetJavaType(flags, Context));
        }

        void writeMethodBody()
        {
            if (Item.Body == null || !WriteMethodBody)
            {
                Builder.EndOfStatement();
            }
            else
            {
                using (Builder.AppendLine().Block())
                {
                    WriteMethodBodyInternal();
                    if (!Context.Conversion.SkipBody)
                        Builder.Append(Item.Body, Context, true).AppendLine();
                }
            }
        }

        private void writeParameters(ParameterListSyntax list)
        {
            bool first = true;
            foreach (var parameter in list.Parameters)
            {
                Builder.CommaAppendLine(ref first);
                writeParameter(parameter);
            }
        }

        private void writeParameter(ParameterSyntax parameter)
        {
            var flags = IsNative ? JavaTypeFlags.NativeMethod : JavaTypeFlags.None;
            bool isRef = parameter.IsRef() | parameter.IsOut();
            if (isRef)
                flags |= JavaTypeFlags.IsByRef;

            WriteType(parameter.Type, flags);
            Builder.Space().Append(parameter.Identifier.Text);
        }


        protected virtual void WriteThrows() { /* Do nothing */ }

        protected virtual void WriteTypeParameters() { /* Do nothing */ }

        protected virtual void WriteMethodBodyInternal() { /* Do nothing */ }

        protected virtual void WriteReturnType() { /* Do nothing */ }

        public virtual bool WriteMethodBody
        {
            get { return true; }
        }

        public virtual int Arity
        {
            get { return 0; }
        }

        public abstract string MethodName { get; }

        public abstract bool IsNative { get; }
    }

    class MethodWriter : MethodWriter<MethodDeclarationSyntax>
    {
        public MethodWriter(MethodDeclarationSyntax method, JavaCodeConversionContext context)
            : base(method, context) { }

        protected override void WriteModifiers()
        {
            if (IsParentInterface)
                return;

            base.WriteModifiers();
        }

        protected override void WriteTypeParameters()
        {
            Builder.Append(Item.GetTypeParameters(this), Context).Space();
        }

        protected override void WriteReturnType()
        {
            WriteType(Item.ReturnType, IsNative ? JavaTypeFlags.NativeMethod : JavaTypeFlags.None);
            Builder.Space();
        }

        protected override void WriteMethodBodyInternal()
        {
            if (Context.Conversion.SkipBody)
                Builder.Append(Item.ReturnType.GetJavaDefaultReturnStatement(Context)).EndOfStatement();
        }

        public bool IsParentInterface
        {
            get { return Item.Parent.Kind() == SyntaxKind.InterfaceDeclaration; }
        }

        public override string MethodName
        {
            get
            {
                // Try first look for replacements
                var methodSymbol = Item.GetDeclaredSymbol<IMethodSymbol>(this);
                if (methodSymbol.HasJavaReplacement(out var replacement))
                    return replacement;

                var methodName = Item.GetName();
                if (IsNative)
                    return methodName;
                else
                    return methodName.ToJavaCase();
            }
        }

        public override bool IsNative
        {
            get { return Item.IsNative(this); }
        }

        public override int Arity
        {
            get { return Item.Arity; }
        }
    }

    class SignatureMethodWriter : MethodWriter<MethodDeclarationSyntax>
    {
        MethodSignatureInfo _signature;

        public SignatureMethodWriter(MethodSignatureInfo signature, MethodDeclarationSyntax method, JavaCodeConversionContext context)
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
            Builder.Append(_signature.ReturnType.GetJavaType(JavaTypeFlags.NativeMethod)).Space();
        }

        protected override void WriteMethodBodyInternal()
        {
            Builder.EndOfStatement();
        }

        protected override void WriteParameters()
        {
            for (int i = 0; i < _signature.Parameters.Length; i++)
                WriteParameter(ref _signature.Parameters[i]);
        }

        private void WriteParameter(ref MethodParameterInfo parameter)
        {
            Builder.CommaSeparator().Append(parameter.GetJavaType(JavaTypeFlags.NativeMethod)).Space().Append(parameter.Name);
        }

        public override string MethodName
        {
            get { return _signature.MethodName; }
        }

        public override bool WriteMethodBody
        {
            get { return false; }
        }

        public override bool IsNative
        {
            get { return true; } // TODO: Check if the method is really native?
        }
    }

    class ConstructorWriter : MethodWriter<ConstructorDeclarationSyntax>
    {
        bool _isStatic;

        public ConstructorWriter(ConstructorDeclarationSyntax method, JavaCodeConversionContext context)
            : base(method, context)
        {
            _isStatic = Item.Modifiers.Any(SyntaxKind.StaticKeyword);
        }

        protected override void WriteModifiers()
        {
            if (!_isStatic)
                base.WriteModifiers();
        }

        protected override void WriteParameters()
        {
            if (!_isStatic)
                base.WriteParameters();
        }

        protected override void WriteMethodBodyInternal()
        {
            if (Item.Initializer != null)
                Builder.Append(Item.Initializer, Context).EndOfStatement();
        }

        public override string MethodName
        {
            get
            {
                if (_isStatic)
                    return "static";
                else
                    return (Item.Parent as BaseTypeDeclarationSyntax).GetName();
            }
        }

        public override bool IsNative
        {
            get { return false; }
        }
    }

    class DestructorWriter : MethodWriter<DestructorDeclarationSyntax>
    {
        public DestructorWriter(DestructorDeclarationSyntax method, JavaCodeConversionContext context)
            : base(method, context) { }

        protected override void WriteThrows()
        {
            Builder.Space().Append("throws Throwable");
        }

        protected override void WriteMethodBodyInternal()
        {
            Builder.Append("super.finalize()").EndOfStatement();
        }

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
