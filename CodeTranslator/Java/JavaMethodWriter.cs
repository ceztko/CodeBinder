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
    abstract class MethodWriter<TMethod> : SyntaxWriter<TMethod>
        where TMethod : BaseMethodDeclarationSyntax
    {
        protected MethodWriter(TMethod method, ICompilationContextProvider context)
            : base(method, context) { }

        protected override void Write()
        {
            WriteModifiers();
            WriteReturnType();
            Builder.Append(MethodName);
            if (Syntax.ParameterList.Parameters.Count == 0)
            {
                Builder.Append("()");
            }
            else
            {
                using (Builder.BeginParameterList())
                {
                    WriteParameters(Syntax.ParameterList);
                }
            }
            WriteMethodBody();
        }

        protected virtual void WriteMethodBody()
        {
            if (Syntax.Body != null)
                Builder.Space().Append(new BlockWriter(Syntax.Body, this));
        }

        protected virtual void WriteModifiers()
        {
            Builder.Append(Syntax.GetJavaModifiersString());
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
                    Builder.Append(",").Space();

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
            WriteType(Syntax.ReturnType, IsNative ? JavaTypeFlags.NativeMethod : JavaTypeFlags.None);
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
            get { return Syntax.Parent.Kind() == SyntaxKind.InterfaceDeclaration; }
        }

        public override string MethodName
        {
            get { return Syntax.GetName(); }
        }

        public override bool IsNative
        {
            get { return Syntax.IsNative(this); }
        }
    }

    class ConstructorWriter : MethodWriter<ConstructorDeclarationSyntax>
    {
        public ConstructorWriter(ConstructorDeclarationSyntax method, ICompilationContextProvider context)
            : base(method, context) { }

        public override string MethodName
        {
            get { return (Syntax.Parent as BaseTypeDeclarationSyntax).GetName();}
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
            Builder.Append("protected ");
        }

        protected override void WriteReturnType()
        {
            Builder.Append("void ");
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
