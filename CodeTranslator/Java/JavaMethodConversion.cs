using CodeTranslator.Shared;
using CodeTranslator.Util;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CodeTranslator.Shared.CSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.Java
{
    abstract class BaseMethodWriter : BaseWriter
    {
        protected BaseMethodWriter(ICompilationContextProvider context)
            : base(context) { }

        protected override void Write()
        {
            Builder.Append(Method.GetJavaModifiersString());
            Builder.Append(" ");
            WriteReturnType();
            Builder.Append(MethodName);
            if (Method.ParameterList.Parameters.Count == 0)
            {
                Builder.Append("()");
            }
            else
            {
                using (Builder.BeginParameterList())
                {
                    WriteParameters(Method.ParameterList);
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
            WriteType(parameter.Type);
            Builder.Append(" ").Append(parameter.Identifier.Text);
        }

        protected void WriteType(TypeSyntax type)
        {
            Builder.Append(type.GetJavaType(this));
        }

        protected virtual void WriteReturnType() { }

        public BaseMethodDeclarationSyntax Method
        {
            get { return GetMethod(); }
        }

        public abstract string MethodName { get; }

        protected abstract BaseMethodDeclarationSyntax GetMethod();
    }

    abstract class MethodWriter<TMethod> : BaseMethodWriter
        where TMethod : BaseMethodDeclarationSyntax
    {
        public new TMethod Method { get; private set; }

        protected MethodWriter(TMethod method, ICompilationContextProvider context)
            : base(context)
        {
            Method = method;
        }

        protected override BaseMethodDeclarationSyntax GetMethod()
        {
            return Method;
        }
    }

    class MethodWriter : MethodWriter<MethodDeclarationSyntax>
    {
        public MethodWriter(MethodDeclarationSyntax method, ICompilationContextProvider context)
            : base(method, context) { }

        protected override void WriteReturnType()
        {
            WriteType(Method.ReturnType);
        }

        public override string MethodName
        {
            get { return Method.GetName(); }
        }
    }

    class ConstructorWriter : MethodWriter<ConstructorDeclarationSyntax>
    {
        public ConstructorWriter(ConstructorDeclarationSyntax method, ICompilationContextProvider context)
            : base(method, context) { }

        public override string MethodName
        {
            get { return (Method.Parent as BaseTypeDeclarationSyntax).GetName();}
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
    }
}
