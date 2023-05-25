// SPDX-FileCopyrightText: (C) 2023 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

using CodeBinder.Attributes;
using Microsoft.CodeAnalysis;

namespace CodeBinder.JavaScript.TypeScript;

abstract class MethodWriter<TMethod> : CodeWriter<TMethod, TypeScriptCompilationContext>
    where TMethod : BaseMethodDeclarationSyntax
{
    protected MethodWriter(TMethod method, TypeScriptCompilationContext context)
        : base(method, context) { }

    protected override void Write()
    {
        if (!Item.Parent.IsKind(SyntaxKind.InterfaceDeclaration))
            WriteModifiers();

        Builder.Append(MethodName);
        if (Arity != 0)
            WriteTypeParameters();

        WriteParameters();
        WriteReturnType();
        writeMethodBody();
    }

    protected virtual void WriteParameters()
    {
        int parameterCount = ParameterCount;
        if (parameterCount == 0)
        {
            Builder.EmptyParameterList();
        }
        else if (parameterCount == 1)
        {
            using (Builder.ParameterList())
            {
                writeParameters(Item.ParameterList, parameterCount);
            }
        }
        else
        {
            using (Builder.ParameterList(true))
            {
                writeParameters(Item.ParameterList, parameterCount);
                Builder.AppendLine();
            }
        }
    }

    protected virtual void WriteModifiers()
    {
        var modifiers = Item.GetModifiersString(Context);
        if (!modifiers.IsNullOrEmpty())
            Builder.Append(modifiers).Space();
    }

    protected void WriteType(TypeSyntax type, TypeScriptTypeFlags flags)
    {
        Builder.Append(type.GetTypeScriptType(flags, Context));
    }

    void writeMethodBody()
    {
        if (Item.Body == null)
        {
            Builder.EndOfStatement();
        }
        else
        {
            using (Builder.AppendLine().Block())
            {
                WriteMethodBodyPrefixInternal();
                if (!Context.Conversion.SkipBody)
                    Builder.Append(Item.Body, Context, true).AppendLine();
                WriteMethodBodyPostfixInternal();
            }
        }
    }

    void writeParameters(ParameterListSyntax list, int parameterCount)
    {
        bool first = true;
        for (int i = 0; i < parameterCount; i++)
        {
            var parameter = list.Parameters[i];
            Builder.CommaAppendLine(ref first);
            writeParameter(parameter);
        }
    }

    void writeParameter(ParameterSyntax parameter)
    {
        var flags = TypeScriptTypeFlags.None;
        bool isRef = parameter.IsRef() | parameter.IsOut();
        if (isRef)
            flags |= TypeScriptTypeFlags.IsByRef;

        Builder.Append(parameter.Identifier.Text);
        if (parameter.Default != null)
            Builder.QuestionMark();

        Builder.Colon().Space();
        WriteType(parameter.Type!, flags);
    }

    protected virtual void WriteTypeParameters() { /* Do nothing */ }

    protected virtual void WriteMethodBodyPrefixInternal() { /* Do nothing */ }

    protected virtual void WriteMethodBodyPostfixInternal() { /* Do nothing */ }

    protected virtual void WriteReturnType() { /* Do nothing */ }

    public virtual int Arity
    {
        get { return 0; }
    }

    public virtual int ParameterCount
    {
        get { return Item.ParameterList.Parameters.Count; }
    }

    public abstract string MethodName { get; }
}

class MethodWriter : MethodWriter<MethodDeclarationSyntax>
{
    public MethodWriter(MethodDeclarationSyntax method, TypeScriptCompilationContext context)
        : base(method, context)
    {
    }

    protected override void WriteModifiers()
    {
        if (IsParentInterface)
            return;

        base.WriteModifiers();
    }

    protected override void WriteTypeParameters()
    {
        Builder.Append(Item.GetTypeParameters(Context), Context);
    }

    protected override void WriteReturnType()
    {
        Builder.Colon().Space();
        WriteType(Item.ReturnType, TypeScriptTypeFlags.None);
    }

    protected override void WriteMethodBodyPrefixInternal()
    {
        if (Context.Conversion.SkipBody)
            Builder.Append(Item.ReturnType.GetTypeScriptDefaultReturnStatement(Context)).EndOfStatement();
    }

    public bool IsParentInterface
    {
        get { return Item.Parent!.IsKind(SyntaxKind.InterfaceDeclaration); }
    }

    public override string MethodName
    {
        get
        {
            var methodSymbol = Item.GetDeclaredSymbol<IMethodSymbol>(Context);
            return methodSymbol.GetTypeScriptName(Context);
        }
    }

    public override int Arity
    {
        get { return Item.Arity; }
    }
}

class ConstructorWriter : MethodWriter<ConstructorDeclarationSyntax>
{
    bool _isStatic;
    string? _wrapperMethodName;
    bool _isStructType;

    public ConstructorWriter(ConstructorDeclarationSyntax method, TypeScriptCompilationContext context)
        : base(method, context)
    {
        var methodSymbol = Item.GetDeclaredSymbol<IMethodSymbol>(Context);
        _isStatic = methodSymbol.IsStatic;
        _isStructType = methodSymbol.ContainingType.TypeKind == TypeKind.Struct;
        if (methodSymbol.HasAttribute<OverloadBindingAttribute>())
            _wrapperMethodName = methodSymbol.GetTypeScriptName(Context);
    }

    protected override void WriteModifiers()
    {
        if (!_isStatic)
        {
            if (_wrapperMethodName != null)
                Builder.Append("static").Space();

            base.WriteModifiers();
        }
    }

    protected override void WriteParameters()
    {
        if (!_isStatic)
            base.WriteParameters();
    }

    protected override void WriteMethodBodyPrefixInternal()
    {
        if (_isStructType)
        {
            var symbol = Item.GetDeclaredSymbol<IMethodSymbol>(Context);
            Builder.Append("let ret = new").Space().Append(symbol.ContainingType.Name).EmptyParameterList().EndOfStatement();
        }
        else
        {
            if (Item.Initializer == null)
            {
                if (!_isStatic)
                {
                    // We assume all our objects will need at least a call
                    // to the parameter less base constructor
                    Builder.Append("super()").EndOfStatement();
                }
            }
            else
            {

                Builder.Append(Item.Initializer, _wrapperMethodName != null, Context).EndOfStatement();
            }
        }
    }

    protected override void WriteMethodBodyPostfixInternal()
    {
        //// TODO: Finish method wrapper
        if (_isStructType)
        {
            Builder.Append("return ret").EndOfStatement();
        }
    }

    public override string MethodName
    {
        get
        {
            if (_isStatic)
            {
                return "static";
            }
            else
            {
                if (_wrapperMethodName != null)
                    return _wrapperMethodName;

                return "constructor";
            }
        }
    }
}

class DestructorWriter : MethodWriter<DestructorDeclarationSyntax>
{
    public DestructorWriter(DestructorDeclarationSyntax method, TypeScriptCompilationContext context)
        : base(method, context) { }

    protected override void WriteMethodBodyPostfixInternal()
    {
        Builder.Append("super.finalize()").EndOfStatement();
    }

    protected override void WriteModifiers()
    {
        Builder.Append("protected").Space();
    }

    protected override void WriteReturnType()
    {
        Builder.Colon().Space().Append("void").Space();
    }

    public override string MethodName
    {
        get { return "finalize"; }
    }
}
