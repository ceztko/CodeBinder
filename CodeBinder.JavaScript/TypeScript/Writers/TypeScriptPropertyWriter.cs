// SPDX-FileCopyrightText: (C) 2023 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.JavaScript.TypeScript;

abstract class PropertyWriter<TProperty> : CodeWriter<TProperty, TypeScriptCompilationContext>
        where TProperty : BasePropertyDeclarationSyntax
{
    bool _isAutoProperty;
    bool _isParentInterface;

    protected PropertyWriter(TProperty syntax, TypeScriptCompilationContext context)
        : base(syntax, context)
    {
        if (Item.GetDeclaredSymbol<IPropertySymbol>(Context).IsAbstract)
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

        _isParentInterface = Item.Parent!.IsKind(SyntaxKind.InterfaceDeclaration);
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

        Builder.Append("private").Space().Append(UnderlyingFieldName).Colon().Space().Append(TypeScriptType)
            .Space().Append("=").Space().Append(Item.Type.GetTypeScriptDefaultLiteral(Context)!).EndOfStatement();
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
            string typeScriptModifiers;
            if ((typeScriptModifiers = accessor.GetModifiersString(Context)).Length != 0)
                Builder.Append(typeScriptModifiers).Space();
        }

        if (UseTypeScriptPropertySyntax)
            Builder.Append("set").Space();

        using (Builder.Append(SetterName).ParameterList())
        {
            WriteAccessorParameters(true);
            Builder.Append("value").Colon().Space().Append(TypeScriptType);
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
                    Builder.Append("this").Dot().Append(UnderlyingFieldName).Space().Append("= value").EndOfStatement();
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
        {
            var modifiers = accessor.GetModifiersString(Context);
            if (!modifiers.IsNullOrEmpty())
                Builder.Append(modifiers).Space();
        }

        if (UseTypeScriptPropertySyntax)
            Builder.Append("get").Space();

        using (Builder.Append(GetterName).ParameterList())
        {
            WriteAccessorParameters(false);
        }
        Builder.Colon().Space().Append(TypeScriptType);

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
                    Builder.Append("return").Space().Append("this").Dot().Append(UnderlyingFieldName).EndOfStatement();
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
                            Builder.Append(Item.Type.GetTypeScriptDefaultReturnStatement(Context)).EndOfStatement();
                        else
                            Builder.Append(accessor.Body, Context, true).AppendLine();
                    }
                }
            }
        }
    }

    protected virtual void WriteAccessorParameters(bool setter) { /* Do nothing */ }

    public string UnderlyingFieldName
    {
        get { return "__" + (Context.Conversion.MethodCasing == MethodCasing.LowerCamelCase ? PropertyName.ToTypeScriptLowerCase() : PropertyName); }
    }

    public virtual bool UseTypeScriptPropertySyntax => true;

    public virtual string GetterName
    {
        get { return PropertyName.ToTypeScriptLowerCase(); }
    }

    public virtual string SetterName
    {
        get { return PropertyName.ToTypeScriptLowerCase(); }
    }

    public string TypeScriptType
    {
        get { return Item.Type.GetTypeScriptType(Context); }
    }

    public abstract string PropertyName { get; }
}

class TypeScriptPropertyWriter : PropertyWriter<PropertyDeclarationSyntax>
{
    public TypeScriptPropertyWriter(PropertyDeclarationSyntax syntax, TypeScriptCompilationContext context)
        : base(syntax, context) { }

    public override string PropertyName
    {
        get { return Item.Identifier.Text; }
    }
}

class JavaIndexerWriter : PropertyWriter<IndexerDeclarationSyntax>
{
    public JavaIndexerWriter(IndexerDeclarationSyntax syntax, TypeScriptCompilationContext context)
        : base(syntax, context) { }

    public override string PropertyName
    {
        get { return "Indexer"; }
    }

    public override string GetterName
    {
        get { return "getAt"; }
    }

    public override string SetterName
    {
        get { return "setAt"; }
    }

    public override bool UseTypeScriptPropertySyntax => false;

    protected override void WriteAccessorParameters(bool setter)
    {
        bool first = true;
        foreach (var parameter in Item.ParameterList.Parameters)
        {
            Builder.CommaSeparator(ref first).Append(parameter.Identifier.Text)
                .Colon().Space().Append(parameter.Type!, Context);
        }

        if (setter)
            Builder.CommaSeparator();
    }
}
