using CodeBinder.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace CodeBinder.Shared.CSharp
{
    class CSharpNodeVisitor : CSharpNodeVisitor<CSharpSyntaxTreeContext, CSharpCompilationContext, CSharpLanguageConversion>
    {
        private Stack<CSharpBaseTypeContext> _parents;

        public CSharpBaseTypeContext CurrentParent
        {
            get
            {
                if (_parents.Count == 0)
                    return null;

                var ret = _parents.Peek();
                if (ret != null)
                {
                    // Verify if the current parent is actually a partial type and, if so,
                    // use that istead
                    string parentQualifiedName = ret.Node.GetQualifiedName(this);
                    if (Compilation.TryGetPartialType(parentQualifiedName, out var parentPartialType))
                        ret = parentPartialType;
                }

                return ret;
            }
        }

        private void Unsupported(SyntaxNode node, string message = null)
        {
            if (message == null)
                throw new Exception("Unsupported node: " + node);
            else
                throw new Exception("Unsupported node: " + node + ", " + message);
        }

        #region Supported types

        public CSharpNodeVisitor(CSharpSyntaxTreeContext context)
            : base(context, context.Compilation, context.Compilation.Conversion)
        {
            _parents = new Stack<CSharpBaseTypeContext>();
        }

        public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            bool isPartial;
            checkTypeDeclaration(node, out isPartial);
            var type = new CSharpInterfaceTypeContext(node, Compilation, Conversion.GetInterfaceTypeConversion());
            addType(type, isPartial);
            DefaultVisit(node);
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            bool isPartial;
            checkTypeDeclaration(node, out isPartial);
            var type = new CSharpClassTypeContext(node, Compilation, Conversion.GetClassTypeConversion());
            addType(type, isPartial);
            _parents.Push(type);
            DefaultVisit(node);
            _parents.Pop();
        }

        public override void VisitStructDeclaration(StructDeclarationSyntax node)
        {
            bool isPartial;
            checkTypeDeclaration(node, out isPartial);
            var type = new CSharpStructTypeContext(node, Compilation, Conversion.GetStructTypeConversion());
            addType(type, isPartial);
            _parents.Push(type);
            DefaultVisit(node);
            _parents.Pop();
        }

        public override void VisitEnumDeclaration(EnumDeclarationSyntax node)
        {
            var type = new CSharpEnumTypeContext(node, Compilation, Conversion.GetEnumTypeConversion());
            TreeContext.AddType(type, CurrentParent);
            DefaultVisit(node);
        }

        void addType(CSharpTypeContext type, bool isPartial)
        {
            if (isPartial)
            {
                string qualifiedName = type.Node.GetQualifiedName(this);
                Compilation.AddPartialType(qualifiedName, Compilation, type, CurrentParent);
            }
            else
            {
                TreeContext.AddType(type, CurrentParent);
            }
        }

        public override void Visit(SyntaxNode node)
        {
            if (node.ShouldDiscard(this))
                return;

            var kind = node.Kind();
            switch (kind)
            {
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
                // Yield statements
                case SyntaxKind.YieldBreakStatement:
                case SyntaxKind.YieldReturnStatement:
                // Goto statements
                case SyntaxKind.GotoStatement:
                case SyntaxKind.GotoCaseStatement:
                case SyntaxKind.GotoDefaultStatement:
                // Expressions
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

        #region Unsupported syntax

        public override void VisitAssignmentExpression(AssignmentExpressionSyntax node)
        {
            var leftSymbol = node.Left.GetSymbol(this);
            if (leftSymbol?.Kind == SymbolKind.Parameter)
            {
                var parameter = leftSymbol as IParameterSymbol;
                if (parameter.Type.TypeKind == TypeKind.Struct
                        && parameter.RefKind != RefKind.None
                        && !parameter.Type.IsCLRPrimitiveType())
                    Unsupported(node, "Assignment of ref/out structured type");
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
                if (!arg.RefKindKeyword.IsNone())
                {
                    var argSymbol = arg.Expression.GetSymbol(this);
                    ITypeSymbol argType = null;
                    switch (argSymbol.Kind)
                    {
                        case SymbolKind.Local:
                            argType = (argSymbol as ILocalSymbol).Type;
                            break;
                        case SymbolKind.Parameter:
                            argType = (argSymbol as IParameterSymbol).Type;
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
                            if (!argType.IsCLRPrimitiveType())
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
                        default:
                            Unsupported(node, "Unsupported ref like keyword for non-struct/enum type");
                            break;
                    }

                    // Must be within an invocation
                    assertParent(node.Parent, SyntaxKind.InvocationExpression);

                    StatementKind statementKind;
                    if (node.Parent.Parent.IsStatement(out statementKind) && statementKind == StatementKind.Expression)
                    {
                        // non-return invocation contained in a block
                        assertParent(node.Parent.Parent.Parent, SyntaxKind.Block, SyntaxKind.SwitchSection);
                        return;
                    }

                    ExpressionKind expressionKind;
                    if (node.Parent.Parent.IsExpression(out expressionKind) && expressionKind == ExpressionKind.Assignment)
                    {
                        // non-return invocation contained in a assignment expressio, contained in a block
                        assertParent(node.Parent.Parent.Parent, SyntaxKind.ExpressionStatement);
                        assertParent(node.Parent.Parent.Parent.Parent, SyntaxKind.Block, SyntaxKind.SwitchSection);
                        return;
                    }

                    if (node.Parent.Parent.IsKind(SyntaxKind.EqualsValueClause))
                    {
                        // Local declaration and assignment with invocation
                        assertParent(node.Parent.Parent.Parent.Parent.Parent, SyntaxKind.LocalDeclarationStatement);
                        assertParent(node.Parent.Parent.Parent.Parent.Parent.Parent, SyntaxKind.Block, SyntaxKind.SwitchSection);
                        return;
                    }

                    Unsupported(node, "ref like keyword in unsupported context");
                    break;
                }
            }

            DefaultVisit(node);
        }

        public override void VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
        {
            if (node.Initializer != null)
                Unsupported(node, "Object initializer");

            DefaultVisit(node);
        }

        public override void VisitParameter(ParameterSyntax node)
        {
            if (node.Default != null)
                Unsupported(node, "Optional parameter");

            DefaultVisit(node);
        }

        public override void VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
        {
            if (!node.OperatorToken.IsKind(SyntaxKind.DotToken))
                Unsupported(node, "Not dot member access");

            DefaultVisit(node);
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            if (node.Modifiers.Any(SyntaxKind.PartialKeyword))
                Unsupported(node, "Partial method");

            DefaultVisit(node);
        }

        public override void VisitIdentifierName(IdentifierNameSyntax node)
        {
            var symbol = node.GetSymbol(this);
            switch (symbol.Kind)
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
                case "System.Char":
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

        public override void VisitLiteralExpression(LiteralExpressionSyntax node)
        {
            if (node.Kind() == SyntaxKind.StringLiteralExpression && node.Token.Text.StartsWith("@"))
                Unsupported(node, "Verbatim string literal");

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
                if (current.Kind() == SyntaxKind.Attribute)
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
            if (node.Modifiers.Any((token) => token.Kind() != SyntaxKind.ConstKeyword))
                Unsupported(node, "Variable declaration with unsupported modifiers");

            DefaultVisit(node);
        }

        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            if (node.AccessorList == null)
                Unsupported(node, "Unsupported property with no accessor definied: use \"get\" or \"set\"");

            DefaultVisit(node);
        }

        public override void VisitArrayRankSpecifier(ArrayRankSpecifierSyntax node)
        {
            if (node.Rank > 1)
                Unsupported(node, "Unsupported array with rank > 1");

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
            isPartial = type.Modifiers.Any(SyntaxKind.PartialKeyword);
            if (isPartial)
            {
                Debug.Assert(type.Parent != null);
                var parentKind = type.Parent.Kind();
                switch (parentKind)
                {
                    case SyntaxKind.ClassDeclaration:
                    case SyntaxKind.InterfaceDeclaration:
                    case SyntaxKind.StructDeclaration:
                        var parentType = type.Parent as TypeDeclarationSyntax;
                        if (!parentType.Modifiers.Any(SyntaxKind.PartialKeyword))
                            Unsupported(type, "Nested partial types must have partial parent");

                        break;
                }
            }
        }

        #endregion // Unsupported syntax
    }

    public class CSharpNodeVisitor<TSyntaxTree, TCompilation, TLanguageConversion> : CSharpSyntaxWalker, ICompilationContextProvider
        where TSyntaxTree : SyntaxTreeContext
        where TCompilation : CompilationContext
        where TLanguageConversion : LanguageConversion
    {
        public TSyntaxTree TreeContext { get; private set; }
        public TCompilation Compilation { get; private set; }
        public TLanguageConversion Conversion { get; private set; }

        public CSharpNodeVisitor(TSyntaxTree treeContext, TCompilation compilation, TLanguageConversion conversion)
        {
            TreeContext = treeContext;
            Compilation = compilation;
            Conversion = conversion;
        }

        CompilationContext ICompilationContextProvider.Compilation
        {
            get { return Compilation; }
        }
    }
}
