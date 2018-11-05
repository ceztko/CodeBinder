using CodeTranslator.Shared;
using CodeTranslator.Shared.CSharp;
using CodeTranslator.Util;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.Java
{
    abstract class PropertyWriter<TProperty> : SyntaxWriter<TProperty>
            where TProperty : BasePropertyDeclarationSyntax
    {
        List<string> _modifiers;
        bool _isAutoProperty;
        public bool IsInterface { get; private set; }

        protected PropertyWriter(TProperty syntax, bool isInterface, ICompilationContextProvider context)
            : base(syntax, context)
        {
            _modifiers = Syntax.GetCSharpModifierStrings();
            _isAutoProperty = true;
            IsInterface = isInterface;
            if (Syntax.AccessorList != null)
            {
                foreach (var accessor in Syntax.AccessorList.Accessors)
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
            WriteAccessors(Syntax.AccessorList);
        }

        private void WriteUnderlyingField()
        {
            if (!_isAutoProperty || IsInterface)
                return;

            Builder.Append("private").Space();
            Builder.Append(JavaType).Space();
            Builder.Append(UnderlyingFieldName).EndOfLine();
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
            if (IsInterface)
            {
                Builder.EndOfLine();
            }
            else
            {
                if (_isAutoProperty)
                {
                    using (Builder.Space().BeginBlock())
                    {
                        Builder.Append(UnderlyingFieldName).Space().Append("= value").EndOfLine();
                    }
                }
                else
                {
                    new BlockWriter(accessor.Body, this).Write(Builder.Space());
                }
            }
        }

        private void WriteGetter(AccessorDeclarationSyntax accessor)
        {
            Builder.Append(JavaExtensions.GetJavaPropertyModifiersString(_modifiers)).Space();
            Builder.Append(JavaType).Space();
            Builder.Append(GetterName).Append("()");
            if (IsInterface)
            {
                Builder.EndOfLine();

            }
            else
            {
                if (_isAutoProperty)
                {
                    using (Builder.Space().BeginBlock())
                    {
                        Builder.Append("return").Space().Append(UnderlyingFieldName).EndOfLine();
                    }
                }
                else
                {
                    new BlockWriter(accessor.Body, this).Write(Builder.Space());
                }
            }
        }

        List<string> getSetterModifiers(AccessorDeclarationSyntax accessor)
        {
            var ret = new List<string>();
            ret.AddRange(_modifiers.Skip(1));
            ret.AddRange(accessor.GetCSharpModifierStrings());
            return ret;
        }

        public string UnderlyingFieldName
        {
            get { return "__" + PropertyName.ToJavaCase() + "__"; }
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
            get { return Syntax.Type.GetJavaType(this); }
        }

        public abstract string PropertyName { get; }
    }

    class PropertyWriter : PropertyWriter<PropertyDeclarationSyntax>
    {
        public PropertyWriter(PropertyDeclarationSyntax syntax, bool isInterface, ICompilationContextProvider context)
            : base(syntax, isInterface, context) { }

        public override string PropertyName
        {
            get { return Syntax.Identifier.Text; }
        }
    }

    class IndexerWriter : PropertyWriter<IndexerDeclarationSyntax>
    {
        public IndexerWriter(IndexerDeclarationSyntax syntax, bool isInterface, ICompilationContextProvider context)
            : base(syntax, isInterface, context) { }

        public override string PropertyName
        {
            get { return "At"; }
        }
    }
}
