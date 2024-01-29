// SPDX-FileCopyrightText: (C) 2023 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.JavaScript.TypeScript;

static partial class TypeScriptBuilderExtension
{
    public static CodeBuilder Append(this CodeBuilder builder, ArrayCreationExpressionSyntax syntax, TypeScriptCompilationContext context)
    {
        builder.Append("new").Space().Append(syntax.Type, context);
        if (syntax.Initializer != null)
            builder.Append(syntax.Initializer, context);

        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, OmittedArraySizeExpressionSyntax syntax, TypeScriptCompilationContext context)
    {
        _ = syntax;
        _ = context;
        // Do nothing
        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, AssignmentExpressionSyntax syntax, TypeScriptCompilationContext context)
    {
        var symbol = syntax.Left.GetSymbol(context);
        // Symbol can be null https://github.com/dotnet/roslyn/issues/31471
        if (symbol?.Kind == SymbolKind.Property)
        {
            var operatorKind = syntax.OperatorToken.Kind();
            switch (operatorKind)
            {
                case SyntaxKind.EqualsToken:
                {
                    if (syntax.Left.Kind() == SyntaxKind.ElementAccessExpression)
                    {
                        // Determine if the LHS of the assignment is an indexer set operation
                        var property = (IPropertySymbol)symbol;
                        if (!property.IsIndexer)
                            break;

                        var elementAccess = (ElementAccessExpressionSyntax)syntax.Left;
                        builder.Append(elementAccess.Expression, context).Dot()
                            .Append(elementAccess, property, context)
                                .Parenthesized().Append(elementAccess.ArgumentList, false, context).CommaSeparator().Append(syntax.Right, context); ;
                        return builder;
                    }

                    break;
                }
                default:
                    throw new Exception();
            }
        }

        builder.Append(syntax.Left, context).Space().Append(syntax.GetJavaScriptOperator()).Space().Append(syntax.Right, context);
        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, BinaryExpressionSyntax syntax, TypeScriptCompilationContext context)
    {
        var kind = syntax.Kind(); 
        switch (kind)
        {
            case SyntaxKind.AsExpression:
            {
                var typeSymbol = syntax.Right.GetSymbol<ITypeSymbol>(context);
                switch (typeSymbol.TypeKind)
                {
                    case TypeKind.Enum:
                    {
                        // Enums don't support using normal cast
                        builder.Append(syntax.Left, context);
                        break;
                    }
                    case TypeKind.TypeParameter:
                    {
                        // In case of type parameter, ignore actual instance check
                        builder.Append(syntax.Left, context).Space().Append("as").Space().Append(syntax.Right, context);
                        break;
                    }
                    default:
                    {
                        builder.Append("BinderUtils.as<").Append(syntax.Right, context).Append(">").Parenthesized().Append(syntax.Left, context).CommaSeparator().Append(syntax.Right, context);
                        break;
                    }    
                }

                return builder;
            }
            case SyntaxKind.DivideExpression:
            {
                var typeSymbol = syntax.GetTypeSymbolThrow(context);
                switch (typeSymbol.SpecialType)
                {
                    // Handle integral division
                    // https://stackoverflow.com/a/22307150/213871
                    case SpecialType.System_Byte:
                    case SpecialType.System_Int16:
                    case SpecialType.System_UInt16:
                    case SpecialType.System_Int32:
                    case SpecialType.System_UInt32:
                        builder.Append("Math.trunc").Parenthesized()
                            .Append(syntax.Left, context).Space().Append(syntax.GetJavaScriptOperator()).Space().Append(syntax.Right, context)
                            .Close();
                        return builder;
                }
                break;
            }
            case SyntaxKind.EqualsExpression:
            case SyntaxKind.NotEqualsExpression:
            {
                IMethodSymbol? method;
                if (syntax.TryGetSymbol(context, out method))
                {
                    SymbolReplacement? replacement;
                    if (method.HasTypeScriptReplacement(out replacement))
                    {
                        switch(replacement.Kind)
                        {
                            case SymbolReplacementKind.StaticMethod:
                            {
                                if (replacement.Negate)
                                    builder.ExclamationMark();

                                builder.Append(replacement.Name).Parenthesized().Append(syntax.Left, context).CommaSeparator().Append(syntax.Right, context);
                                return builder;
                            }
                            default:
                            {
                                throw new NotImplementedException();
                            }
                        }
                    }
                }
                break;
            }
            default:
            {
                break;
            }
        }

        builder.Append(syntax.Left, context).Space().Append(syntax.GetJavaScriptOperator()).Space().Append(syntax.Right, context);
        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, CastExpressionSyntax syntax, TypeScriptCompilationContext context)
    {
        var typeSymbol = syntax.Type.GetSymbol<ITypeSymbol>(context);
        switch (typeSymbol.TypeKind)
        {
            case TypeKind.Enum:
            {
                // Enums don't support using normal cast
                builder.Append(syntax.Expression, context);
                break;
            }
            case TypeKind.TypeParameter:
            {
                // In case of type parameter, ignore actual instance check
                builder.Append(syntax.Expression, context).Space().Append("as").Space().Append(syntax.Type, context);
                break;
            }
            default:
            {
                switch (typeSymbol.SpecialType)
                {
                    case SpecialType.System_Boolean:
                        builder.Append("Boolean").Parenthesized().Append(syntax.Expression, context).Close();
                        return builder;
                    case SpecialType.System_IntPtr:
                    case SpecialType.System_SByte:
                    case SpecialType.System_Byte:
                    case SpecialType.System_Int16:
                    case SpecialType.System_UInt16:
                    case SpecialType.System_Int32:
                    case SpecialType.System_UInt32:
                    case SpecialType.System_Single:
                    case SpecialType.System_Double:
                        builder.Append("Number").Parenthesized().Append(syntax.Expression, context).Close();
                        return builder;
                    case SpecialType.System_Int64:
                    case SpecialType.System_UInt64:
                        builder.Append("BigInt").Parenthesized().Append(syntax.Expression, context).Close();
                        return builder;
                    default:
                        builder.Append("BinderUtils.cast<").Append(syntax.Type, context).Append(">").Parenthesized()
                            .Append(syntax.Expression, context).CommaSeparator().Append(syntax.Type, context).Close();
                        break;
                }
                break;
            }
        }

        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, ConditionalExpressionSyntax syntax, TypeScriptCompilationContext context)
    {
        builder.Append(syntax.Condition, context).Space().QuestionMark().Space()
            .Append(syntax.WhenTrue, context).Space().Colon().Space()
            .Append(syntax.WhenFalse, context);
        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, ElementAccessExpressionSyntax syntax, TypeScriptCompilationContext context)
    {
        var symbol = syntax.GetSymbol(context);
        if (symbol?.Kind == SymbolKind.Property)
        {
            var property = (IPropertySymbol)symbol;
            Debug.Assert(property.IsIndexer);
            builder.Append(syntax.Expression, context).Dot().Append(syntax, property, context).Parenthesized().Append(syntax.ArgumentList, false, context);
            return builder;
        }

        builder.Append(syntax.Expression, context).Append(syntax.ArgumentList, context);
        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, InitializerExpressionSyntax syntax, TypeScriptCompilationContext context)
    {
        builder.Append("([").Append(syntax.Expressions, context).Append("])");
        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, BaseExpressionSyntax syntax, TypeScriptCompilationContext context)
    {
        _ = syntax;
        _ = context;
        builder.Append("super");
        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, ThisExpressionSyntax syntax, TypeScriptCompilationContext context)
    {
        _ = syntax;
        _ = context;
        builder.Append("this");
        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, InvocationExpressionSyntax syntax, TypeScriptCompilationContext context)
    {
        var methodSymbol = syntax.GetSymbol<IMethodSymbol>(context);
        bool hasEmptyBody;
        if (methodSymbol.IsPartialMethod(out hasEmptyBody) && (hasEmptyBody || methodSymbol.PartialImplementationPart!.ShouldDiscard(context.Conversion)))
            return builder;

        if (methodSymbol.IsNative())
        {
            var refArguments = getRefArguments(syntax, context);
            if (refArguments.Count != 0)
            {
                writeRefInvocation(builder, syntax, methodSymbol, refArguments, context);
                return builder;
            }
        }

        builder.Append(syntax.Expression, context).Append(syntax.ArgumentList, context);
        return builder;
    }

    // FIXME: This is hacky. It should be fixed by improving existing ref invocation
    // tree manipulation. See TypeScriptValidationContext
    static void writeRefInvocation(CodeBuilder builder, InvocationExpressionSyntax invocation,
        IMethodSymbol method, List<RefArgument> refArguments, TypeScriptCompilationContext context)
    {
        foreach (var arg in refArguments)
        {
            string boxType;
            if (arg.Type.TypeKind == TypeKind.Enum)
                boxType = "NumberRefBox";
            else
                boxType = TypeScriptUtils.GetRefBoxType(arg.Type.GetFullName());

            builder.Append("let").Space().Append("__" + arg.Symbol.Name).Space().Colon().Space().Append(boxType).Space().Append("=").Space()
                .Append("new").Space().Append(boxType).EmptyParameterList().EndOfStatement();
        }

        if (!method.ReturnsVoid)
        {
            builder.Append("let").Space().Append("__ret").Space()
                .Append("=").Space();
        }

        builder.Append(invocation.Expression, context).Parenthesized().
            append(invocation.ArgumentList.Arguments, true, context).Close().EndOfStatement();

        bool first = true;
        foreach (var arg in refArguments)
        {
            if (first)
                first = false;
            else
                builder.EndOfStatement();

            builder.Append(arg.Symbol.Name).Space().Append("=").Space();

            builder.Parenthesized(() =>
            {
                builder.Append("__").Append(arg.Symbol.Name).Dot().Append("value");
                if (arg.Type.TypeKind == TypeKind.Enum)
                    builder.Space().Append("as").Space().Append(arg.Type.Name);
            });

            // Ensure it will work also for non nullable types by
            // unconditionally adding the ignore nullable operator '!'
            builder.ExclamationMark();
        }

        if (!method.ReturnsVoid)
            builder.EndOfStatement().Append("return").Space().Append("__ret");
    }

    static List<RefArgument> getRefArguments(InvocationExpressionSyntax invocation, TypeScriptCompilationContext context)
    {
        var ret = new List<RefArgument>();
        foreach (var arg in invocation.ArgumentList.Arguments)
        {
            if (!arg.RefKindKeyword.IsNone())
            {
                var symbol = arg.Expression.GetSymbol(context)!;
                ITypeSymbol type;
                switch (symbol.Kind)
                {
                    case SymbolKind.Local:
                    {
                        type = (symbol as ILocalSymbol)!.Type;
                        break;
                    }
                    case SymbolKind.Parameter:
                    {
                        type = (symbol as IParameterSymbol)!.Type;
                        break;
                    }
                    case SymbolKind.Field:
                    {
                        type = (symbol as IFieldSymbol)!.Type;
                        break;
                    }
                    default:
                        throw new NotSupportedException();
                }

                ret.Add(new RefArgument() { Argument = arg, Symbol = symbol, Type = type });
            }
        }

        return ret;
    }

    struct RefArgument
    {
        public ArgumentSyntax Argument;
        public ISymbol Symbol;
        public ITypeSymbol Type;
    }

    public static CodeBuilder Append(this CodeBuilder builder, ParameterListSyntax syntax, TypeScriptCompilationContext context)
    {
        builder.Parenthesized().Append(syntax.Parameters, syntax.Parameters.Count, context).Close();
        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, IReadOnlyList<ParameterSyntax> paramaters,
        int parameterCount, TypeScriptCompilationContext context)
    {
        bool first = true;
        for (int i = 0; i < parameterCount; i++)
        {
            var parameter = paramaters[i];
            builder.CommaAppendLine(ref first);
            writeParameter(builder, parameter, context);
        }

        return builder;
    }

    static void writeParameter(CodeBuilder builder, ParameterSyntax parameter, TypeScriptCompilationContext context)
    {
        var flags = TypeScriptTypeFlags.None;
        bool isRef = parameter.IsRef() | parameter.IsOut();
        if (isRef)
            flags |= TypeScriptTypeFlags.ByRef;

        builder.Append(parameter.Identifier.Text);
        if (parameter.Default != null)
            builder.QuestionMark(); // Use "or undefined" syntax

        builder.Colon().Space();
        builder.Append(parameter.Type!.GetTypeScriptType(flags, context));
    }

    public static CodeBuilder Append(this CodeBuilder builder, LiteralExpressionSyntax syntax, TypeScriptCompilationContext context)
    {
        _ = context;
        if (syntax.Token.IsKind(SyntaxKind.StringLiteralToken) && syntax.Token.Text.StartsWith("@"))
            builder.Append(syntax.Token.Text.Replace("\"", "\"\"")); // Handle verbatim strings
        else
            builder.Append(syntax.Token.Text);
        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, MemberAccessExpressionSyntax syntax, TypeScriptCompilationContext context)
    {
        var symbol = syntax.GetSymbol(context)!;
        // TODO2: This method is garbage, should be replaced by syntax tree manipulation
        switch (symbol.Kind)
        {
            case SymbolKind.Field:
            {
                var field = (IFieldSymbol)symbol;
                if (field.HasTypeScriptReplacement(out var replacement))
                {
                    switch (replacement.Kind)
                    {
                        case SymbolReplacementKind.Literal:
                            builder.Append(replacement.Name);
                            return builder;
                        default:
                            throw new NotSupportedException();
                    }

                }
                break;
            }
            case SymbolKind.Method:
            {
                var method = (IMethodSymbol)symbol;
                if (method.OriginalDefinition.HasTypeScriptReplacement(out var replacement))
                {
                    switch (replacement.Kind)
                    {
                        case SymbolReplacementKind.Method:
                            builder.Append(syntax.Expression, context).Dot().Append(replacement.Name);
                            return builder;
                        case SymbolReplacementKind.StaticMethod:
                            builder.Append(replacement.Name).Parenthesized().Append(syntax.Expression, context).Close();
                            return builder;
                        default:
                            throw new NotSupportedException();
                    }
                }

                if (method.IsGetEnumerator())
                {
                    builder.Append(syntax.Expression, context).Append("[Symbol.iterator]");
                    return builder;
                }

                break;
            }
            case SymbolKind.Property:
            {
                // TODO
                break;
            }
        }

        var typeSymbol = syntax.Expression.GetTypeSymbol(context);
        if (typeSymbol?.IsCLRPrimitiveType() == true)
        {
            string javaBoxType = TypeScriptUtils.GetBoxType(typeSymbol.GetFullName());
            builder.Parenthesized().Parenthesized().Append(javaBoxType).Close().Append(syntax.Expression, context).Close().Dot().Append(syntax.Name, context);
            return builder;
        }

        if (symbol.Kind == SymbolKind.Property
            && symbol.OriginalDefinition.ContainingType.GetFullName() == "System.Nullable<T>"
            && symbol.Name == "Value")
        {
            // There are no nullable types in Java, just discard ".Value" accessor
            builder.Append(syntax.Expression, context);
            return builder;
        }

        builder.Append(syntax.Expression, context).Dot().Append(syntax.Name, context);
        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, ObjectCreationExpressionSyntax syntax, TypeScriptCompilationContext context)
    {
        var symbol = syntax.GetSymbol<IMethodSymbol>(context);
        var name = symbol.GetTypeScriptName(context, out bool isOverload);
        if (isOverload)
        {
            builder.Append(symbol.ContainingType.Name).Dot().Append(name).Append(syntax.ArgumentList!, context);
        }
        else
        {
            switch (symbol.ContainingType.SpecialType)
            {
                case SpecialType.System_Boolean:
                    builder.Append("false");
                    return builder;
                case SpecialType.System_IntPtr:
                case SpecialType.System_SByte:
                case SpecialType.System_Byte:
                case SpecialType.System_Int16:
                case SpecialType.System_UInt16:
                case SpecialType.System_Int32:
                case SpecialType.System_UInt32:
                case SpecialType.System_Single:
                case SpecialType.System_Double:
                    builder.Append("0");
                    return builder;
                case SpecialType.System_Int64:
                case SpecialType.System_UInt64:
                    builder.Append("0n");
                    return builder;
                default:
                    builder.Append("new").Space().Append(syntax.Type, context).Append(syntax.ArgumentList!, context);
                    return builder;
            }
        }

        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, ParenthesizedExpressionSyntax syntax, TypeScriptCompilationContext context)
    {
        builder.Parenthesized().Append(syntax.Expression, context);
        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, PostfixUnaryExpressionSyntax syntax, TypeScriptCompilationContext context)
    {
        builder.Append(syntax.Operand, context).Append(syntax.GetJavaScriptOperator());
        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, PrefixUnaryExpressionSyntax syntax, TypeScriptCompilationContext context)
    {
        builder.Append(syntax.GetJavaScriptOperator()).Append(syntax.Operand, context);
        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, TypeOfExpressionSyntax syntax, TypeScriptCompilationContext context)
    {
        builder.Append(syntax.Type, context);
        return builder;
    }

    // Reference: roslyn/src/Compilers/CSharp/Portable/Generated/Syntax.xml.Main.Generated.cs
    public static CodeBuilder Append(this CodeBuilder builder, ExpressionSyntax expression, TypeScriptCompilationContext context)
    {
        // FIX-ME: This should be replaced by syntax tree manipulation (hard)
        void tryAddThis()
        {
            var symbol = expression.GetSymbol(context);
            if (symbol != null)
            {
                if (symbol.IsStatic)
                {
                    switch (symbol.Kind)
                    {
                        case SymbolKind.Field:
                        case SymbolKind.Property:
                            builder = builder.Append(symbol.ContainingType.Name).Dot();
                            break;
                        case SymbolKind.Method:
                            var method = (IMethodSymbol)symbol;
                            if (!method.IsNative())
                                builder = builder.Append(symbol.ContainingType.Name).Dot();
                            break;
                    }
                }
                else
                {
                    switch (symbol.Kind)
                    {
                        case SymbolKind.Field:
                            builder = builder.Append("this").Dot();
                            break;
                        case SymbolKind.Method:
                            var method = (IMethodSymbol)symbol;
                            if (!(method.MethodKind == MethodKind.LocalFunction))
                                builder = builder.Append("this").Dot();
                            break;
                        case SymbolKind.Property:
                            var property = (IPropertySymbol)symbol;
                            if (!property.IsIndexer)
                                builder = builder.Append("this").Dot();
                            break;
                    }
                }
            }
        }

        var kind = expression.Kind();
        switch (kind)
        {
            case SyntaxKind.ArrayCreationExpression:
                return builder.Append((ArrayCreationExpressionSyntax)expression, context);
            case SyntaxKind.OmittedArraySizeExpression:
                return builder.Append((OmittedArraySizeExpressionSyntax)expression, context);
            case SyntaxKind.AddAssignmentExpression:
            case SyntaxKind.AndAssignmentExpression:
            case SyntaxKind.DivideAssignmentExpression:
            case SyntaxKind.ExclusiveOrAssignmentExpression:
            case SyntaxKind.LeftShiftAssignmentExpression:
            case SyntaxKind.ModuloAssignmentExpression:
            case SyntaxKind.MultiplyAssignmentExpression:
            case SyntaxKind.OrAssignmentExpression:
            case SyntaxKind.RightShiftAssignmentExpression:
            case SyntaxKind.SimpleAssignmentExpression:
            case SyntaxKind.SubtractAssignmentExpression:
                return builder.Append((AssignmentExpressionSyntax)expression, context);
            case SyntaxKind.AddExpression:
            case SyntaxKind.SubtractExpression:
            case SyntaxKind.MultiplyExpression:
            case SyntaxKind.DivideExpression:
            case SyntaxKind.ModuloExpression:
            case SyntaxKind.LeftShiftExpression:
            case SyntaxKind.RightShiftExpression:
            case SyntaxKind.LogicalOrExpression:
            case SyntaxKind.LogicalAndExpression:
            case SyntaxKind.BitwiseOrExpression:
            case SyntaxKind.BitwiseAndExpression:
            case SyntaxKind.ExclusiveOrExpression:
            case SyntaxKind.EqualsExpression:
            case SyntaxKind.NotEqualsExpression:
            case SyntaxKind.LessThanExpression:
            case SyntaxKind.LessThanOrEqualExpression:
            case SyntaxKind.GreaterThanExpression:
            case SyntaxKind.GreaterThanOrEqualExpression:
            case SyntaxKind.IsExpression:
            case SyntaxKind.AsExpression:
                return builder.Append((BinaryExpressionSyntax)expression, context);
            case SyntaxKind.CastExpression:
                return builder.Append((CastExpressionSyntax)expression, context);
            case SyntaxKind.ConditionalExpression:
                return builder.Append((ConditionalExpressionSyntax)expression, context);
            case SyntaxKind.ElementAccessExpression:
                return builder.Append((ElementAccessExpressionSyntax)expression, context);
            case SyntaxKind.ObjectInitializerExpression:
            case SyntaxKind.CollectionInitializerExpression:
            case SyntaxKind.ArrayInitializerExpression:
            case SyntaxKind.ComplexElementInitializerExpression:
                return builder.Append((InitializerExpressionSyntax)expression, context);
            case SyntaxKind.BaseExpression:
                return builder.Append((BaseExpressionSyntax)expression, context);
            case SyntaxKind.ThisExpression:
                return builder.Append((ThisExpressionSyntax)expression, context);
            case SyntaxKind.InvocationExpression:
                return builder.Append((InvocationExpressionSyntax)expression, context);
            case SyntaxKind.NumericLiteralExpression:
            case SyntaxKind.StringLiteralExpression:
            case SyntaxKind.CharacterLiteralExpression:
            case SyntaxKind.TrueLiteralExpression:
            case SyntaxKind.FalseLiteralExpression:
            case SyntaxKind.NullLiteralExpression:
                return builder.Append((LiteralExpressionSyntax)expression, context);
            case SyntaxKind.SimpleMemberAccessExpression:
                return builder.Append((MemberAccessExpressionSyntax)expression, context);
            case SyntaxKind.ObjectCreationExpression:
                return builder.Append((ObjectCreationExpressionSyntax)expression, context);
            case SyntaxKind.ParenthesizedExpression:
                return builder.Append((ParenthesizedExpressionSyntax)expression, context);
            case SyntaxKind.PostIncrementExpression:
            case SyntaxKind.PostDecrementExpression:
            case SyntaxKind.SuppressNullableWarningExpression:
                return builder.Append((PostfixUnaryExpressionSyntax)expression, context);
            case SyntaxKind.UnaryPlusExpression:
            case SyntaxKind.UnaryMinusExpression:
            case SyntaxKind.BitwiseNotExpression:
            case SyntaxKind.LogicalNotExpression:
            case SyntaxKind.PreIncrementExpression:
            case SyntaxKind.PreDecrementExpression:
                return builder.Append((PrefixUnaryExpressionSyntax)expression, context);
            case SyntaxKind.TypeOfExpression:
                return builder.Append((TypeOfExpressionSyntax)expression, context);
            case SyntaxKind.QualifiedName:
            case SyntaxKind.ArrayType:
            case SyntaxKind.GenericName:
            case SyntaxKind.IdentifierName:
            case SyntaxKind.NullableType:
            case SyntaxKind.OmittedTypeArgument:
            case SyntaxKind.PredefinedType:
            case SyntaxKind.RefType:
                tryAddThis();
                return builder.Append((TypeSyntax)expression, context);
            // Unsupported expressions
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
            // Unsupported prefix unary expressions
            case SyntaxKind.AddressOfExpression:
            case SyntaxKind.PointerIndirectionExpression:
            // Unsupported binary expressions
            case SyntaxKind.CoalesceExpression:
            // Unsupported member access expressions
            case SyntaxKind.PointerMemberAccessExpression:
            // Unsupported literal expressions
            case SyntaxKind.ArgListExpression:
            case SyntaxKind.DefaultLiteralExpression:
            // Unsupported type expressions
            case SyntaxKind.AliasQualifiedName:
            case SyntaxKind.TupleType:
            case SyntaxKind.PointerType:
            default:
                throw new Exception();
        }
    }

    public static CodeBuilder Append(this CodeBuilder builder, BracketedArgumentListSyntax syntax, TypeScriptCompilationContext context)
    {
        builder.Append(syntax, true, context);
        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, BracketedArgumentListSyntax syntax, bool bracketed, TypeScriptCompilationContext context)
    {
        if (bracketed)
            builder.Bracketed().Append(syntax.Arguments, context);
        else
            builder.Append(syntax.Arguments, context);

        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, ArgumentListSyntax syntax, TypeScriptCompilationContext context)
    {
        builder.Parenthesized().Append(syntax.Arguments, context);
        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, IEnumerable<ArgumentSyntax> arguments, TypeScriptCompilationContext context)
    {
        return append(builder, arguments, false, context);
    }

    static CodeBuilder append(this CodeBuilder builder, IEnumerable<ArgumentSyntax> arguments, bool refInvocation, TypeScriptCompilationContext context)
    {
        bool first = true;
        foreach (var arg in arguments)
        {
            builder.CommaSeparator(ref first);
            if (refInvocation && !arg.RefKindKeyword.IsNone())
            {
                // In native invocations, prepend "__" for ref/out arguments
                builder.Append("__");
            }

            builder.Append(arg.Expression, context);
        }

        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, IEnumerable<ExpressionSyntax> expressions, TypeScriptCompilationContext context)
    {
        bool first = true;
        foreach (var expression in expressions)
            builder.CommaSeparator(ref first).Append(expression, context);

        return builder;
    }
}
