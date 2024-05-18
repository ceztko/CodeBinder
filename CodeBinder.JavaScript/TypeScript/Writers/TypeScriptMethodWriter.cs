// SPDX-FileCopyrightText: (C) 2023 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

using CodeBinder.Attributes;

namespace CodeBinder.JavaScript.TypeScript;

abstract class MethodWriter<TMethod> : CodeWriter<TMethod, TypeScriptCompilationContext>
    where TMethod : BaseMethodDeclarationSyntax
{
    public IMethodSymbol Symbol { get; private set; }

    protected MethodWriter(TMethod method, TypeScriptCompilationContext context)
        : base(method, context)
    {
        Symbol = method.GetDeclaredSymbol<IMethodSymbol>(context);
    }

    protected override void Write()
    {
        if (!Item.Parent.IsKind(SyntaxKind.InterfaceDeclaration))
            WriteModifiers();

        var namePrefix = MethodNamePrefix;
        if (namePrefix != string.Empty)
            Builder.Append(namePrefix);

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
                Builder.Append(Item.ParameterList.Parameters, parameterCount, Context);
            }
        }
        else
        {
            using (Builder.ParameterList(true))
            {
                Builder.Append(Item.ParameterList.Parameters, parameterCount, Context);
                Builder.AppendLine();
            }
        }
    }

    protected void WriteParameterDefaults()
    {
        foreach (var param in Item.ParameterList.Parameters)
        {
            if (param.Default == null)
                continue;

            Builder.Append(param.Identifier.Text).Append(" = ").Append(param.Identifier.Text).Append(" ?? ").Append(param.Default.Value, Context).EndOfStatement();
        }
    }

    protected virtual void WriteModifiers()
    {
        if (Symbol.ExplicitInterfaceImplementations.Length != 0)
        {
            // We assume interface implementations to be
            // always public and not needing other modifiers
            return;
        }

        var modifiers = Item.GetModifiersString(Context);
        if (!modifiers.IsNullOrEmpty())
            Builder.Append(modifiers).Space();
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
                if (!Context.Conversion.SkipBody && WriteMethodBody)
                    Builder.Append(Item.Body, Context, true).AppendLine();
                WriteMethodBodyPostfixInternal();
            }
        }
    }

    protected virtual void WriteTypeParameters() { /* Do nothing */ }

    protected virtual void WriteMethodBodyPrefixInternal() { /* Do nothing */ }

    protected virtual void WriteMethodBodyPostfixInternal() { /* Do nothing */ }

    protected virtual void WriteReturnType() { /* Do nothing */ }

    public virtual string MethodNamePrefix => string.Empty;

    public virtual bool WriteMethodBody => true;

    public virtual int Arity => 0;

    public virtual int ParameterCount => Item.ParameterList.Parameters.Count;

    public abstract string MethodName { get; }
}

class MethodWriter : MethodWriter<MethodDeclarationSyntax>
{
    bool _isEnumerator;
    bool _isGenerator;

    public MethodWriter(MethodDeclarationSyntax method, TypeScriptCompilationContext context)
        : base(method, context)
    {
        if (Symbol.IsGetEnumerator())
        {
            _isEnumerator = true;
            _isGenerator = isGenerator();
        }
        else
        {
            if (Symbol.ReturnType.OriginalDefinition.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T
                || Symbol.ReturnType.OriginalDefinition.SpecialType == SpecialType.System_Collections_Generic_IEnumerator_T)
            {
                _isGenerator = isGenerator();
            }
        }
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
        Builder.Append(Item.ReturnType.GetTypeScriptType(TypeScriptTypeFlags.None, Context));
    }

    protected override void WriteMethodBodyPrefixInternal()
    {
        WriteParameterDefaults();
        if (Context.Conversion.SkipBody)
            Builder.Append(Item.ReturnType.GetTypeScriptDefaultReturnStatement(Context)).EndOfStatement();
    }

    public bool IsParentInterface
    {
        get { return Item.Parent!.IsKind(SyntaxKind.InterfaceDeclaration); }
    }

    public override string MethodNamePrefix
    {
        get
        {
            // In TypeScript, only generator functions can use yield
            if (_isGenerator)
                return "*";

            return string.Empty;
        }
    }

    public override string MethodName
    {
        get
        {
            if (_isEnumerator)
                return "[Symbol.iterator]";

            return Symbol.GetTypeScriptName(Context);
        }
    }

    public override int Arity
    {
        get { return Item.Arity; }
    }

    bool isGenerator()
    {
        var isGeneratorWalker = new IsGeneratorWalker();
        isGeneratorWalker.Visit(Item);
        return isGeneratorWalker.IsGenerator;
    }

    class IsGeneratorWalker : CSharpSyntaxWalker
    {
        public bool IsGenerator { get; private set; }

        public override void VisitYieldStatement(YieldStatementSyntax node)
        {
            IsGenerator = true;
        }
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
        _isStatic = Symbol.IsStatic;
        _isStructType = Symbol.ContainingType.TypeKind == TypeKind.Struct;
        if (Symbol.HasAttribute<OverloadBindingAttribute>())
            _wrapperMethodName = Symbol.GetTypeScriptName(Context);
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
        WriteParameterDefaults();

        if (_isStructType)
        {
            Builder.Append("let ret = new").Space().Append(Symbol.ContainingType.Name).EmptyParameterList().EndOfStatement();
            Builder.Append("ret").Dot().Append($"__{_wrapperMethodName}");
            bool first = true;
            using (Builder.ParameterList())
            {
                foreach (var param in Item.ParameterList.Parameters)
                    Builder.CommaSeparator(ref first).Append(param.Identifier.Text);
            }
            Builder.EndOfStatement();
            Builder.Append("return ret").EndOfStatement();
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

    public override bool WriteMethodBody => !_isStructType;

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

class StructConstructorWrapperWriter : MethodWriter<ConstructorDeclarationSyntax>
{
    string _wrapperMethodName;

    public StructConstructorWrapperWriter(ConstructorDeclarationSyntax method, TypeScriptCompilationContext context)
        : base(method, context)
    {
        _wrapperMethodName = $"__{Symbol.GetTypeScriptName(Context)}";
    }

    protected override void WriteModifiers()
    {
        Builder.Append("private").Space();
    }

    public override string MethodName => _wrapperMethodName;
}

class DestructorWriter : MethodWriter<DestructorDeclarationSyntax>
{
    public DestructorWriter(DestructorDeclarationSyntax method, TypeScriptCompilationContext context)
        : base(method, context)
    {
        // We support destructor syntax only for finalizers
        var symbol = method.GetDeclaredSymbol<IMethodSymbol>(context);
        if (!symbol.ContainingType.Implements<IObjectFinalizer>())
            throw new NotSupportedException();
    }

    protected override void WriteModifiers()
    {
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
