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
            Builder.Append(Syntax.GetJavaModifiersString());
            Builder.Append(" ");
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
            using (Builder.Append(" ").BeginBlock())
            {

            }
        }

        private void WriteParameters(ParameterListSyntax list)
        {
            bool first = true;
            foreach (var parameter in list.Parameters)
            {
                if (first)
                    first = false;
                else
                    Builder.Append(", ");

                WriteParameter(parameter);
            }
        }

        private void WriteParameter(ParameterSyntax parameter)
        {
            var flags = IsNative ? JavaMethodFlags.Native : JavaMethodFlags.None;
            bool isRef = parameter.IsRef() | parameter.IsOut();
            if (isRef)
                flags |= JavaMethodFlags.IsByRef;

            WriteType(parameter.Type, flags);
            Builder.Append(" ").Append(parameter.Identifier.Text);
        }

        protected void WriteType(TypeSyntax type, JavaMethodFlags flags)
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

        protected override void WriteReturnType()
        {
            WriteType(Syntax.ReturnType, IsNative ? JavaMethodFlags.Native : JavaMethodFlags.None);
            Builder.Append(" ");
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
