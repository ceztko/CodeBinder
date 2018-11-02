using CodeTranslator.Shared;
using CodeTranslator.Util;
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
            WriteType(parameter.Type);
            Builder.Append(" ").Append(parameter.Identifier.Text);
        }

        protected void WriteType(TypeSyntax type)
        {
            Builder.Append(type.GetJavaType(this));
        }

        protected virtual void WriteReturnType() { }

        public abstract string MethodName { get; }
    }

    class MethodWriter : MethodWriter<MethodDeclarationSyntax>
    {
        public MethodWriter(MethodDeclarationSyntax method, ICompilationContextProvider context)
            : base(method, context) { }

        protected override void WriteReturnType()
        {
            WriteType(Syntax.ReturnType);
            Builder.Append(" ");
        }

        public override string MethodName
        {
            get { return Syntax.GetName(); }
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
