using CodeBinder.Shared;
using CodeBinder.Shared.CSharp;
using CodeBinder.Util;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Java
{
    abstract class PropertyWriter<TProperty> : JavaCodeWriter<TProperty>
            where TProperty : BasePropertyDeclarationSyntax
    {
        SyntaxKind[] _modifiers;
        bool _isAutoProperty;
        bool _isParentInterface;

        protected PropertyWriter(TProperty syntax, JavaCodeConversionContext context)
            : base(syntax, context)
        {
            _modifiers = Item.GetCSharpModifiers().ToArray();

            if (_modifiers.Contains(SyntaxKind.AbstractKeyword))
            {
                _isAutoProperty = false;
            }
            else
            {
                _isAutoProperty = true;
                foreach (var accessor in Item.AccessorList!.Accessors)
                {
                    if (accessor.Body != null)
                    {
                        _isAutoProperty = false;
                        break;
                    }
                }
            }

            _isParentInterface = Item.Parent!.Kind() == SyntaxKind.InterfaceDeclaration;
        }

        protected override void Write()
        {
            WriteUnderlyingField();
            WriteAccessors(Item.AccessorList!);
        }

        private void WriteUnderlyingField()
        {
            if (!_isAutoProperty || _isParentInterface)
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
            switch (accessor.Keyword.Kind())
            {
                case SyntaxKind.GetKeyword:
                    WriteGetter(accessor);
                    break;
                case SyntaxKind.SetKeyword:
                    WriteSetter(accessor);
                    break;
                default:
                    throw new Exception();
            }
        }

        private void WriteSetter(AccessorDeclarationSyntax accessor)
        {
            if (!_isParentInterface)
            {
                var modifiers = getSetterModifiers(accessor);
                string javaModifiers;
                if (modifiers.Count != 0 && (javaModifiers = JavaUtils.GetPropertyModifiersString(modifiers)).Length != 0)
                    Builder.Append(javaModifiers).Space();
            }
            Builder.Append("void").Space();
            using (Builder.Append(SetterName).ParameterList())
            {
                WriteAccessorParameters(true);
                Builder.Append(JavaType).Space().Append("value");
            }

            if (_isParentInterface)
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
                    if (accessor.Body == null)
                    {
                        Builder.EndOfStatement();
                    }
                    else
                    {
                        using (Builder.AppendLine().Block())
                        {
                            if (!Context.Conversion.SkipBody)
                                Builder.Append(accessor.Body, Context, true).AppendLine();
                        }
                    }
                }
            }
        }

        private void WriteGetter(AccessorDeclarationSyntax accessor)
        {
            if (!_isParentInterface)
                Builder.Append(JavaUtils.GetPropertyModifiersString(_modifiers)).Space();

            Builder.Append(JavaType).Space();
            using (Builder.Append(GetterName).ParameterList())
            {
                WriteAccessorParameters(false);
            }

            if (_isParentInterface)
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
                    if (accessor.Body == null)
                    {
                        Builder.EndOfStatement();
                    }
                    else
                    {
                        using (Builder.AppendLine().Block())
                        {
                            if (Context.Conversion.SkipBody)
                                Builder.Append(Item.Type.GetJavaDefaultReturnStatement(Context)).EndOfStatement();
                            else
                                Builder.Append(accessor.Body, Context, true).AppendLine();
                        }
                    }
                }
            }
        }

        protected virtual void WriteAccessorParameters(bool setter) { /* Do nothing */ }

        IReadOnlyList<SyntaxKind> getSetterModifiers(AccessorDeclarationSyntax accessor)
        {
            var ret = new List<SyntaxKind>();
            var accessorModifiers = accessor.GetCSharpModifiers();
            if (accessorModifiers.Count == 0)
            {
                return _modifiers;
            }
            else
            {
                ret.AddRange(_modifiers.Skip(1));
                ret.AddRange(accessorModifiers);
            }
            return ret;
        }

        public string UnderlyingFieldName
        {
            get { return "__" + (Context.Conversion.MethodsLowerCase ? PropertyName.ToJavaLowerCase() : PropertyName); }
        }

        public virtual string GetterName
        {
            get { return "get" + PropertyName; }
        }

        public virtual string SetterName
        {
            get { return "set" + PropertyName; }
        }

        public string JavaType
        {
            get { return Item.Type.GetJavaType(Context); }
        }

        public abstract string PropertyName { get; }
    }

    class JavaPropertyWriter : PropertyWriter<PropertyDeclarationSyntax>
    {
        public JavaPropertyWriter(PropertyDeclarationSyntax syntax, JavaCodeConversionContext context)
            : base(syntax, context) { }

        public override string PropertyName
        {
            get { return Item.Identifier.Text; }
        }
    }

    class JavaIndexerWriter : PropertyWriter<IndexerDeclarationSyntax>
    {
        public JavaIndexerWriter(IndexerDeclarationSyntax syntax, JavaCodeConversionContext context)
            : base(syntax, context) { }

        public override string PropertyName
        {
            get { return "Indexer"; }
        }

        public override string GetterName
        {
            get { return "get"; }
        }

        public override string SetterName
        {
            get { return "set"; }
        }

        protected override void WriteAccessorParameters(bool setter)
        {
            bool first = true;
            foreach (var parameter in Item.ParameterList.Parameters)
            {
                Builder.CommaSeparator(ref first).Append(parameter.Type!, Context)
                    .Space().Append(parameter.Identifier.Text);
            }

            if (setter)
                Builder.CommaSeparator();
        }
    }
}
