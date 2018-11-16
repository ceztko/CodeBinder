using CodeTranslator.Shared;
using CodeTranslator.Shared.CSharp;
using CodeTranslator.Util;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.Java
{
    abstract class PropertyWriter<TProperty> : CodeWriter<TProperty>
            where TProperty : BasePropertyDeclarationSyntax
    {
        SyntaxKind[] _modifiers;
        bool _isAutoProperty;

        protected PropertyWriter(TProperty syntax, ICompilationContextProvider context)
            : base(syntax, context)
        {
            _modifiers = Context.GetCSharpModifiers().ToArray();
            _isAutoProperty = true;
            if (Context.AccessorList != null)
            {
                foreach (var accessor in Context.AccessorList.Accessors)
                {
                    if (accessor.Body != null)
                    {
                        _isAutoProperty = false;
                        break;
                    }
                }
            }
        }

        protected override void Write()
        {
            WriteUnderlyingField();
            WriteAccessors(Context.AccessorList);
        }

        private void WriteUnderlyingField()
        {
            if (!_isAutoProperty || IsParentInterface)
                return;

            Builder.Append("private").Space();
            Builder.Append(JavaType).Space();
            Builder.Append(UnderlyingFieldName).EndOfStatement();
        }

        private void WriteAccessors(AccessorListSyntax accessorList)
        {
            foreach (var accessor in accessorList.Accessors)
                WriteAccessor(accessor);
        }

        private void WriteAccessor(AccessorDeclarationSyntax accessor)
        {
            switch (accessor.Keyword.Text)
            {
                case "get":
                    WriteGetter(accessor);
                    break;
                case "set":
                    WriteSetter(accessor);
                    break;
                default:
                    throw new Exception();
            }
        }

        private void WriteSetter(AccessorDeclarationSyntax accessor)
        {
            var modifiers = getSetterModifiers(accessor);
            Builder.Append(JavaExtensions.GetJavaPropertyModifiersString(modifiers)).Space();
            Builder.Append(JavaType).Space();
            Builder.Append(SetterName).Append("(").Append(JavaType).Space().Append("value").Append(")");
            if (IsParentInterface)
            {
                Builder.EndOfStatement();
            }
            else
            {
                if (_isAutoProperty)
                {
                    using (Builder.AppendLine().Block())
                    {
                        Builder.Append(UnderlyingFieldName).Space().Append("= value").EndOfStatement();
                    }
                }
                else
                {
                    Builder.Space().Append(accessor.Body, this).AppendLine();
                }
            }
        }

        private void WriteGetter(AccessorDeclarationSyntax accessor)
        {
            Builder.Append(JavaExtensions.GetJavaPropertyModifiersString(_modifiers)).Space();
            Builder.Append(JavaType).Space();
            Builder.Append(GetterName).Append("()");
            if (IsParentInterface)
            {
                Builder.EndOfStatement();

            }
            else
            {
                if (_isAutoProperty)
                {
                    using (Builder.AppendLine().Block())
                    {
                        Builder.Append("return").Space().Append(UnderlyingFieldName).EndOfStatement();
                    }
                }
                else
                {
                    Builder.Space().Append(accessor.Body, this).AppendLine();
                }
            }
        }

        List<SyntaxKind> getSetterModifiers(AccessorDeclarationSyntax accessor)
        {
            var ret = new List<SyntaxKind>();
            ret.AddRange(_modifiers.Skip(1));
            ret.AddRange(accessor.GetCSharpModifiers());
            return ret;
        }

        public bool IsParentInterface
        {
            get { return Context.Parent.Kind() == SyntaxKind.InterfaceDeclaration; }
        }

        public string UnderlyingFieldName
        {
            get { return "__" + PropertyName.ToJavaCase(); }
        }

        public string GetterName
        {
            get { return "get" + PropertyName; }
        }

        public string SetterName
        {
            get { return "set" + PropertyName; }
        }

        public string JavaType
        {
            get { return Context.Type.GetJavaType(this); }
        }

        public abstract string PropertyName { get; }
    }

    class PropertyWriter : PropertyWriter<PropertyDeclarationSyntax>
    {
        public PropertyWriter(PropertyDeclarationSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        public override string PropertyName
        {
            get { return Context.Identifier.Text; }
        }
    }

    class IndexerWriter : PropertyWriter<IndexerDeclarationSyntax>
    {
        public IndexerWriter(IndexerDeclarationSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        public override string PropertyName
        {
            get { return "At"; }
        }
    }
}
