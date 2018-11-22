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
        bool _isParentInterface;

        protected PropertyWriter(TProperty syntax, ICompilationContextProvider context)
            : base(syntax, context)
        {
            _modifiers = Context.GetCSharpModifiers().ToArray();

            if (_modifiers.Contains(SyntaxKind.AbstractKeyword))
            {
                _isAutoProperty = false;
            }
            else
            {
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

            _isParentInterface = Context.Parent.Kind() == SyntaxKind.InterfaceDeclaration;
        }

        protected override void Write()
        {
            WriteUnderlyingField();
            WriteAccessors(Context.AccessorList);
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
                Builder.Append(JavaExtensions.GetJavaPropertyModifiersString(modifiers)).Space();
            }
            Builder.Append("void").Space();
            Builder.Append(SetterName).Append("(").Append(JavaType).Space().Append("value").Append(")");
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
                            if (!CSToJavaConversion.SkipBody)
                                Builder.Space().Append(accessor.Body, this, true);
                        }
                    }
                }
            }
        }

        private void WriteGetter(AccessorDeclarationSyntax accessor)
        {
            if (!_isParentInterface)
                Builder.Append(JavaExtensions.GetJavaPropertyModifiersString(_modifiers)).Space();

            Builder.Append(JavaType).Space();
            Builder.Append(GetterName).EmptyParameterList();
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
                            if (CSToJavaConversion.SkipBody)
                                Builder.Append(Context.Type.GetJavaDefaultReturnStatement(this)).EndOfStatement();
                            else
                                Builder.Append(accessor.Body, this, true);
                        }
                    }
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

        public string UnderlyingFieldName
        {
            get { return "__" + PropertyName.ToJavaCase(); }
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
    }
}
