using CodeTranslator.Attributes;
using CodeTranslator.Shared;
using CodeTranslator.Shared.CSharp;
using CodeTranslator.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.Java
{
    abstract class TypeWriter<TBaseType> : CodeWriter<TBaseType>
        where TBaseType : BaseTypeDeclarationSyntax
    {
        protected TypeWriter(TBaseType syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            var modifiers = Context.GetJavaModifiersString();
            if (!string.IsNullOrEmpty(modifiers))
                Builder.Append(modifiers).Space();

            if (NeedStaticKeyword)
            {
                var parentKind = Context.Parent.Kind();
                switch (parentKind)
                {
                    case SyntaxKind.ClassDeclaration:
                    case SyntaxKind.StructDeclaration:
                        Builder.Append("static").Space();
                        break;
                }
            }

            Builder.Append(Context.GetJavaTypeDeclaration()).Space();
            Builder.Append(TypeName);
            if (Arity > 0)
            {
                Builder.Space();
                WriteTypeParameters();
            }

            if (Context.BaseList != null)
            {
                Builder.Space();
                WriteBaseTypes(Context.BaseList);
            }
            Builder.AppendLine();
            using (Builder.Block())
            {
                WriteTypeMembers();
            }
        }

        protected virtual void WriteTypeParameters() { }

        protected abstract void WriteTypeMembers();

        private void WriteBaseTypes(BaseListSyntax baseList)
        {
            bool first = true;
            bool isInterface = false;
            foreach (var type in baseList.Types)
            {
                if (first)
                    first = false;
                else if (isInterface)
                    Builder.CommaSeparator();
                else
                    Builder.Space();

                string javaTypeName = type.Type.GetJavaType(this, out isInterface);
                Builder.Append(isInterface ? "implements" : "extends").Space().Append(javaTypeName);
            }
        }

        protected void WriteTypeMembers(SyntaxList<MemberDeclarationSyntax> members)
        {
            bool first = true;
            foreach (var member in members)
            {
                if (member.HasAttribute<IgnoreAttribute>(this))
                    continue;

                foreach (var writer in member.GetWriters(this))
                {
                    if (first)
                        first = false;
                    else
                        Builder.AppendLine();

                    Builder.Append(writer);
                }
            }
        }

        public virtual int Arity
        {
            get { return 0; }
        }

        public virtual string TypeName
        {
            get { return Context.GetName(); }
        }

        public virtual bool NeedStaticKeyword
        {
            get { return false; }
        }
    }
}
