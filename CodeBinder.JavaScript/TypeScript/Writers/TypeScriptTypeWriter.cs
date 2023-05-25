// SPDX-FileCopyrightText: (C) 2023 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.JavaScript.TypeScript;

[DebuggerDisplay("TypeName = {TypeName}")]
abstract class TypeScriptBaseTypeWriter<TTypeContext> : CodeWriter<TTypeContext>
    where TTypeContext : CSharpBaseTypeContext, ITypeContext<TypeScriptCompilationContext>
{
    protected TypeScriptBaseTypeWriter(TTypeContext context)
        : base(context) { }

    protected override void Write()
    {
        if (Item.Node.HasAccessibility(Accessibility.Public, Item))
            Builder.Append("export").Space();

        if (WriteAbstractModifier)
            Builder.Append("abstract").Space();

        Builder.Append(TypeDeclaration).Space();
        Builder.Append(TypeName);
        if (Arity > 0)
        {
            Builder.Space();
            WriteTypeParameters();
        }

        Builder.Space();
        if (Item.Node.BaseList == null)
        {
            if (ClassType)
                Builder.Append("extends ObjectTS");
        }
        else
            WriteBaseTypes(Item.Node.BaseList);

        Builder.AppendLine();
        using (Builder.Block())
        {
            WriteMembers();
        }
    }

    protected virtual void WriteTypeParameters() { }

    protected abstract void WriteMembers();

    private void WriteBaseTypes(BaseListSyntax baseList)
    {
        Debug.Assert(baseList.Types.Count != 0);
        bool first = true;
        bool firstInterface = true;
        bool isInterface = false;
        bool hasExtend = false;
        foreach (var type in baseList.Types)
        {
            if (first)
                first = false;
            else if (isInterface)
                Builder.CommaSeparator();
            else
                Builder.Space();

            string javaTypeName = type.Type.GetTypeScriptType(Compilation, out isInterface);
            if (isInterface)
            {
                if (firstInterface)
                {
                    if (!hasExtend)
                        Builder.Append("extends ObjectTS").Space();

                    Builder.Append("implements").Space();
                    firstInterface = false;
                }
            }
            else
            {
                Builder.Append("extends").Space();
                hasExtend = true;
            }
            Builder.Append(javaTypeName);
        }
    }

    protected void WriteMembers(IEnumerable<MemberDeclarationSyntax> members, PartialDeclarationsTree partialDeclarations)
    {
        bool first = true;
        foreach (var member in members)
        {
            if (member.ShouldDiscard(Compilation))
                continue;

            var accessibility = member.GetAccessibility(Compilation);
            if (accessibility.IsInternal())
                Builder.AppendLine("/** @internal */");

            foreach (var writer in member.GetWriters(partialDeclarations, Compilation))
                Builder.AppendLine(ref first).Append(writer);
        }
    }

    public TypeScriptCompilationContext Compilation => (Item as ITypeContext<TypeScriptCompilationContext>).Compilation;

    public virtual bool WriteAbstractModifier => false;

    public virtual bool ClassType => false;

    public virtual int Arity
    {
        get { return 0; }
    }

    public virtual string TypeName
    {
        get { return Item.Node.GetName(); }
    }

    public abstract string TypeDeclaration { get; }
}

abstract class TypeScriptTypeWriter<TTypeContext> : TypeScriptBaseTypeWriter<TTypeContext>
    where TTypeContext : CSharpTypeContext, ITypeContext<TypeScriptCompilationContext>
{
    PartialDeclarationsTree _partialDeclarations;

    protected TypeScriptTypeWriter(TTypeContext ctx, PartialDeclarationsTree partialDeclarations)
        : base(ctx)
    {
        _partialDeclarations = partialDeclarations;
    }

    protected override void WriteMembers()
    {
        if (_partialDeclarations.RootPartialDeclarations.Count == 0)
            WriteMembers(Item.Node.Members, _partialDeclarations);
        else
            WriteMembers(getPartialDeclarationMembers(), _partialDeclarations);
    }

    IEnumerable<MemberDeclarationSyntax> getPartialDeclarationMembers()
    {
        foreach (var declaration in _partialDeclarations.RootPartialDeclarations)
        {
            foreach (var member in declaration.Members)
            {
                switch (member.Kind())
                {
                    case SyntaxKind.InterfaceDeclaration:
                    case SyntaxKind.ClassDeclaration:
                    case SyntaxKind.StructDeclaration:
                    {
                        if (_partialDeclarations.ChildrenPartialDeclarations.ContainsKey((TypeDeclarationSyntax)member))
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
}
