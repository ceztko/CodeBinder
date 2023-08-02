// SPDX-FileCopyrightText: (C) 2018 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

using CodeBinder.Attributes;
using Microsoft.CodeAnalysis;
using System.Linq;

namespace CodeBinder.Shared.CSharp;

/// <summary>
/// CSharp language specific validation context
/// </summary>
public class CSharpValidationContext<TLanguageConversion> : CSharpValidationContext
    where TLanguageConversion : CSharpLanguageConversion
{
    TLanguageConversion _Conversion;

    protected CSharpValidationContext(TLanguageConversion conversion)
    {
        _Conversion = conversion;
    }
    public override TLanguageConversion Conversion => _Conversion;
}

/// <summary>
/// CSharp language specific validation context
/// </summary>
/// <remarks>This class is for infrastructure only. It's bound to a full CSharpCompilationConversion</remarks>
public abstract class CSharpValidationContext : CSharpValidationContextBase
{
    internal CSharpValidationContext() { }

    public override abstract CSharpLanguageConversion Conversion { get; }
}

/// <summary>
/// CSharp language specific validation context
/// </summary>
public class CSharpValidationContextBase<TLanguageConversion> : CSharpValidationContextBase
    where TLanguageConversion : LanguageConversion
{
    TLanguageConversion _Conversion;

    protected CSharpValidationContextBase(TLanguageConversion conversion)
    {
        _Conversion = conversion;
    }
    public override TLanguageConversion Conversion => _Conversion;
}

/// <summary>
/// CSharp language specific validation context
/// </summary>
/// <remarks>This class is for infrastructure only. It's bound to a generic LanguageConversion</remarks>
public abstract class CSharpValidationContextBase : ValidationContext<CSharpNodeVisitor>
{
    HashSet<ITypeSymbol> _typeWithRegularCostructor = null!;
    Dictionary<string, List<IMethodSymbol>> _methods;

    internal CSharpValidationContextBase()
    {
        _methods = new Dictionary<string, List<IMethodSymbol>>();
        Init += CSharpValidationContextBase_Initialized;
    }

    private void CSharpValidationContextBase_Initialized(CSharpNodeVisitor visitor)
    {
        if (Conversion.CheckMethodOverloads)
            _typeWithRegularCostructor = new HashSet<ITypeSymbol>(SymbolEqualityComparer.Default);

        visitor.BeforeNodeVisit += Visitor_BeforeNodeVisit;

        // Declarations
        visitor.ClassDeclarationVisit += Visitor_ClassDeclarationVisit;
        visitor.StructDeclarationVisit += Visitor_StructDeclarationVisit;
        visitor.InterfaceDeclarationVisit += Visitor_InterfaceDeclarationVisit;
        visitor.MethodDeclarationVisit += Visitor_MethodDeclarationVisit;
        visitor.ConstructorDeclarationVisit += Visitor_ConstructorDeclarationVisit;
        visitor.DestructorDeclarationVisit += Visitor_DestructorDeclarationVisit;

        // Statements
        visitor.ForEachStatementVisit += Visitor_ForEachStatementVisit;
        visitor.UsingStatementVisit += Visitor_UsingStatementVisit;

        // Expressions
        visitor.AssignmentExpressionVisit += Visitor_AssignmentExpressionVisit;
        visitor.InvocationExpressionVisit += Visitor_InvocationExpressionVisit;
        visitor.ObjectCreationExpressionVisit += Visitor_ObjectCreationExpressionVisit;
        visitor.MemberAccessExpressionVisit += Visitor_MemberAccessExpressionVisit;
        visitor.IdentifierNameVisit += Visitor_IdentifierNameVisit;
        visitor.ArgumentVisit += Visitor_ArgumentVisit;
        visitor.VariableDeclarationVisit += Visitor_VariableDeclarationVisit;
        visitor.AliasQualifiedNameVisit += Visitor_AliasQualifiedNameVisit;
        visitor.TypeParameterVisit += Visitor_TypeParameterVisit;
        visitor.LocalDeclarationStatementVisit += Visitor_LocalDeclarationStatementVisit;
        visitor.PropertyDeclarationVisit += Visitor_PropertyDeclarationVisit;
        visitor.ArrayTypeVisit += Visitor_ArrayTypeVisit;
        visitor.ArrayRankSpecifierVisit += Visitor_ArrayRankSpecifierVisit;
        visitor.TypeOfExpressionVisit += Visitor_TypeOfExpressionVisit;
    }

    private void Visitor_BeforeNodeVisit(NodeVisitor visitor, SyntaxNode node, ref NodeVisitorToken token)
    {
        if (node.ShouldDiscard(this, Conversion))
        {
            token.Cancel();
            return;
        }

        var kind = node.Kind();
        switch (kind)
        {
            // Yield statements
            case SyntaxKind.YieldBreakStatement:
            case SyntaxKind.YieldReturnStatement:
            {
                if (!Conversion.SupportedPolicies.Contains(Policies.Iterators))
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
    }

    private void Visitor_ClassDeclarationVisit(CSharpNodeVisitor visitor, ClassDeclarationSyntax node)
    {
        checkTypeDeclaration(node);
    }

    private void Visitor_StructDeclarationVisit(CSharpNodeVisitor visitor, StructDeclarationSyntax node)
    {
        checkTypeDeclaration(node);
    }

    private void Visitor_InterfaceDeclarationVisit(CSharpNodeVisitor visitor, InterfaceDeclarationSyntax node)
    {
        checkTypeDeclaration(node);

        // TODO It's not supported in ObjC. Should be easy: create protocols with all methods of inherited protocols
        if (node.BaseList != null)
            Unsupported(node, $"ObjectiveC: Interface {node.Identifier.Text} can't inherit other interfaces");
    }

    private void Visitor_MethodDeclarationVisit(CSharpNodeVisitor visitor, MethodDeclarationSyntax node)
    {
        var symbol = node.GetDeclaredSymbol<IMethodSymbol>(this);
        if (Conversion.SupportedPolicies.Contains(Policies.PassByRef))
        {
            foreach (var arg in node.ParameterList.Parameters)
            {
                if (!arg.IsRefLike())
                    continue;

                if (!symbol.IsNative())
                    Unsupported(node, "ref like parameter is supported only in extern methods");
            }
        }

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
                        Unsupported(node, "Unsupported FreeHandle body, it must be a call of static method");
                        continue;
                    }

                    var invocation = expressionSyntax.Expression as InvocationExpressionSyntax;
                    if (invocation == null)
                    {
                        Unsupported(node, "Unsupported FreeHandle body, it must be a call of static method");
                        continue;
                    }

                    IMethodSymbol? methodSymbol;
                    if (!invocation.TryGetSymbol(this, out methodSymbol)
                        || !methodSymbol.IsStatic)
                    {
                        Unsupported(node, "Unsupported FreeHandle body, it must be a call of static method");
                        continue;
                    }
                }
            }
        }

        tryCheckOverloads(symbol, node);
    }

    private void Visitor_ConstructorDeclarationVisit(CSharpNodeVisitor visitor, ConstructorDeclarationSyntax node)
    {
        var symbol = node.GetDeclaredSymbol<IMethodSymbol>(this);
        if (Conversion.CheckMethodOverloads && !symbol.IsStatic)
        {
            if (symbol.HasAttribute<OverloadBindingAttribute>())
            {
                if (node.Initializer != null && node.Initializer.ThisOrBaseKeyword.IsKind(SyntaxKind.BaseConstructorInitializer))
                    Unsupported(node, "Unsupported constructor overload with base initializer");

                if (symbol.ContainingType.TypeKind != TypeKind.Struct
                    && node.Body != null && node.Body.Statements.Count != 0)
                {
                    Unsupported(node, "Unsupported constructor overload with non null body");
                }
            }
            else
            {
                if (symbol.ContainingType.TypeKind == TypeKind.Struct)
                    Unsupported(node, "Constructors in structs without [OverloadBinding] attributes are not allowed");
                else if (_typeWithRegularCostructor.Contains(symbol.ContainingType))
                    Unsupported(node, "Overloaded constructors without [OverloadBinding] attributes are not allowed");
                else
                    _typeWithRegularCostructor.Add(symbol.ContainingType);
            }
        }

        tryCheckOverloads(symbol, node);
    }

    private void Visitor_DestructorDeclarationVisit(CSharpNodeVisitor visitor, DestructorDeclarationSyntax node)
    {
        if (!Conversion.SupportedPolicies.Contains(Policies.InstanceFinalizers))
        {
            var symbol = node.GetDeclaredSymbol<IMethodSymbol>(this);
            if (!symbol.ContainingType.Implements<IObjectFinalizer>())
            {
                Unsupported(node, "Unsupported destructor in a object not implementing IObjectFinalizer or in a"
                    + " language conversion without Policies.InstanceFinalizers");
            }
        }
    }

    private void Visitor_ForEachStatementVisit(CSharpNodeVisitor visitor, ForEachStatementSyntax node)
    {
        if (node.Expression.TryGetTypeSymbolRaw<IArrayTypeSymbol>(this, out var _))
            Unsupported(node, "Iteration of array");
    }

    private void Visitor_UsingStatementVisit(CSharpNodeVisitor visitor, UsingStatementSyntax node)
    {
        if (node.Expression != null)
            Unsupported(node, "Using statement with expression");
    }

    private void Visitor_AssignmentExpressionVisit(CSharpNodeVisitor visitor, AssignmentExpressionSyntax node)
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
    }

    private void Visitor_InvocationExpressionVisit(CSharpNodeVisitor visitor, InvocationExpressionSyntax node)
    {
        if (Conversion.SupportedPolicies.Contains(Policies.PassByRef))
            return;

        BlockSyntax? block = null;
        foreach (var arg in node.ArgumentList.Arguments)
        {
            if (!arg.IsRefLike())
                continue;

            var argSymbol = arg.Expression.GetSymbol(this)!;
            ITypeSymbol argType;
            switch (argSymbol.Kind)
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
                    continue;
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
                {
                    if (!argType.IsCLRPrimitiveType() && argType.GetFullName() != "CodeBinder.cbstring")
                    {
                        switch (refKind)
                        {
                            case SyntaxKind.RefKeyword:
                                // Supported structured type pass by reference
                                break;
                            case SyntaxKind.OutKeyword:
                                Unsupported(node, "out keyword supported only for CLR primitive types or enum");
                                continue;
                            default:
                                throw new Exception();
                        }
                    }

                    break;
                }
                case TypeKind.Enum:
                {
                    break;
                }
                case TypeKind.Class:
                {
                    if (argType.GetFullName() != "System.String")
                        goto default;

                    break;
                }
                default:
                {
                    Unsupported(node, "Unsupported ref like keyword for non-struct/enum type");
                    continue;
                }
            }

            if (block == null)
            {
                var parent = node.Parent;
                while (true)
                {
                    if (parent == null)
                        throw new NotSupportedException("Unsupported invocation not in a block");

                    if (parent.IsKind(SyntaxKind.Block))
                    {
                        block = (BlockSyntax)parent;
                        break;
                    }

                    parent = parent.Parent;
                }
            }
        }
    }

    private void Visitor_ObjectCreationExpressionVisit(CSharpNodeVisitor visitor, ObjectCreationExpressionSyntax node)
    {
        if (node.Initializer != null)
            Unsupported(node, "Object initializer");
    }

    private void Visitor_MemberAccessExpressionVisit(CSharpNodeVisitor visitor, MemberAccessExpressionSyntax node)
    {
        if (!node.OperatorToken.IsKind(SyntaxKind.DotToken))
            Unsupported(node, "Not dot member access");
    }

    private void Visitor_IdentifierNameVisit(CSharpNodeVisitor visitor, IdentifierNameSyntax node)
    {
        var symbol = node.GetSymbol(this);
        switch (symbol!.Kind)
        {
            case SymbolKind.DynamicType:
                Unsupported(node, "Dynamic type specifier");
                break;
        }
    }

    private void Visitor_ArgumentVisit(CSharpNodeVisitor visitor, ArgumentSyntax node)
    {
        if (node.NameColon != null)
            Unsupported(node, "Argument with optional argument specification");
    }

    private void Visitor_VariableDeclarationVisit(CSharpNodeVisitor visitor, VariableDeclarationSyntax node)
    {
        if (node.Variables.Count != 1)
        {
            Unsupported(node, "Variable declaration with variable count not equals to 1");
        }
        else
        {
            var variable = node.Variables[0];
            var symbol = variable.GetDeclaredSymbol(this)!;
            if (symbol.Kind == SymbolKind.Field)
            {
                if (variable.Initializer != null)
                    Unsupported(node, "Field variable declaration with non null initializer");
            }
        }
    }

    private void Visitor_AliasQualifiedNameVisit(CSharpNodeVisitor visitor, AliasQualifiedNameSyntax node)
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

    private void Visitor_TypeParameterVisit(CSharpNodeVisitor visitor, TypeParameterSyntax node)
    {
        if (!node.VarianceKeyword.IsNone())
            Unsupported(node, "Type parameter with unsupported variance modifier");
    }

    private void Visitor_LocalDeclarationStatementVisit(CSharpNodeVisitor visitor, LocalDeclarationStatementSyntax node)
    {
        if (node.Modifiers.Any((token) => !token.IsKind(SyntaxKind.ConstKeyword)))
            Unsupported(node, "Variable declaration with unsupported modifiers");
    }

    private void Visitor_PropertyDeclarationVisit(CSharpNodeVisitor visitor, PropertyDeclarationSyntax node)
    {
        if (node.AccessorList == null)
            Unsupported(node, "Unsupported property with no accessor definied: use \"get\" or \"set\"");
    }

    private void Visitor_ArrayTypeVisit(CSharpNodeVisitor visitor, ArrayTypeSyntax node)
    {
        if (node.RankSpecifiers.Count > 1)
            Unsupported(node, "Unsupported array with rank specifiers > 1");
    }

    private void Visitor_ArrayRankSpecifierVisit(CSharpNodeVisitor visitor, ArrayRankSpecifierSyntax node)
    {
        if (node.Rank != 1)
            Unsupported(node, "Unsupported array with rank != 1");
    }

    private void Visitor_TypeOfExpressionVisit(CSharpNodeVisitor visitor, TypeOfExpressionSyntax node)
    {
        var typeSymbol = node.Type.GetTypeSymbolThrow(this);
        if (typeSymbol.TypeKind == TypeKind.TypeParameter)
            Unsupported(node, "Unsupported typeof expression with parameterized type");
    }

    private void checkTypeDeclaration(TypeDeclarationSyntax type)
    {
        if (type.IsPartial())
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

    private void tryCheckOverloads(IMethodSymbol method, SyntaxNode node)
    {
        if (!Conversion.CheckMethodOverloads || method.IsPartialDefinition)
            return;

        var bindedName = method.GetBindedName(Conversion, out bool _);
        string methodName = $"{method.ContainingType.GetFullName()}.{bindedName}";
        List<IMethodSymbol>? bindedMethods;
        if (_methods.TryGetValue(methodName, out bindedMethods))
        {
            foreach (var bindendMethodSymbol in bindedMethods)
            {
                if (method.DoParameterCountOverlap(bindendMethodSymbol))
                {
                    Unsupported(node, "Method " + method.GetDebugName()
                        + " parameter count overlap with method " + bindendMethodSymbol.GetDebugName());
                }
            }
        }
        else
        {
            bindedMethods = new List<IMethodSymbol> { method };
            _methods.Add(methodName, bindedMethods);
        }
    }
}

class CSharpValidationContextBaseImpl : CSharpValidationContextBase<LanguageConversion>
{
    public CSharpValidationContextBaseImpl(LanguageConversion conversion)
        : base(conversion) { }
}

class CSharpValidationContextImpl : CSharpValidationContext<CSharpLanguageConversion>
{
    public CSharpValidationContextImpl(CSharpLanguageConversion conversion)
        : base(conversion) { }
}
