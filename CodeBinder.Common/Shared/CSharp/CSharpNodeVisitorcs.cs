// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT
using CodeBinder.Attributes;
using System.Linq;

namespace CodeBinder.Shared.CSharp;

/// <summary>
/// Inherit this class if you need a default CSharpCompilationContext
/// </summary>
public class CSharpNodeVisitor<TCompilationContext, TTypeContext, TLanguageConversion> : CSharpNodeVisitorBase<TCompilationContext, CSharpMemberTypeContext, CSharpLanguageConversion>
    where TCompilationContext : CSharpCompilationContext
    where TTypeContext : CSharpMemberTypeContext
    where TLanguageConversion : CSharpLanguageConversion
{
    Dictionary<string, List<CSharpTypeContext>> _types;
    Stack<CSharpMemberTypeContext> _parents;

    protected CSharpNodeVisitor(TCompilationContext context)
        : base(context)
    {
        _types = new Dictionary<string, List<CSharpTypeContext>>();
        _parents = new Stack<CSharpMemberTypeContext>();
    }

    public CSharpMemberTypeContext? CurrentParent
    {
        get
        {
            if (_parents.Count == 0)
                return null;

            return _parents.Peek();
        }
    }

    private void Unsupported(SyntaxNode node, string? message = null)
    {
        if (message == null)
            AddError("Unsupported node: " + node);
        else
            AddError("Unsupported node: " + node + ", " + message);
    }

    #region Supported types

    public override void VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        checkTypeDeclaration(node, out _);
        var typeCtx = Compilation.CreateContext(node);
        typeCtx.Init();
        addTypeContext(typeCtx);
        _parents.Push(typeCtx);
        Compilation.AddClass(typeCtx);
        DefaultVisit(node);
        _parents.Pop();
    }

    public override void VisitStructDeclaration(StructDeclarationSyntax node)
    {
        checkTypeDeclaration(node, out _);
        var typeCtx = Compilation.CreateContext(node);
        typeCtx.Init();
        addTypeContext(typeCtx);
        _parents.Push(typeCtx);
        Compilation.AddStruct(typeCtx);
        DefaultVisit(node);
        _parents.Pop();
    }

    public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
    {
        checkTypeDeclaration(node, out _);
        var typeCtx = Compilation.CreateContext(node);
        typeCtx.Init();
        addTypeContext(typeCtx);
        Compilation.AddInterface(typeCtx);
        DefaultVisit(node);
    }

    public override void VisitEnumDeclaration(EnumDeclarationSyntax node)
    {
        var typeCtx = Compilation.CreateContext(node);
        typeCtx.Init();
        Compilation.AddTypeContext(typeCtx, CurrentParent);
        Compilation.AddEnum(typeCtx);
        DefaultVisit(node);
    }

    public override void VisitDelegateDeclaration(DelegateDeclarationSyntax node)
    {
        var typeCtx = Compilation.CreateContext(node);
        typeCtx.Init();
        Compilation.AddDelegate(typeCtx);
        DefaultVisit(node);
    }

    public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
    {

        IMethodSymbol symbol;
        if (node.Identifier.Text == "FreeHandle" && node.Body != null)
        {
            symbol = node.GetDeclaredSymbol<IMethodSymbol>(this);
            if (symbol.OverriddenMethod?.ContainingType.GetFullName() == "CodeBinder.HandledObjectBase")
            {
                foreach (var statement in node.Body.Statements)
                {
                    var expressionSyntax = statement as ExpressionStatementSyntax;
                    if (expressionSyntax == null)
                    {
                        AddError("Unsupported FreeHandle body, it must be a call of static method");
                        continue;
                    }

                    var invocation = expressionSyntax.Expression as InvocationExpressionSyntax;
                    if (invocation == null)
                    {
                        AddError("Unsupported FreeHandle body, it must be a call of static method");
                        continue;
                    }

                    IMethodSymbol? methodSymbol;
                    if (!invocation.TryGetSymbol(this, out methodSymbol)
                        || !methodSymbol.IsStatic)
                    {
                        AddError("Unsupported FreeHandle body, it must be a call of static method");
                        continue;
                    }

                    Compilation.AddFinalizer(node);
                }
            }
        }

        base.VisitMethodDeclaration(node);
    }

    public override void Visit(SyntaxNode? node)
    {
        if (node == null)
            throw new ArgumentNullException(nameof(node));

        if (node.ShouldDiscard(Compilation))
            return;

        var kind = node.Kind();
        switch (kind)
        {
            // Yield statements
            case SyntaxKind.YieldBreakStatement:
            case SyntaxKind.YieldReturnStatement:
            {
                if (!Compilation.Conversion.SupportedPolicies.Contains(Policies.YieldReturn))
                    Unsupported(node);

                break;
            }
            // Type constraints
            case SyntaxKind.ConstructorConstraint:
            case SyntaxKind.StructConstraint:
            case SyntaxKind.ClassConstraint:
            // Declarations
            case SyntaxKind.EventDeclaration:
            case SyntaxKind.EventFieldDeclaration:
            case SyntaxKind.ConversionOperatorDeclaration:
            case SyntaxKind.OperatorDeclaration:
            // Statements
            case SyntaxKind.CheckedStatement:
            case SyntaxKind.UnsafeStatement:
            case SyntaxKind.LabeledStatement:
            case SyntaxKind.FixedStatement:
            case SyntaxKind.LocalFunctionStatement:
            case SyntaxKind.ForEachVariableStatement:
            // Goto statements
            case SyntaxKind.GotoStatement:
            case SyntaxKind.GotoCaseStatement:
            case SyntaxKind.GotoDefaultStatement:
            // Expressions
            case SyntaxKind.RefExpression:
            case SyntaxKind.DeclarationExpression:
            case SyntaxKind.ThrowExpression:
            case SyntaxKind.DefaultExpression:
            case SyntaxKind.AnonymousMethodExpression:
            case SyntaxKind.ParenthesizedLambdaExpression:
            case SyntaxKind.SimpleLambdaExpression:
            case SyntaxKind.RefValueExpression:
            case SyntaxKind.RefTypeExpression:
            case SyntaxKind.ImplicitArrayCreationExpression:
            case SyntaxKind.ElementBindingExpression:
            case SyntaxKind.ImplicitElementAccess:
            case SyntaxKind.MemberBindingExpression:
            case SyntaxKind.SizeOfExpression:
            case SyntaxKind.MakeRefExpression:
            case SyntaxKind.ImplicitStackAllocArrayCreationExpression:
            case SyntaxKind.InterpolatedStringExpression:
            case SyntaxKind.AwaitExpression:
            case SyntaxKind.QueryExpression:
            case SyntaxKind.StackAllocArrayCreationExpression:
            case SyntaxKind.AnonymousObjectCreationExpression:
            case SyntaxKind.TupleExpression:
            case SyntaxKind.IsPatternExpression:
            case SyntaxKind.CheckedExpression:
            case SyntaxKind.ConditionalAccessExpression:
            // Prefix unary expressions
            case SyntaxKind.AddressOfExpression:
            case SyntaxKind.PointerIndirectionExpression:
            // Binary expressions
            case SyntaxKind.CoalesceExpression:
            // Member access expressions
            case SyntaxKind.PointerMemberAccessExpression:
            // Literal expressions
            case SyntaxKind.ArgListExpression:
            case SyntaxKind.DefaultLiteralExpression:
            // Unsupported type expressions
            case SyntaxKind.TupleType:
            case SyntaxKind.PointerType:
            // Linq
            case SyntaxKind.FromClause:
            case SyntaxKind.WhereClause:
            case SyntaxKind.SelectClause:
            case SyntaxKind.GroupClause:
            case SyntaxKind.JoinIntoClause:
            case SyntaxKind.OrderByClause:
            case SyntaxKind.JoinClause:
            case SyntaxKind.LetClause:
            // Misc
            case SyntaxKind.CasePatternSwitchLabel:
            case SyntaxKind.CatchFilterClause:
            case SyntaxKind.ArrowExpressionClause:
            {
                Unsupported(node);
                break;
            }
        }

        base.Visit(node);
    }

    #endregion Supported types

    void addTypeContext(CSharpTypeContext type)
    {
        string fullName = type.Node.GetFullName(this);
        if (!_types.TryGetValue(fullName, out var types))
        {
            types = new List<CSharpTypeContext>();
            _types.Add(fullName, types);
        }

        // If the type is the main declaration, put it first, otherwise just put back
        if (type.Node.BaseList == null)
            types.Add(type);
        else
            types.Insert(0, type);
    }

    protected override void afterVisit()
    {
        var mainTypesMap = new Dictionary<ITypeSymbol, CSharpTypeContext>(SymbolEqualityComparer.Default);
        foreach (var types in _types.Values)
        {
            var main = types[0];
            var symbol = main.Node.GetDeclaredSymbol<ITypeSymbol>(this);
            Compilation.AddNamespace(symbol.GetContainingNamespace());
            for (int i = 0; i < types.Count; i++)
                main.AddPartialDeclaration(types[i]);

            mainTypesMap[symbol] = main;
        }

        foreach (var type in mainTypesMap.Values)
        {
            var symbol = type.Node.GetDeclaredSymbol<ITypeSymbol>(this);
            if (symbol.ContainingType == null)
                Compilation.AddTypeContext(type, null);
            else
                // We assume partial types are contained in partial types, see visitor
                Compilation.AddTypeContext(type, mainTypesMap[symbol.ContainingType]);
        }
    }

    #region Unsupported syntax

    public override void VisitForEachStatement(ForEachStatementSyntax node)
    {
        if (node.Expression.TryGetTypeSymbolRaw<IArrayTypeSymbol>(Compilation, out var arraySymbol))
            Unsupported(node, "Iteration of array");

        DefaultVisit(node);
    }

    public override void VisitAssignmentExpression(AssignmentExpressionSyntax node)
    {
        var leftSymbol = node.Left.GetSymbol(this);
        if (leftSymbol != null)
        {
            switch (leftSymbol.Kind)
            {
                case SymbolKind.Parameter:
                {
                    var parameter = (IParameterSymbol)leftSymbol;
                    if (parameter.Type.TypeKind == TypeKind.Struct
                            && parameter.RefKind != RefKind.None
                            && !parameter.Type.IsCLRPrimitiveType())
                        Unsupported(node, "Assignment of ref/out structured type");
                    break;
                }
                case SymbolKind.Property:
                {
                    if (!node.OperatorToken.IsKind(SyntaxKind.EqualsToken))
                        Unsupported(node, "Assigment with lhs property and non equals token");

                    break;
                }
            }
        }

        DefaultVisit(node);
    }

    public override void VisitArgumentList(ArgumentListSyntax node)
    {
        void assertParent(SyntaxNode parent, params SyntaxKind[] kinds)
        {
            var parentKind = parent.Kind();
            foreach (var kind in kinds)
            {
                if (parentKind == kind)
                    return;
            }

            Unsupported(node, "ref like keyword in unsupported context");
        }

        foreach (var arg in node.Arguments)
        {
            if (!(arg.RefKindKeyword.IsNone() || Compilation.Conversion.SupportedPolicies.Contains(Policies.PassByRef)))
            {
                var argSymbol = arg.Expression.GetSymbol(this);
                ITypeSymbol argType = null!;
                switch (argSymbol!.Kind)
                {
                    case SymbolKind.Local:
                        argType = (argSymbol as ILocalSymbol)!.Type;
                        break;
                    case SymbolKind.Parameter:
                        argType = (argSymbol as IParameterSymbol)!.Type;
                        break;
                    case SymbolKind.Field:
                        argType = (argSymbol as IFieldSymbol)!.Type;
                        break;
                    default:
                        Unsupported(node, "ref like keyword keyword in non local/parameter expression");
                        break;
                }

                var refKind = arg.RefKindKeyword.Kind();
                switch (refKind)
                {
                    case SyntaxKind.RefKeyword:
                    case SyntaxKind.OutKeyword:
                        break;
                    default:
                        Unsupported(node, "Unsupported ref like keyword");
                        break;
                }

                switch (argType.TypeKind)
                {
                    case TypeKind.Struct:
                        if (!argType.IsCLRPrimitiveType() && argType.GetFullName() != "CodeBinder.cbstring")
                        {
                            switch (refKind)
                            {
                                case SyntaxKind.RefKeyword:
                                    // Supported structured type pass by reference
                                    DefaultVisit(node);
                                    return;
                                case SyntaxKind.OutKeyword:
                                    Unsupported(node, "out keyword supported only for CLR primitive types or enum");
                                    break;
                                default:
                                    throw new Exception();
                            }
                        }

                        break;
                    case TypeKind.Enum:
                        break;
                    case TypeKind.Class:
                        if (argType.GetFullName() != "System.String")
                            goto default;

                        break;
                    default:
                        Unsupported(node, "Unsupported ref like keyword for non-struct/enum type");
                        break;
                }

                // Must be within an invocation
                assertParent(node.Parent!, SyntaxKind.InvocationExpression);

                StatementKind statementKind;
                if (node.Parent!.Parent!.IsStatement(out statementKind))
                {
                    switch (statementKind)
                    {
                        case StatementKind.Expression:
                        case StatementKind.Return:
                        {
                            // invocation contained in a block
                            assertParent(node.Parent!.Parent!.Parent!, SyntaxKind.Block, SyntaxKind.SwitchSection);
                            goto Exit;
                        }
                    }

                    Unsupported(node, "ref like keyword in unsupported context");
                }

                ExpressionKind expressionKind;
                if (node.Parent!.Parent!.IsExpression(out expressionKind))
                {
                    switch (expressionKind)
                    {
                        case ExpressionKind.Assignment:
                        {
                            // non-return invocation contained in a assignment expressio, contained in a block
                            assertParent(node.Parent!.Parent!.Parent!, SyntaxKind.ExpressionStatement);
                            assertParent(node.Parent!.Parent!.Parent!.Parent!, SyntaxKind.Block, SyntaxKind.SwitchSection);
                            goto Exit;
                        }
                    }

                    Unsupported(node, "ref like keyword in unsupported context");
                }

                if (node.Parent.Parent.IsKind(SyntaxKind.EqualsValueClause))
                {
                    // Local declaration and assignment with invocation
                    assertParent(node.Parent!.Parent!.Parent!.Parent!.Parent!, SyntaxKind.LocalDeclarationStatement);
                    assertParent(node.Parent!.Parent!.Parent!.Parent!.Parent!.Parent!, SyntaxKind.Block, SyntaxKind.SwitchSection);
                    goto Exit;
                }

                Unsupported(node, "ref like keyword in unsupported context");
                break;
            }
        }

    Exit:
        DefaultVisit(node);
    }

    public override void VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
    {
        if (node.Initializer != null)
            Unsupported(node, "Object initializer");

        DefaultVisit(node);
    }

    public override void VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
    {
        if (!node.OperatorToken.IsKind(SyntaxKind.DotToken))
            Unsupported(node, "Not dot member access");

        DefaultVisit(node);
    }

    public override void VisitIdentifierName(IdentifierNameSyntax node)
    {
        var symbol = node.GetSymbol(this);
        switch (symbol!.Kind)
        {
            case SymbolKind.DynamicType:
                Unsupported(node, "Dynamic type specifier");
                break;
        }

        DefaultVisit(node);
    }

    public override void VisitArgument(ArgumentSyntax node)
    {
        if (node.NameColon != null)
            Unsupported(node, "Argument with optional argument specification");

        DefaultVisit(node);
    }

    public override void VisitNullableType(NullableTypeSyntax node)
    {
        var typeSymbol = node.ElementType.GetTypeSymbol(this);
        string fullname = typeSymbol.GetFullName();
        switch(fullname)
        {
            // Types that are boxable
            case "System.Boolean":
            case "System.Byte":
            case "System.SByte":
            case "System.Int16":
            case "System.UInt16":
            case "System.Int32":
            case "System.UInt32":
            case "System.Int64":
            case "System.UInt64":
            case "System.Single":
            case "System.Double":
            case "System.IntPtr":
                break;
            default:
                if (!(typeSymbol.TypeKind == TypeKind.Enum || typeSymbol.TypeKind == TypeKind.Struct))
                    Unsupported(node, "Nullable types supported only on boxable types or structs");
                break;
        }

        DefaultVisit(node);
    }

    public override void VisitUsingStatement(UsingStatementSyntax node)
    {
        if (node.Expression != null)
            Unsupported(node, "Using statement with expression");

        DefaultVisit(node);
    }

    public override void VisitVariableDeclaration(VariableDeclarationSyntax node)
    {
        if (node.Variables.Count != 1)
            Unsupported(node, "Variable declaration with variable count not equals to 1");

        DefaultVisit(node);
    }

    public override void VisitAliasQualifiedName(AliasQualifiedNameSyntax node)
    {
        var current = node.Parent;
        while (current != null)
        {
            if (current.IsKind(SyntaxKind.Attribute))
            {
                // NOTE: If an ancestor is attribute, just ignore the node
                return;
            }

            current = current.Parent;
        }

        Unsupported(node, "Unsupported qualified name expression with parent " + node.Parent);
    }

    public override void VisitTypeParameter(TypeParameterSyntax node)
    {
        if (!node.VarianceKeyword.IsNone())
            Unsupported(node, "Type parameter with unsupported variance modifier");

        DefaultVisit(node);
    }

    public override void VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
    {
        if (node.Modifiers.Any((token) => !token.IsKind(SyntaxKind.ConstKeyword)))
            Unsupported(node, "Variable declaration with unsupported modifiers");

        DefaultVisit(node);
    }

    public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
    {
        if (node.AccessorList == null)
            Unsupported(node, "Unsupported property with no accessor definied: use \"get\" or \"set\"");

        DefaultVisit(node);
    }

    public override void VisitArrayType(ArrayTypeSyntax node)
    {
        if (node.RankSpecifiers.Count > 1)
            Unsupported(node, "Unsupported array with rank specifiers > 1");

        DefaultVisit(node);
    }

    public override void VisitArrayRankSpecifier(ArrayRankSpecifierSyntax node)
    {
        if (node.Rank != 1)
            Unsupported(node, "Unsupported array with rank != 1");

        DefaultVisit(node);
    }

    public override void VisitTypeOfExpression(TypeOfExpressionSyntax node)
    {
        var typeSymbol = node.Type.GetTypeSymbol(this);
        if (typeSymbol.TypeKind == TypeKind.TypeParameter)
            Unsupported(node, "Unsupported typeof expression with parameterized type");

        DefaultVisit(node);
    }

    private void checkTypeDeclaration(TypeDeclarationSyntax type, out bool isPartial)
    {
        isPartial = type.IsPartial();
        if (isPartial)
        {
            var parent = type.Parent!;
            var parentKind = parent.Kind();
            switch (parentKind)
            {
                case SyntaxKind.ClassDeclaration:
                case SyntaxKind.InterfaceDeclaration:
                case SyntaxKind.StructDeclaration:
                {
                    var parentType = (TypeDeclarationSyntax)parent;
                    if (!parentType.Modifiers.Any(SyntaxKind.PartialKeyword))
                        Unsupported(type, "Nested partial types must have partial parent");

                    break;
                }
            }
        }
    }

    #endregion // Unsupported syntax
}

/// <summary>
/// Inherit this class if you don't need a default CSharpCompilationContext
/// </summary>
public abstract class CSharpNodeVisitorBase<TCompilationContext, TTypeContext, TLanguageConversion> : CSharpSyntaxWalker, INodeVisitor, ICompilationContextProvider
    where TCompilationContext : CompilationContext<TTypeContext>
    where TTypeContext : TypeContext<TTypeContext>
    where TLanguageConversion : LanguageConversion
{
    List<string> _errors;
    Dictionary<string, List<IMethodSymbol>> _uniqueMethodNames;
    HashSet<ITypeSymbol> _typeWithRegularCostructor;
    bool _checkMethodOverloads;

    public CSharpNodeVisitorBase(TCompilationContext context)
    {
        Compilation = context;
        _errors = new List<string>();
        _checkMethodOverloads = (Compilation.Conversion?.OverloadFeatures is OverloadFeature value)
            ? value != OverloadFeature.FullSupport : false;
        if (_checkMethodOverloads)
        {
            _uniqueMethodNames = new Dictionary<string, List<IMethodSymbol>>();
            _typeWithRegularCostructor = new HashSet<ITypeSymbol>(SymbolEqualityComparer.Default);
        }
        else
        {
            // These shouldn't be used, flag them with null
            _uniqueMethodNames = null!;
            _typeWithRegularCostructor = null!;
        }
    }

    public void Visit(SyntaxTree context)
    {
        Visit(context.GetRoot());
    }

    protected void AddError(string error)
    {
        _errors.Add(error);
    }

    public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        if (!(node.IsPartialMethod(out var hasEmptyBody) && hasEmptyBody))
            addSymbolBinding(node.GetDeclaredSymbol<IMethodSymbol>(this));

        DefaultVisit(node);
    }

    public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
    {
        var symbol = node.GetDeclaredSymbol<IMethodSymbol>(this);
        if (_checkMethodOverloads && !symbol.IsStatic)
        {
            if (symbol.HasAttribute<OverloadBindingAttribute>())
            {
                if (node.Initializer != null && node.Initializer.ThisOrBaseKeyword.IsKind(SyntaxKind.BaseConstructorInitializer))
                    AddError("Unsupported constructor overload with base initializer");

                if (symbol.ContainingType.TypeKind != TypeKind.Struct
                    && node.Body != null && node.Body.Statements.Count != 0)
                {
                    AddError("Unsupported constructor overload with non null body");
                }
            }
            else
            {
                if (symbol.ContainingType.TypeKind == TypeKind.Struct)
                    AddError("Constructors in structs without [OverloadBinding] attributes are not allowed");
                else if (_typeWithRegularCostructor.Contains(symbol.ContainingType))
                    AddError("Overloaded constructors without [OverloadBinding] attributes are not allowed");
                else
                    _typeWithRegularCostructor.Add(symbol.ContainingType);
            }
        }

        addSymbolBinding(symbol);
        DefaultVisit(node);
    }

    public override void VisitDestructorDeclaration(DestructorDeclarationSyntax node)
    {
        if (!Compilation.Conversion.SupportedPolicies.Contains(Policies.InstanceFinalizers))
        {
            var symbol = node.GetDeclaredSymbol<IMethodSymbol>(this);
            if (!symbol.ContainingType.Implements<IObjectFinalizer>())
            {
                AddError("Unsupported destructor in a object not implementing IObjectFinalizer or in a"
                    + " language conversion without Policies.InstanceFinalizers");
            }
        }

        DefaultVisit(node);
    }

    // Construct a method binding to handle overloaded methods/constructors
    void addSymbolBinding(IMethodSymbol methodSymbol)
    {
        string? stem = null;
        var enableIfMissing = OverloadFeature.None;
        if (methodSymbol.TryGetAttribute<OverloadBindingAttribute>(out var overloadAttrib))
        {
            stem = overloadAttrib.GetConstructorArgument<string>(0);
            enableIfMissing = overloadAttrib.GetConstructorArgument<OverloadFeature>(1);
        }

        if (stem == null && methodSymbol.OverriddenMethod != null)
        {
            if (methodSymbol.OverriddenMethod.TryGetAttribute<OverloadBindingAttribute>(out overloadAttrib))
            {
                stem = overloadAttrib.GetConstructorArgument<string>(0);
                enableIfMissing = overloadAttrib.GetConstructorArgument<OverloadFeature>(0);
            }
        }

        string bindedName;
        if (stem == null || (Compilation.Conversion.OverloadFeatures?.HasFlag(enableIfMissing) ?? true))
        {
            bindedName = GetMethodBaseName(methodSymbol);
        }
        else
        {
            if (methodSymbol.MethodKind == MethodKind.Constructor)
            {
                bindedName = stem;
                handleMethodCasing(ref bindedName);
            }
            else
            {
                bindedName = GetMethodBaseName(methodSymbol) + stem;
            }
        }

        if (_checkMethodOverloads)
        {
            string qualifiedBindedName = $"{methodSymbol.ContainingType.GetFullName()}.{bindedName}";
            List<IMethodSymbol>? bindedMethods;
            if (_uniqueMethodNames.TryGetValue(qualifiedBindedName, out bindedMethods))
            {
                bool doParameterOverlaps = false;
                foreach (var bindendMethodSymbol in bindedMethods)
                {
                    doParameterOverlaps = methodSymbol.DoParameterCountOverlap(bindendMethodSymbol);
                    if (doParameterOverlaps)
                    {
                        AddError("Method " + methodSymbol.GetDebugName()
                            + " parameter count overlap with method " + bindendMethodSymbol.GetDebugName());
                        break;
                    }
                }

                if (!doParameterOverlaps)
                    bindedMethods.Add(methodSymbol);
            }
            else
            {
                bindedMethods = new List<IMethodSymbol> { methodSymbol };
                _uniqueMethodNames.Add(qualifiedBindedName, bindedMethods);
            }
        }

        Compilation.AddMethodBinding(methodSymbol, bindedName);
    }

    protected bool TryGetModuleName(TypeDeclarationSyntax type, [NotNullWhen(true)] out string? moduleName)
    {
        // To support partial calsses, iterate syntax attributes,
        // don't infer them from context
        foreach (var attributeList in type.AttributeLists)
        {
            foreach (var attribute in attributeList.GetAttributes(this))
            {
                if (attribute.IsAttribute<ModuleAttribute>())
                {
                    moduleName = attribute.GetConstructorArgument<string>(0);
                    return true;
                }
            }
        }

        // As a last resort ask for all the symbol attributes
        if (type.TryGetAttribute<ModuleAttribute>(this, out var attr))
        {
            moduleName = attr.GetConstructorArgument<string>(0);
            return true;
        }

        moduleName = null;
        return false;
    }

    protected virtual string GetMethodBaseName(IMethodSymbol symbol)
    {
        string baseName;
        if (symbol.ExplicitInterfaceImplementations.Length == 0)
        {
            baseName = symbol.Name;
        }
        else
        {
            // Get name of explicitly interface implemented method
            baseName = symbol.ExplicitInterfaceImplementations[0].Name;
        }

        handleMethodCasing(ref baseName);
        return baseName;
    }

    public TCompilationContext Compilation { get; private set; }

    protected virtual void afterVisit()
    {
        // Do nothing
    }

    void INodeVisitor.AfterVisit()
    {
        afterVisit();
    }

    CompilationContext ICompilationContextProvider.Compilation
    {
        get { return Compilation; }
    }

    public IReadOnlyList<string> Errors => _errors;


    void handleMethodCasing(ref string methodName)
    {
        switch (Compilation.Conversion.MethodCasing)
        {
            case MethodCasing.LowerCamelCase:
                methodName = methodNameToLowerCase(methodName);
                break;
        }
    }

    static string methodNameToLowerCase(string methodName)
    {
        if (char.IsLower(methodName, 0))
            return methodName;

        return char.ToLowerInvariant(methodName[0]) + methodName.Substring(1);
    }
}

class CSharpNodeVisitorImpl : CSharpNodeVisitor<CSharpCompilationContext, CSharpMemberTypeContext, CSharpLanguageConversion>
{
    public CSharpNodeVisitorImpl(CSharpCompilationContext context)
        : base(context)
    {
    }
}
