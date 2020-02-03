using CodeBinder.Shared;
using CodeBinder.Util;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CodeBinder.Shared.CSharp;
using System;
using System.Collections.Generic;
using System.Text;
using CodeBinder.Java.Shared;
using Microsoft.CodeAnalysis;
using System.Diagnostics;

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
            int parameterCount = ParameterCount;
            if (parameterCount == 0)
            {
                Builder.EmptyParameterList();
            }
            else if (parameterCount == 1)
            {
                using (Builder.ParameterList())
                {
                    writeParameters(Item.ParameterList, parameterCount);
                }
            }
            else
            {
                using (Builder.Indent())
                {
                    using (Builder.ParameterList(true))
                    {
                        writeParameters(Item.ParameterList, parameterCount);
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
            if (Item.Body == null)
            {
                Builder.EndOfStatement();
            }
            else
            {
                using (Builder.AppendLine().Block())
                {
                    WriteMethodBodyInternal();
                    if (WriteMethodBody && !Context.Conversion.SkipBody)
                        Builder.Append(Item.Body, Context, true).AppendLine();
                }
            }
        }

        private void writeParameters(ParameterListSyntax list, int parameterCount)
        {
            bool first = true;
            for (int i = 0; i < parameterCount; i++)
            {
                var parameter = list.Parameters[i];
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

            WriteType(parameter.Type!, flags);
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

        public virtual int ParameterCount
        {
            get { return Item.ParameterList.Parameters.Count; }
        }

        public abstract string MethodName { get; }

        public abstract bool IsNative { get; }
    }

    class MethodWriter : MethodWriter<MethodDeclarationSyntax>
    {
        int _optionalIndex;

        public MethodWriter(MethodDeclarationSyntax method, int optionalIndex, JavaCodeConversionContext context)
            : base(method, context)
        {
            _optionalIndex = optionalIndex;
        }

        protected override void WriteModifiers()
        {
            if (IsParentInterface)
                return;

            base.WriteModifiers();
        }

        protected override void WriteTypeParameters()
        {
            Builder.Append(Item.GetTypeParameters(Context), Context).Space();
        }

        protected override void WriteReturnType()
        {
            WriteType(Item.ReturnType, IsNative ? JavaTypeFlags.NativeMethod : JavaTypeFlags.None);
            Builder.Space();
        }

        protected override void WriteMethodBodyInternal()
        {
            if (_optionalIndex >= 0)
            {
                var typeSymbol = Item.ReturnType.GetTypeSymbol(Context);
                if (typeSymbol.SpecialType != SpecialType.System_Void)
                    Builder.Append("return").Space();

                using (Builder.Append(MethodName).ParameterList())
                {
                    for (int i = 0; i < Item.ParameterList.Parameters.Count; i++)
                    {
                        var parameter = Item.ParameterList.Parameters[i];
                        if (i > 0)
                            Builder.CommaSeparator();

                        if (i < _optionalIndex)
                            Builder.Append(parameter.Identifier.Text);
                        else
                            Builder.Append(parameter.Default!.Value, Context);
                    }
                }

                Builder.EndOfStatement();
            }
            else
            {
                if (Context.Conversion.SkipBody)
                    Builder.Append(Item.ReturnType.GetJavaDefaultReturnStatement(Context)).EndOfStatement();
            }
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
                var methodSymbol = Item.GetDeclaredSymbol<IMethodSymbol>(Context);
                if (methodSymbol.HasJavaReplacement(out var replacement))
                    return replacement.Name;

                var methodName = Item.GetName();
                if (IsNative)
                {
                    return methodName;
                }
                else
                {
                    if (Context.Conversion.MethodsLowerCase)
                        return methodName.ToJavaLowerCase();
                    else
                        return methodName;
                }

            }
        }

        public override bool WriteMethodBody
        {
            get { return _optionalIndex == -1; }
        }

        public override bool IsNative
        {
            get { return Item.IsNative(Context); }
        }

        public override int Arity
        {
            get { return Item.Arity; }
        }

        public override int ParameterCount
        {
            get
            {
                if (_optionalIndex == -1)
                    return base.ParameterCount;

                return _optionalIndex;
            }
        }
    }

    class JavaConstructorWriter : MethodWriter<ConstructorDeclarationSyntax>
    {
        bool _isStatic;

        public JavaConstructorWriter(ConstructorDeclarationSyntax method, JavaCodeConversionContext context)
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
                    return (Item.Parent as BaseTypeDeclarationSyntax)!.GetName();
            }
        }

        public override bool IsNative
        {
            get { return false; }
        }
    }

    class JavaDestructorWriter : MethodWriter<DestructorDeclarationSyntax>
    {
        public JavaDestructorWriter(DestructorDeclarationSyntax method, JavaCodeConversionContext context)
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
