using CodeBinder.Attributes;
using CodeBinder.Shared;
using CodeBinder.Shared.CSharp;
using CodeBinder.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CodeBinder.Java
{
    [DebuggerDisplay("TypeName = {TypeName}")]
    abstract class BaseTypeWriter<TTypeDeclaration> : CodeWriter<TTypeDeclaration>
        where TTypeDeclaration : BaseTypeDeclarationSyntax
    {
        protected BaseTypeWriter(TTypeDeclaration syntax, ICompilationContextProvider context)
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

        protected void WriteTypeMembers(IEnumerable<MemberDeclarationSyntax> members, PartialDeclarationsTree partialDeclarations)
        {
            bool first = true;
            foreach (var member in members)
            {
                if (member.ShouldDiscard(this))
                    continue;

                foreach (var writer in member.GetWriters(partialDeclarations, this))
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

    abstract class TypeWriter<TTypeDeclaration> : BaseTypeWriter<TTypeDeclaration>
        where TTypeDeclaration : TypeDeclarationSyntax
    {
        PartialDeclarationsTree _partialDeclarations;

        protected TypeWriter(TTypeDeclaration syntax, PartialDeclarationsTree partialDeclarations, ICompilationContextProvider context)
            : base(findMainDeclaration(syntax, partialDeclarations), context)
        {
            _partialDeclarations = partialDeclarations;
        }

        protected override void WriteTypeMembers()
        {
            if (_partialDeclarations.PartialDeclarations.Count == 0)
                WriteTypeMembers(Context.Members, _partialDeclarations);
            else
                WriteTypeMembers(getPartialDeclarationMembers(), _partialDeclarations);
        }

        IEnumerable<MemberDeclarationSyntax> getPartialDeclarationMembers()
        {
            foreach (var declaration in _partialDeclarations.PartialDeclarations)
            {
                foreach (var member in declaration.Members)
                {
                    switch (member.Kind())
                    {
                        case SyntaxKind.InterfaceDeclaration:
                        case SyntaxKind.ClassDeclaration:
                        case SyntaxKind.StructDeclaration:
                        {
                            if (_partialDeclarations.MemberPartialDeclarations.ContainsKey(member as TypeDeclarationSyntax))
                                yield return member;
                            break;
                        }
                        default:
                        {
                            yield return member;
                            break;
                        }
                    }
                }
            }
        }

        static TTypeDeclaration findMainDeclaration(TTypeDeclaration syntax, PartialDeclarationsTree partialDeclarations)
        {
            // If there are no partial declarations, just return the given syntax
            if (partialDeclarations.PartialDeclarations.Count == 0)
                return syntax;

            // Find the declaration with non null base list, or just return the first
            TypeDeclarationSyntax ret = null;
            foreach (var declaration in partialDeclarations.PartialDeclarations)
            {
                ret = declaration;
                if (declaration.BaseList != null)
                    break;
            }

            return (TTypeDeclaration)ret;
        }
    }
}
