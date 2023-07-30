// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.Shared.CSharp;

public class CSharpNodeVisitor : NodeVisitor
{
    Walker _walker;

    public CSharpNodeVisitor()
    {
        _walker = new Walker(this);
    }

    protected override void VisitTree(SyntaxTree tree)
    {
        _walker.Visit(tree.GetRoot());
    }

    public event Action<CSharpNodeVisitor, IdentifierNameSyntax>? IdentifierNameVisit;
    public event Action<CSharpNodeVisitor, QualifiedNameSyntax>? QualifiedNameVisit;
    public event Action<CSharpNodeVisitor, GenericNameSyntax>? GenericNameVisit;
    public event Action<CSharpNodeVisitor, TypeArgumentListSyntax>? TypeArgumentListVisit;
    public event Action<CSharpNodeVisitor, AliasQualifiedNameSyntax>? AliasQualifiedNameVisit;
    public event Action<CSharpNodeVisitor, PredefinedTypeSyntax>? PredefinedTypeVisit;
    public event Action<CSharpNodeVisitor, ArrayTypeSyntax>? ArrayTypeVisit;
    public event Action<CSharpNodeVisitor, ArrayRankSpecifierSyntax>? ArrayRankSpecifierVisit;
    public event Action<CSharpNodeVisitor, PointerTypeSyntax>? PointerTypeVisit;
    public event Action<CSharpNodeVisitor, FunctionPointerTypeSyntax>? FunctionPointerTypeVisit;
    public event Action<CSharpNodeVisitor, FunctionPointerParameterListSyntax>? FunctionPointerParameterListVisit;
    public event Action<CSharpNodeVisitor, FunctionPointerCallingConventionSyntax>? FunctionPointerCallingConventionVisit;
    public event Action<CSharpNodeVisitor, FunctionPointerUnmanagedCallingConventionListSyntax>? FunctionPointerUnmanagedCallingConventionListVisit;
    public event Action<CSharpNodeVisitor, FunctionPointerUnmanagedCallingConventionSyntax>? FunctionPointerUnmanagedCallingConventionVisit;
    public event Action<CSharpNodeVisitor, NullableTypeSyntax>? NullableTypeVisit;
    public event Action<CSharpNodeVisitor, TupleTypeSyntax>? TupleTypeVisit;
    public event Action<CSharpNodeVisitor, TupleElementSyntax>? TupleElementVisit;
    public event Action<CSharpNodeVisitor, OmittedTypeArgumentSyntax>? OmittedTypeArgumentVisit;
    public event Action<CSharpNodeVisitor, RefTypeSyntax>? RefTypeVisit;
    public event Action<CSharpNodeVisitor, ScopedTypeSyntax>? ScopedTypeVisit;
    public event Action<CSharpNodeVisitor, ParenthesizedExpressionSyntax>? ParenthesizedExpressionVisit;
    public event Action<CSharpNodeVisitor, TupleExpressionSyntax>? TupleExpressionVisit;
    public event Action<CSharpNodeVisitor, PrefixUnaryExpressionSyntax>? PrefixUnaryExpressionVisit;
    public event Action<CSharpNodeVisitor, AwaitExpressionSyntax>? AwaitExpressionVisit;
    public event Action<CSharpNodeVisitor, PostfixUnaryExpressionSyntax>? PostfixUnaryExpressionVisit;
    public event Action<CSharpNodeVisitor, MemberAccessExpressionSyntax>? MemberAccessExpressionVisit;
    public event Action<CSharpNodeVisitor, ConditionalAccessExpressionSyntax>? ConditionalAccessExpressionVisit;
    public event Action<CSharpNodeVisitor, MemberBindingExpressionSyntax>? MemberBindingExpressionVisit;
    public event Action<CSharpNodeVisitor, ElementBindingExpressionSyntax>? ElementBindingExpressionVisit;
    public event Action<CSharpNodeVisitor, RangeExpressionSyntax>? RangeExpressionVisit;
    public event Action<CSharpNodeVisitor, ImplicitElementAccessSyntax>? ImplicitElementAccessVisit;
    public event Action<CSharpNodeVisitor, BinaryExpressionSyntax>? BinaryExpressionVisit;
    public event Action<CSharpNodeVisitor, AssignmentExpressionSyntax>? AssignmentExpressionVisit;
    public event Action<CSharpNodeVisitor, ConditionalExpressionSyntax>? ConditionalExpressionVisit;
    public event Action<CSharpNodeVisitor, ThisExpressionSyntax>? ThisExpressionVisit;
    public event Action<CSharpNodeVisitor, BaseExpressionSyntax>? BaseExpressionVisit;
    public event Action<CSharpNodeVisitor, LiteralExpressionSyntax>? LiteralExpressionVisit;
    public event Action<CSharpNodeVisitor, MakeRefExpressionSyntax>? MakeRefExpressionVisit;
    public event Action<CSharpNodeVisitor, RefTypeExpressionSyntax>? RefTypeExpressionVisit;
    public event Action<CSharpNodeVisitor, RefValueExpressionSyntax>? RefValueExpressionVisit;
    public event Action<CSharpNodeVisitor, CheckedExpressionSyntax>? CheckedExpressionVisit;
    public event Action<CSharpNodeVisitor, DefaultExpressionSyntax>? DefaultExpressionVisit;
    public event Action<CSharpNodeVisitor, TypeOfExpressionSyntax>? TypeOfExpressionVisit;
    public event Action<CSharpNodeVisitor, SizeOfExpressionSyntax>? SizeOfExpressionVisit;
    public event Action<CSharpNodeVisitor, InvocationExpressionSyntax>? InvocationExpressionVisit;
    public event Action<CSharpNodeVisitor, ElementAccessExpressionSyntax>? ElementAccessExpressionVisit;
    public event Action<CSharpNodeVisitor, ArgumentListSyntax>? ArgumentListVisit;
    public event Action<CSharpNodeVisitor, BracketedArgumentListSyntax>? BracketedArgumentListVisit;
    public event Action<CSharpNodeVisitor, ArgumentSyntax>? ArgumentVisit;
    public event Action<CSharpNodeVisitor, ExpressionColonSyntax>? ExpressionColonVisit;
    public event Action<CSharpNodeVisitor, NameColonSyntax>? NameColonVisit;
    public event Action<CSharpNodeVisitor, DeclarationExpressionSyntax>? DeclarationExpressionVisit;
    public event Action<CSharpNodeVisitor, CastExpressionSyntax>? CastExpressionVisit;
    public event Action<CSharpNodeVisitor, AnonymousMethodExpressionSyntax>? AnonymousMethodExpressionVisit;
    public event Action<CSharpNodeVisitor, SimpleLambdaExpressionSyntax>? SimpleLambdaExpressionVisit;
    public event Action<CSharpNodeVisitor, RefExpressionSyntax>? RefExpressionVisit;
    public event Action<CSharpNodeVisitor, ParenthesizedLambdaExpressionSyntax>? ParenthesizedLambdaExpressionVisit;
    public event Action<CSharpNodeVisitor, InitializerExpressionSyntax>? InitializerExpressionVisit;
    public event Action<CSharpNodeVisitor, ImplicitObjectCreationExpressionSyntax>? ImplicitObjectCreationExpressionVisit;
    public event Action<CSharpNodeVisitor, ObjectCreationExpressionSyntax>? ObjectCreationExpressionVisit;
    public event Action<CSharpNodeVisitor, WithExpressionSyntax>? WithExpressionVisit;
    public event Action<CSharpNodeVisitor, AnonymousObjectMemberDeclaratorSyntax>? AnonymousObjectMemberDeclaratorVisit;
    public event Action<CSharpNodeVisitor, AnonymousObjectCreationExpressionSyntax>? AnonymousObjectCreationExpressionVisit;
    public event Action<CSharpNodeVisitor, ArrayCreationExpressionSyntax>? ArrayCreationExpressionVisit;
    public event Action<CSharpNodeVisitor, ImplicitArrayCreationExpressionSyntax>? ImplicitArrayCreationExpressionVisit;
    public event Action<CSharpNodeVisitor, StackAllocArrayCreationExpressionSyntax>? StackAllocArrayCreationExpressionVisit;
    public event Action<CSharpNodeVisitor, ImplicitStackAllocArrayCreationExpressionSyntax>? ImplicitStackAllocArrayCreationExpressionVisit;
    public event Action<CSharpNodeVisitor, QueryExpressionSyntax>? QueryExpressionVisit;
    public event Action<CSharpNodeVisitor, QueryBodySyntax>? QueryBodyVisit;
    public event Action<CSharpNodeVisitor, FromClauseSyntax>? FromClauseVisit;
    public event Action<CSharpNodeVisitor, LetClauseSyntax>? LetClauseVisit;
    public event Action<CSharpNodeVisitor, JoinClauseSyntax>? JoinClauseVisit;
    public event Action<CSharpNodeVisitor, JoinIntoClauseSyntax>? JoinIntoClauseVisit;
    public event Action<CSharpNodeVisitor, WhereClauseSyntax>? WhereClauseVisit;
    public event Action<CSharpNodeVisitor, OrderByClauseSyntax>? OrderByClauseVisit;
    public event Action<CSharpNodeVisitor, OrderingSyntax>? OrderingVisit;
    public event Action<CSharpNodeVisitor, SelectClauseSyntax>? SelectClauseVisit;
    public event Action<CSharpNodeVisitor, GroupClauseSyntax>? GroupClauseVisit;
    public event Action<CSharpNodeVisitor, QueryContinuationSyntax>? QueryContinuationVisit;
    public event Action<CSharpNodeVisitor, OmittedArraySizeExpressionSyntax>? OmittedArraySizeExpressionVisit;
    public event Action<CSharpNodeVisitor, InterpolatedStringExpressionSyntax>? InterpolatedStringExpressionVisit;
    public event Action<CSharpNodeVisitor, IsPatternExpressionSyntax>? IsPatternExpressionVisit;
    public event Action<CSharpNodeVisitor, ThrowExpressionSyntax>? ThrowExpressionVisit;
    public event Action<CSharpNodeVisitor, WhenClauseSyntax>? WhenClauseVisit;
    public event Action<CSharpNodeVisitor, DiscardPatternSyntax>? DiscardPatternVisit;
    public event Action<CSharpNodeVisitor, DeclarationPatternSyntax>? DeclarationPatternVisit;
    public event Action<CSharpNodeVisitor, VarPatternSyntax>? VarPatternVisit;
    public event Action<CSharpNodeVisitor, RecursivePatternSyntax>? RecursivePatternVisit;
    public event Action<CSharpNodeVisitor, PositionalPatternClauseSyntax>? PositionalPatternClauseVisit;
    public event Action<CSharpNodeVisitor, PropertyPatternClauseSyntax>? PropertyPatternClauseVisit;
    public event Action<CSharpNodeVisitor, SubpatternSyntax>? SubpatternVisit;
    public event Action<CSharpNodeVisitor, ConstantPatternSyntax>? ConstantPatternVisit;
    public event Action<CSharpNodeVisitor, ParenthesizedPatternSyntax>? ParenthesizedPatternVisit;
    public event Action<CSharpNodeVisitor, RelationalPatternSyntax>? RelationalPatternVisit;
    public event Action<CSharpNodeVisitor, TypePatternSyntax>? TypePatternVisit;
    public event Action<CSharpNodeVisitor, BinaryPatternSyntax>? BinaryPatternVisit;
    public event Action<CSharpNodeVisitor, UnaryPatternSyntax>? UnaryPatternVisit;
    public event Action<CSharpNodeVisitor, ListPatternSyntax>? ListPatternVisit;
    public event Action<CSharpNodeVisitor, SlicePatternSyntax>? SlicePatternVisit;
    public event Action<CSharpNodeVisitor, InterpolatedStringTextSyntax>? InterpolatedStringTextVisit;
    public event Action<CSharpNodeVisitor, InterpolationSyntax>? InterpolationVisit;
    public event Action<CSharpNodeVisitor, InterpolationAlignmentClauseSyntax>? InterpolationAlignmentClauseVisit;
    public event Action<CSharpNodeVisitor, InterpolationFormatClauseSyntax>? InterpolationFormatClauseVisit;
    public event Action<CSharpNodeVisitor, GlobalStatementSyntax>? GlobalStatementVisit;
    public event Action<CSharpNodeVisitor, BlockSyntax>? BlockVisit;
    public event Action<CSharpNodeVisitor, LocalFunctionStatementSyntax>? LocalFunctionStatementVisit;
    public event Action<CSharpNodeVisitor, LocalDeclarationStatementSyntax>? LocalDeclarationStatementVisit;
    public event Action<CSharpNodeVisitor, VariableDeclarationSyntax>? VariableDeclarationVisit;
    public event Action<CSharpNodeVisitor, VariableDeclaratorSyntax>? VariableDeclaratorVisit;
    public event Action<CSharpNodeVisitor, EqualsValueClauseSyntax>? EqualsValueClauseVisit;
    public event Action<CSharpNodeVisitor, SingleVariableDesignationSyntax>? SingleVariableDesignationVisit;
    public event Action<CSharpNodeVisitor, DiscardDesignationSyntax>? DiscardDesignationVisit;
    public event Action<CSharpNodeVisitor, ParenthesizedVariableDesignationSyntax>? ParenthesizedVariableDesignationVisit;
    public event Action<CSharpNodeVisitor, ExpressionStatementSyntax>? ExpressionStatementVisit;
    public event Action<CSharpNodeVisitor, EmptyStatementSyntax>? EmptyStatementVisit;
    public event Action<CSharpNodeVisitor, LabeledStatementSyntax>? LabeledStatementVisit;
    public event Action<CSharpNodeVisitor, GotoStatementSyntax>? GotoStatementVisit;
    public event Action<CSharpNodeVisitor, BreakStatementSyntax>? BreakStatementVisit;
    public event Action<CSharpNodeVisitor, ContinueStatementSyntax>? ContinueStatementVisit;
    public event Action<CSharpNodeVisitor, ReturnStatementSyntax>? ReturnStatementVisit;
    public event Action<CSharpNodeVisitor, ThrowStatementSyntax>? ThrowStatementVisit;
    public event Action<CSharpNodeVisitor, YieldStatementSyntax>? YieldStatementVisit;
    public event Action<CSharpNodeVisitor, WhileStatementSyntax>? WhileStatementVisit;
    public event Action<CSharpNodeVisitor, DoStatementSyntax>? DoStatementVisit;
    public event Action<CSharpNodeVisitor, ForStatementSyntax>? ForStatementVisit;
    public event Action<CSharpNodeVisitor, ForEachStatementSyntax>? ForEachStatementVisit;
    public event Action<CSharpNodeVisitor, ForEachVariableStatementSyntax>? ForEachVariableStatementVisit;
    public event Action<CSharpNodeVisitor, UsingStatementSyntax>? UsingStatementVisit;
    public event Action<CSharpNodeVisitor, FixedStatementSyntax>? FixedStatementVisit;
    public event Action<CSharpNodeVisitor, CheckedStatementSyntax>? CheckedStatementVisit;
    public event Action<CSharpNodeVisitor, UnsafeStatementSyntax>? UnsafeStatementVisit;
    public event Action<CSharpNodeVisitor, LockStatementSyntax>? LockStatementVisit;
    public event Action<CSharpNodeVisitor, IfStatementSyntax>? IfStatementVisit;
    public event Action<CSharpNodeVisitor, ElseClauseSyntax>? ElseClauseVisit;
    public event Action<CSharpNodeVisitor, SwitchStatementSyntax>? SwitchStatementVisit;
    public event Action<CSharpNodeVisitor, SwitchSectionSyntax>? SwitchSectionVisit;
    public event Action<CSharpNodeVisitor, CasePatternSwitchLabelSyntax>? CasePatternSwitchLabelVisit;
    public event Action<CSharpNodeVisitor, CaseSwitchLabelSyntax>? CaseSwitchLabelVisit;
    public event Action<CSharpNodeVisitor, DefaultSwitchLabelSyntax>? DefaultSwitchLabelVisit;
    public event Action<CSharpNodeVisitor, SwitchExpressionSyntax>? SwitchExpressionVisit;
    public event Action<CSharpNodeVisitor, SwitchExpressionArmSyntax>? SwitchExpressionArmVisit;
    public event Action<CSharpNodeVisitor, TryStatementSyntax>? TryStatementVisit;
    public event Action<CSharpNodeVisitor, CatchClauseSyntax>? CatchClauseVisit;
    public event Action<CSharpNodeVisitor, CatchDeclarationSyntax>? CatchDeclarationVisit;
    public event Action<CSharpNodeVisitor, CatchFilterClauseSyntax>? CatchFilterClauseVisit;
    public event Action<CSharpNodeVisitor, FinallyClauseSyntax>? FinallyClauseVisit;
    public event Action<CSharpNodeVisitor, CompilationUnitSyntax>? CompilationUnitVisit;
    public event Action<CSharpNodeVisitor, ExternAliasDirectiveSyntax>? ExternAliasDirectiveVisit;
    public event Action<CSharpNodeVisitor, UsingDirectiveSyntax>? UsingDirectiveVisit;
    public event Action<CSharpNodeVisitor, NamespaceDeclarationSyntax>? NamespaceDeclarationVisit;
    public event Action<CSharpNodeVisitor, FileScopedNamespaceDeclarationSyntax>? FileScopedNamespaceDeclarationVisit;
    public event Action<CSharpNodeVisitor, AttributeListSyntax>? AttributeListVisit;
    public event Action<CSharpNodeVisitor, AttributeTargetSpecifierSyntax>? AttributeTargetSpecifierVisit;
    public event Action<CSharpNodeVisitor, AttributeSyntax>? AttributeVisit;
    public event Action<CSharpNodeVisitor, AttributeArgumentListSyntax>? AttributeArgumentListVisit;
    public event Action<CSharpNodeVisitor, AttributeArgumentSyntax>? AttributeArgumentVisit;
    public event Action<CSharpNodeVisitor, NameEqualsSyntax>? NameEqualsVisit;
    public event Action<CSharpNodeVisitor, TypeParameterListSyntax>? TypeParameterListVisit;
    public event Action<CSharpNodeVisitor, TypeParameterSyntax>? TypeParameterVisit;
    public event Action<CSharpNodeVisitor, ClassDeclarationSyntax>? ClassDeclarationVisit;
    public event Action<CSharpNodeVisitor, StructDeclarationSyntax>? StructDeclarationVisit;
    public event Action<CSharpNodeVisitor, InterfaceDeclarationSyntax>? InterfaceDeclarationVisit;
    public event Action<CSharpNodeVisitor, RecordDeclarationSyntax>? RecordDeclarationVisit;
    public event Action<CSharpNodeVisitor, EnumDeclarationSyntax>? EnumDeclarationVisit;
    public event Action<CSharpNodeVisitor, DelegateDeclarationSyntax>? DelegateDeclarationVisit;
    public event Action<CSharpNodeVisitor, EnumMemberDeclarationSyntax>? EnumMemberDeclarationVisit;
    public event Action<CSharpNodeVisitor, BaseListSyntax>? BaseListVisit;
    public event Action<CSharpNodeVisitor, SimpleBaseTypeSyntax>? SimpleBaseTypeVisit;
    public event Action<CSharpNodeVisitor, PrimaryConstructorBaseTypeSyntax>? PrimaryConstructorBaseTypeVisit;
    public event Action<CSharpNodeVisitor, TypeParameterConstraintClauseSyntax>? TypeParameterConstraintClauseVisit;
    public event Action<CSharpNodeVisitor, ConstructorConstraintSyntax>? ConstructorConstraintVisit;
    public event Action<CSharpNodeVisitor, ClassOrStructConstraintSyntax>? ClassOrStructConstraintVisit;
    public event Action<CSharpNodeVisitor, TypeConstraintSyntax>? TypeConstraintVisit;
    public event Action<CSharpNodeVisitor, DefaultConstraintSyntax>? DefaultConstraintVisit;
    public event Action<CSharpNodeVisitor, FieldDeclarationSyntax>? FieldDeclarationVisit;
    public event Action<CSharpNodeVisitor, EventFieldDeclarationSyntax>? EventFieldDeclarationVisit;
    public event Action<CSharpNodeVisitor, ExplicitInterfaceSpecifierSyntax>? ExplicitInterfaceSpecifierVisit;
    public event Action<CSharpNodeVisitor, MethodDeclarationSyntax>? MethodDeclarationVisit;
    public event Action<CSharpNodeVisitor, OperatorDeclarationSyntax>? OperatorDeclarationVisit;
    public event Action<CSharpNodeVisitor, ConversionOperatorDeclarationSyntax>? ConversionOperatorDeclarationVisit;
    public event Action<CSharpNodeVisitor, ConstructorDeclarationSyntax>? ConstructorDeclarationVisit;
    public event Action<CSharpNodeVisitor, ConstructorInitializerSyntax>? ConstructorInitializerVisit;
    public event Action<CSharpNodeVisitor, DestructorDeclarationSyntax>? DestructorDeclarationVisit;
    public event Action<CSharpNodeVisitor, PropertyDeclarationSyntax>? PropertyDeclarationVisit;
    public event Action<CSharpNodeVisitor, ArrowExpressionClauseSyntax>? ArrowExpressionClauseVisit;
    public event Action<CSharpNodeVisitor, EventDeclarationSyntax>? EventDeclarationVisit;
    public event Action<CSharpNodeVisitor, IndexerDeclarationSyntax>? IndexerDeclarationVisit;
    public event Action<CSharpNodeVisitor, AccessorListSyntax>? AccessorListVisit;
    public event Action<CSharpNodeVisitor, AccessorDeclarationSyntax>? AccessorDeclarationVisit;
    public event Action<CSharpNodeVisitor, ParameterListSyntax>? ParameterListVisit;
    public event Action<CSharpNodeVisitor, BracketedParameterListSyntax>? BracketedParameterListVisit;
    public event Action<CSharpNodeVisitor, ParameterSyntax>? ParameterVisit;
    public event Action<CSharpNodeVisitor, FunctionPointerParameterSyntax>? FunctionPointerParameterVisit;
    public event Action<CSharpNodeVisitor, IncompleteMemberSyntax>? IncompleteMemberVisit;
    public event Action<CSharpNodeVisitor, SkippedTokensTriviaSyntax>? SkippedTokensTriviaVisit;
    public event Action<CSharpNodeVisitor, DocumentationCommentTriviaSyntax>? DocumentationCommentTriviaVisit;
    public event Action<CSharpNodeVisitor, TypeCrefSyntax>? TypeCrefVisit;
    public event Action<CSharpNodeVisitor, QualifiedCrefSyntax>? QualifiedCrefVisit;
    public event Action<CSharpNodeVisitor, NameMemberCrefSyntax>? NameMemberCrefVisit;
    public event Action<CSharpNodeVisitor, IndexerMemberCrefSyntax>? IndexerMemberCrefVisit;
    public event Action<CSharpNodeVisitor, OperatorMemberCrefSyntax>? OperatorMemberCrefVisit;
    public event Action<CSharpNodeVisitor, ConversionOperatorMemberCrefSyntax>? ConversionOperatorMemberCrefVisit;
    public event Action<CSharpNodeVisitor, CrefParameterListSyntax>? CrefParameterListVisit;
    public event Action<CSharpNodeVisitor, CrefBracketedParameterListSyntax>? CrefBracketedParameterListVisit;
    public event Action<CSharpNodeVisitor, CrefParameterSyntax>? CrefParameterVisit;
    public event Action<CSharpNodeVisitor, XmlElementSyntax>? XmlElementVisit;
    public event Action<CSharpNodeVisitor, XmlElementStartTagSyntax>? XmlElementStartTagVisit;
    public event Action<CSharpNodeVisitor, XmlElementEndTagSyntax>? XmlElementEndTagVisit;
    public event Action<CSharpNodeVisitor, XmlEmptyElementSyntax>? XmlEmptyElementVisit;
    public event Action<CSharpNodeVisitor, XmlNameSyntax>? XmlNameVisit;
    public event Action<CSharpNodeVisitor, XmlPrefixSyntax>? XmlPrefixVisit;
    public event Action<CSharpNodeVisitor, XmlTextAttributeSyntax>? XmlTextAttributeVisit;
    public event Action<CSharpNodeVisitor, XmlCrefAttributeSyntax>? XmlCrefAttributeVisit;
    public event Action<CSharpNodeVisitor, XmlNameAttributeSyntax>? XmlNameAttributeVisit;
    public event Action<CSharpNodeVisitor, XmlTextSyntax>? XmlTextVisit;
    public event Action<CSharpNodeVisitor, XmlCDataSectionSyntax>? XmlCDataSectionVisit;
    public event Action<CSharpNodeVisitor, XmlProcessingInstructionSyntax>? XmlProcessingInstructionVisit;
    public event Action<CSharpNodeVisitor, XmlCommentSyntax>? XmlCommentVisit;
    public event Action<CSharpNodeVisitor, IfDirectiveTriviaSyntax>? IfDirectiveTriviaVisit;
    public event Action<CSharpNodeVisitor, ElifDirectiveTriviaSyntax>? ElifDirectiveTriviaVisit;
    public event Action<CSharpNodeVisitor, ElseDirectiveTriviaSyntax>? ElseDirectiveTriviaVisit;
    public event Action<CSharpNodeVisitor, EndIfDirectiveTriviaSyntax>? EndIfDirectiveTriviaVisit;
    public event Action<CSharpNodeVisitor, RegionDirectiveTriviaSyntax>? RegionDirectiveTriviaVisit;
    public event Action<CSharpNodeVisitor, EndRegionDirectiveTriviaSyntax>? EndRegionDirectiveTriviaVisit;
    public event Action<CSharpNodeVisitor, ErrorDirectiveTriviaSyntax>? ErrorDirectiveTriviaVisit;
    public event Action<CSharpNodeVisitor, WarningDirectiveTriviaSyntax>? WarningDirectiveTriviaVisit;
    public event Action<CSharpNodeVisitor, BadDirectiveTriviaSyntax>? BadDirectiveTriviaVisit;
    public event Action<CSharpNodeVisitor, DefineDirectiveTriviaSyntax>? DefineDirectiveTriviaVisit;
    public event Action<CSharpNodeVisitor, UndefDirectiveTriviaSyntax>? UndefDirectiveTriviaVisit;
    public event Action<CSharpNodeVisitor, LineDirectiveTriviaSyntax>? LineDirectiveTriviaVisit;
    public event Action<CSharpNodeVisitor, LineDirectivePositionSyntax>? LineDirectivePositionVisit;
    public event Action<CSharpNodeVisitor, LineSpanDirectiveTriviaSyntax>? LineSpanDirectiveTriviaVisit;
    public event Action<CSharpNodeVisitor, PragmaWarningDirectiveTriviaSyntax>? PragmaWarningDirectiveTriviaVisit;
    public event Action<CSharpNodeVisitor, PragmaChecksumDirectiveTriviaSyntax>? PragmaChecksumDirectiveTriviaVisit;
    public event Action<CSharpNodeVisitor, ReferenceDirectiveTriviaSyntax>? ReferenceDirectiveTriviaVisit;
    public event Action<CSharpNodeVisitor, LoadDirectiveTriviaSyntax>? LoadDirectiveTriviaVisit;
    public event Action<CSharpNodeVisitor, ShebangDirectiveTriviaSyntax>? ShebangDirectiveTriviaVisit;
    public event Action<CSharpNodeVisitor, NullableDirectiveTriviaSyntax>? NullableDirectiveTriviaVisit;

    public class Walker : CSharpSyntaxWalker
    {
        private CSharpNodeVisitor _wrapper;

        public Walker(CSharpNodeVisitor wrapper)
        {
            _wrapper = wrapper;
        }

        public override void VisitIdentifierName(IdentifierNameSyntax node)
        {
            _wrapper.IdentifierNameVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitQualifiedName(QualifiedNameSyntax node)
        {
            _wrapper.QualifiedNameVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitGenericName(GenericNameSyntax node)
        {
            _wrapper.GenericNameVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitTypeArgumentList(TypeArgumentListSyntax node)
        {
            _wrapper.TypeArgumentListVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitAliasQualifiedName(AliasQualifiedNameSyntax node)
        {
            _wrapper.AliasQualifiedNameVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitPredefinedType(PredefinedTypeSyntax node)
        {
            _wrapper.PredefinedTypeVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitArrayType(ArrayTypeSyntax node)
        {
            _wrapper.ArrayTypeVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitArrayRankSpecifier(ArrayRankSpecifierSyntax node)
        {
            _wrapper.ArrayRankSpecifierVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitPointerType(PointerTypeSyntax node)
        {
            _wrapper.PointerTypeVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitFunctionPointerType(FunctionPointerTypeSyntax node)
        {
            _wrapper.FunctionPointerTypeVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitFunctionPointerParameterList(FunctionPointerParameterListSyntax node)
        {
            _wrapper.FunctionPointerParameterListVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitFunctionPointerCallingConvention(FunctionPointerCallingConventionSyntax node)
        {
            _wrapper.FunctionPointerCallingConventionVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitFunctionPointerUnmanagedCallingConventionList(FunctionPointerUnmanagedCallingConventionListSyntax node)
        {
            _wrapper.FunctionPointerUnmanagedCallingConventionListVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitFunctionPointerUnmanagedCallingConvention(FunctionPointerUnmanagedCallingConventionSyntax node)
        {
            _wrapper.FunctionPointerUnmanagedCallingConventionVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitNullableType(NullableTypeSyntax node)
        {
            _wrapper.NullableTypeVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitTupleType(TupleTypeSyntax node)
        {
            _wrapper.TupleTypeVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitTupleElement(TupleElementSyntax node)
        {
            _wrapper.TupleElementVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitOmittedTypeArgument(OmittedTypeArgumentSyntax node)
        {
            _wrapper.OmittedTypeArgumentVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitRefType(RefTypeSyntax node)
        {
            _wrapper.RefTypeVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitScopedType(ScopedTypeSyntax node)
        {
            _wrapper.ScopedTypeVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitParenthesizedExpression(ParenthesizedExpressionSyntax node)
        {
            _wrapper.ParenthesizedExpressionVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitTupleExpression(TupleExpressionSyntax node)
        {
            _wrapper.TupleExpressionVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitPrefixUnaryExpression(PrefixUnaryExpressionSyntax node)
        {
            _wrapper.PrefixUnaryExpressionVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitAwaitExpression(AwaitExpressionSyntax node)
        {
            _wrapper.AwaitExpressionVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitPostfixUnaryExpression(PostfixUnaryExpressionSyntax node)
        {
            _wrapper.PostfixUnaryExpressionVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
        {
            _wrapper.MemberAccessExpressionVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitConditionalAccessExpression(ConditionalAccessExpressionSyntax node)
        {
            _wrapper.ConditionalAccessExpressionVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitMemberBindingExpression(MemberBindingExpressionSyntax node)
        {
            _wrapper.MemberBindingExpressionVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitElementBindingExpression(ElementBindingExpressionSyntax node)
        {
            _wrapper.ElementBindingExpressionVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitRangeExpression(RangeExpressionSyntax node)
        {
            _wrapper.RangeExpressionVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitImplicitElementAccess(ImplicitElementAccessSyntax node)
        {
            _wrapper.ImplicitElementAccessVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitBinaryExpression(BinaryExpressionSyntax node)
        {
            _wrapper.BinaryExpressionVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitAssignmentExpression(AssignmentExpressionSyntax node)
        {
            _wrapper.AssignmentExpressionVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitConditionalExpression(ConditionalExpressionSyntax node)
        {
            _wrapper.ConditionalExpressionVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitThisExpression(ThisExpressionSyntax node)
        {
            _wrapper.ThisExpressionVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitBaseExpression(BaseExpressionSyntax node)
        {
            _wrapper.BaseExpressionVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitLiteralExpression(LiteralExpressionSyntax node)
        {
            _wrapper.LiteralExpressionVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitMakeRefExpression(MakeRefExpressionSyntax node)
        {
            _wrapper.MakeRefExpressionVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitRefTypeExpression(RefTypeExpressionSyntax node)
        {
            _wrapper.RefTypeExpressionVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitRefValueExpression(RefValueExpressionSyntax node)
        {
            _wrapper.RefValueExpressionVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitCheckedExpression(CheckedExpressionSyntax node)
        {
            _wrapper.CheckedExpressionVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitDefaultExpression(DefaultExpressionSyntax node)
        {
            _wrapper.DefaultExpressionVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitTypeOfExpression(TypeOfExpressionSyntax node)
        {
            _wrapper.TypeOfExpressionVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitSizeOfExpression(SizeOfExpressionSyntax node)
        {
            _wrapper.SizeOfExpressionVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            _wrapper.InvocationExpressionVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitElementAccessExpression(ElementAccessExpressionSyntax node)
        {
            _wrapper.ElementAccessExpressionVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitArgumentList(ArgumentListSyntax node)
        {
            _wrapper.ArgumentListVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitBracketedArgumentList(BracketedArgumentListSyntax node)
        {
            _wrapper.BracketedArgumentListVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitArgument(ArgumentSyntax node)
        {
            _wrapper.ArgumentVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitExpressionColon(ExpressionColonSyntax node)
        {
            _wrapper.ExpressionColonVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitNameColon(NameColonSyntax node)
        {
            _wrapper.NameColonVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitDeclarationExpression(DeclarationExpressionSyntax node)
        {
            _wrapper.DeclarationExpressionVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitCastExpression(CastExpressionSyntax node)
        {
            _wrapper.CastExpressionVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitAnonymousMethodExpression(AnonymousMethodExpressionSyntax node)
        {
            _wrapper.AnonymousMethodExpressionVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitSimpleLambdaExpression(SimpleLambdaExpressionSyntax node)
        {
            _wrapper.SimpleLambdaExpressionVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitRefExpression(RefExpressionSyntax node)
        {
            _wrapper.RefExpressionVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitParenthesizedLambdaExpression(ParenthesizedLambdaExpressionSyntax node)
        {
            _wrapper.ParenthesizedLambdaExpressionVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitInitializerExpression(InitializerExpressionSyntax node)
        {
            _wrapper.InitializerExpressionVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitImplicitObjectCreationExpression(ImplicitObjectCreationExpressionSyntax node)
        {
            _wrapper.ImplicitObjectCreationExpressionVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
        {
            _wrapper.ObjectCreationExpressionVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitWithExpression(WithExpressionSyntax node)
        {
            _wrapper.WithExpressionVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitAnonymousObjectMemberDeclarator(AnonymousObjectMemberDeclaratorSyntax node)
        {
            _wrapper.AnonymousObjectMemberDeclaratorVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitAnonymousObjectCreationExpression(AnonymousObjectCreationExpressionSyntax node)
        {
            _wrapper.AnonymousObjectCreationExpressionVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitArrayCreationExpression(ArrayCreationExpressionSyntax node)
        {
            _wrapper.ArrayCreationExpressionVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitImplicitArrayCreationExpression(ImplicitArrayCreationExpressionSyntax node)
        {
            _wrapper.ImplicitArrayCreationExpressionVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitStackAllocArrayCreationExpression(StackAllocArrayCreationExpressionSyntax node)
        {
            _wrapper.StackAllocArrayCreationExpressionVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitImplicitStackAllocArrayCreationExpression(ImplicitStackAllocArrayCreationExpressionSyntax node)
        {
            _wrapper.ImplicitStackAllocArrayCreationExpressionVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitQueryExpression(QueryExpressionSyntax node)
        {
            _wrapper.QueryExpressionVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitQueryBody(QueryBodySyntax node)
        {
            _wrapper.QueryBodyVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitFromClause(FromClauseSyntax node)
        {
            _wrapper.FromClauseVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitLetClause(LetClauseSyntax node)
        {
            _wrapper.LetClauseVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitJoinClause(JoinClauseSyntax node)
        {
            _wrapper.JoinClauseVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitJoinIntoClause(JoinIntoClauseSyntax node)
        {
            _wrapper.JoinIntoClauseVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitWhereClause(WhereClauseSyntax node)
        {
            _wrapper.WhereClauseVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitOrderByClause(OrderByClauseSyntax node)
        {
            _wrapper.OrderByClauseVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitOrdering(OrderingSyntax node)
        {
            _wrapper.OrderingVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitSelectClause(SelectClauseSyntax node)
        {
            _wrapper.SelectClauseVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitGroupClause(GroupClauseSyntax node)
        {
            _wrapper.GroupClauseVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitQueryContinuation(QueryContinuationSyntax node)
        {
            _wrapper.QueryContinuationVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitOmittedArraySizeExpression(OmittedArraySizeExpressionSyntax node)
        {
            _wrapper.OmittedArraySizeExpressionVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitInterpolatedStringExpression(InterpolatedStringExpressionSyntax node)
        {
            _wrapper.InterpolatedStringExpressionVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitIsPatternExpression(IsPatternExpressionSyntax node)
        {
            _wrapper.IsPatternExpressionVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitThrowExpression(ThrowExpressionSyntax node)
        {
            _wrapper.ThrowExpressionVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitWhenClause(WhenClauseSyntax node)
        {
            _wrapper.WhenClauseVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitDiscardPattern(DiscardPatternSyntax node)
        {
            _wrapper.DiscardPatternVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitDeclarationPattern(DeclarationPatternSyntax node)
        {
            _wrapper.DeclarationPatternVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitVarPattern(VarPatternSyntax node)
        {
            _wrapper.VarPatternVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitRecursivePattern(RecursivePatternSyntax node)
        {
            _wrapper.RecursivePatternVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitPositionalPatternClause(PositionalPatternClauseSyntax node)
        {
            _wrapper.PositionalPatternClauseVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitPropertyPatternClause(PropertyPatternClauseSyntax node)
        {
            _wrapper.PropertyPatternClauseVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitSubpattern(SubpatternSyntax node)
        {
            _wrapper.SubpatternVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitConstantPattern(ConstantPatternSyntax node)
        {
            _wrapper.ConstantPatternVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitParenthesizedPattern(ParenthesizedPatternSyntax node)
        {
            _wrapper.ParenthesizedPatternVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitRelationalPattern(RelationalPatternSyntax node)
        {
            _wrapper.RelationalPatternVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitTypePattern(TypePatternSyntax node)
        {
            _wrapper.TypePatternVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitBinaryPattern(BinaryPatternSyntax node)
        {
            _wrapper.BinaryPatternVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitUnaryPattern(UnaryPatternSyntax node)
        {
            _wrapper.UnaryPatternVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitListPattern(ListPatternSyntax node)
        {
            _wrapper.ListPatternVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitSlicePattern(SlicePatternSyntax node)
        {
            _wrapper.SlicePatternVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitInterpolatedStringText(InterpolatedStringTextSyntax node)
        {
            _wrapper.InterpolatedStringTextVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitInterpolation(InterpolationSyntax node)
        {
            _wrapper.InterpolationVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitInterpolationAlignmentClause(InterpolationAlignmentClauseSyntax node)
        {
            _wrapper.InterpolationAlignmentClauseVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitInterpolationFormatClause(InterpolationFormatClauseSyntax node)
        {
            _wrapper.InterpolationFormatClauseVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitGlobalStatement(GlobalStatementSyntax node)
        {
            _wrapper.GlobalStatementVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitBlock(BlockSyntax node)
        {
            _wrapper.BlockVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitLocalFunctionStatement(LocalFunctionStatementSyntax node)
        {
            _wrapper.LocalFunctionStatementVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
        {
            _wrapper.LocalDeclarationStatementVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitVariableDeclaration(VariableDeclarationSyntax node)
        {
            _wrapper.VariableDeclarationVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitVariableDeclarator(VariableDeclaratorSyntax node)
        {
            _wrapper.VariableDeclaratorVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitEqualsValueClause(EqualsValueClauseSyntax node)
        {
            _wrapper.EqualsValueClauseVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitSingleVariableDesignation(SingleVariableDesignationSyntax node)
        {
            _wrapper.SingleVariableDesignationVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitDiscardDesignation(DiscardDesignationSyntax node)
        {
            _wrapper.DiscardDesignationVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitParenthesizedVariableDesignation(ParenthesizedVariableDesignationSyntax node)
        {
            _wrapper.ParenthesizedVariableDesignationVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitExpressionStatement(ExpressionStatementSyntax node)
        {
            _wrapper.ExpressionStatementVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitEmptyStatement(EmptyStatementSyntax node)
        {
            _wrapper.EmptyStatementVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitLabeledStatement(LabeledStatementSyntax node)
        {
            _wrapper.LabeledStatementVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitGotoStatement(GotoStatementSyntax node)
        {
            _wrapper.GotoStatementVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitBreakStatement(BreakStatementSyntax node)
        {
            _wrapper.BreakStatementVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitContinueStatement(ContinueStatementSyntax node)
        {
            _wrapper.ContinueStatementVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitReturnStatement(ReturnStatementSyntax node)
        {
            _wrapper.ReturnStatementVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitThrowStatement(ThrowStatementSyntax node)
        {
            _wrapper.ThrowStatementVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitYieldStatement(YieldStatementSyntax node)
        {
            _wrapper.YieldStatementVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitWhileStatement(WhileStatementSyntax node)
        {
            _wrapper.WhileStatementVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitDoStatement(DoStatementSyntax node)
        {
            _wrapper.DoStatementVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitForStatement(ForStatementSyntax node)
        {
            _wrapper.ForStatementVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitForEachStatement(ForEachStatementSyntax node)
        {
            _wrapper.ForEachStatementVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitForEachVariableStatement(ForEachVariableStatementSyntax node)
        {
            _wrapper.ForEachVariableStatementVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitUsingStatement(UsingStatementSyntax node)
        {
            _wrapper.UsingStatementVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitFixedStatement(FixedStatementSyntax node)
        {
            _wrapper.FixedStatementVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitCheckedStatement(CheckedStatementSyntax node)
        {
            _wrapper.CheckedStatementVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitUnsafeStatement(UnsafeStatementSyntax node)
        {
            _wrapper.UnsafeStatementVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitLockStatement(LockStatementSyntax node)
        {
            _wrapper.LockStatementVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitIfStatement(IfStatementSyntax node)
        {
            _wrapper.IfStatementVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitElseClause(ElseClauseSyntax node)
        {
            _wrapper.ElseClauseVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitSwitchStatement(SwitchStatementSyntax node)
        {
            _wrapper.SwitchStatementVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitSwitchSection(SwitchSectionSyntax node)
        {
            _wrapper.SwitchSectionVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitCasePatternSwitchLabel(CasePatternSwitchLabelSyntax node)
        {
            _wrapper.CasePatternSwitchLabelVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitCaseSwitchLabel(CaseSwitchLabelSyntax node)
        {
            _wrapper.CaseSwitchLabelVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitDefaultSwitchLabel(DefaultSwitchLabelSyntax node)
        {
            _wrapper.DefaultSwitchLabelVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitSwitchExpression(SwitchExpressionSyntax node)
        {
            _wrapper.SwitchExpressionVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitSwitchExpressionArm(SwitchExpressionArmSyntax node)
        {
            _wrapper.SwitchExpressionArmVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitTryStatement(TryStatementSyntax node)
        {
            _wrapper.TryStatementVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitCatchClause(CatchClauseSyntax node)
        {
            _wrapper.CatchClauseVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitCatchDeclaration(CatchDeclarationSyntax node)
        {
            _wrapper.CatchDeclarationVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitCatchFilterClause(CatchFilterClauseSyntax node)
        {
            _wrapper.CatchFilterClauseVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitFinallyClause(FinallyClauseSyntax node)
        {
            _wrapper.FinallyClauseVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitCompilationUnit(CompilationUnitSyntax node)
        {
            _wrapper.CompilationUnitVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitExternAliasDirective(ExternAliasDirectiveSyntax node)
        {
            _wrapper.ExternAliasDirectiveVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitUsingDirective(UsingDirectiveSyntax node)
        {
            _wrapper.UsingDirectiveVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
        {
            _wrapper.NamespaceDeclarationVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitFileScopedNamespaceDeclaration(FileScopedNamespaceDeclarationSyntax node)
        {
            _wrapper.FileScopedNamespaceDeclarationVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitAttributeList(AttributeListSyntax node)
        {
            _wrapper.AttributeListVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitAttributeTargetSpecifier(AttributeTargetSpecifierSyntax node)
        {
            _wrapper.AttributeTargetSpecifierVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitAttribute(AttributeSyntax node)
        {
            _wrapper.AttributeVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitAttributeArgumentList(AttributeArgumentListSyntax node)
        {
            _wrapper.AttributeArgumentListVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitAttributeArgument(AttributeArgumentSyntax node)
        {
            _wrapper.AttributeArgumentVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitNameEquals(NameEqualsSyntax node)
        {
            _wrapper.NameEqualsVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitTypeParameterList(TypeParameterListSyntax node)
        {
            _wrapper.TypeParameterListVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitTypeParameter(TypeParameterSyntax node)
        {
            _wrapper.TypeParameterVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            _wrapper.ClassDeclarationVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitStructDeclaration(StructDeclarationSyntax node)
        {
            _wrapper.StructDeclarationVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            _wrapper.InterfaceDeclarationVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitRecordDeclaration(RecordDeclarationSyntax node)
        {
            _wrapper.RecordDeclarationVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitEnumDeclaration(EnumDeclarationSyntax node)
        {
            _wrapper.EnumDeclarationVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitDelegateDeclaration(DelegateDeclarationSyntax node)
        {
            _wrapper.DelegateDeclarationVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitEnumMemberDeclaration(EnumMemberDeclarationSyntax node)
        {
            _wrapper.EnumMemberDeclarationVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitBaseList(BaseListSyntax node)
        {
            _wrapper.BaseListVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitSimpleBaseType(SimpleBaseTypeSyntax node)
        {
            _wrapper.SimpleBaseTypeVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitPrimaryConstructorBaseType(PrimaryConstructorBaseTypeSyntax node)
        {
            _wrapper.PrimaryConstructorBaseTypeVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitTypeParameterConstraintClause(TypeParameterConstraintClauseSyntax node)
        {
            _wrapper.TypeParameterConstraintClauseVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitConstructorConstraint(ConstructorConstraintSyntax node)
        {
            _wrapper.ConstructorConstraintVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitClassOrStructConstraint(ClassOrStructConstraintSyntax node)
        {
            _wrapper.ClassOrStructConstraintVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitTypeConstraint(TypeConstraintSyntax node)
        {
            _wrapper.TypeConstraintVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitDefaultConstraint(DefaultConstraintSyntax node)
        {
            _wrapper.DefaultConstraintVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            _wrapper.FieldDeclarationVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitEventFieldDeclaration(EventFieldDeclarationSyntax node)
        {
            _wrapper.EventFieldDeclarationVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitExplicitInterfaceSpecifier(ExplicitInterfaceSpecifierSyntax node)
        {
            _wrapper.ExplicitInterfaceSpecifierVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            _wrapper.MethodDeclarationVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitOperatorDeclaration(OperatorDeclarationSyntax node)
        {
            _wrapper.OperatorDeclarationVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitConversionOperatorDeclaration(ConversionOperatorDeclarationSyntax node)
        {
            _wrapper.ConversionOperatorDeclarationVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            _wrapper.ConstructorDeclarationVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitConstructorInitializer(ConstructorInitializerSyntax node)
        {
            _wrapper.ConstructorInitializerVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitDestructorDeclaration(DestructorDeclarationSyntax node)
        {
            _wrapper.DestructorDeclarationVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            _wrapper.PropertyDeclarationVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitArrowExpressionClause(ArrowExpressionClauseSyntax node)
        {
            _wrapper.ArrowExpressionClauseVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitEventDeclaration(EventDeclarationSyntax node)
        {
            _wrapper.EventDeclarationVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitIndexerDeclaration(IndexerDeclarationSyntax node)
        {
            _wrapper.IndexerDeclarationVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitAccessorList(AccessorListSyntax node)
        {
            _wrapper.AccessorListVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitAccessorDeclaration(AccessorDeclarationSyntax node)
        {
            _wrapper.AccessorDeclarationVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitParameterList(ParameterListSyntax node)
        {
            _wrapper.ParameterListVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitBracketedParameterList(BracketedParameterListSyntax node)
        {
            _wrapper.BracketedParameterListVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitParameter(ParameterSyntax node)
        {
            _wrapper.ParameterVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitFunctionPointerParameter(FunctionPointerParameterSyntax node)
        {
            _wrapper.FunctionPointerParameterVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitIncompleteMember(IncompleteMemberSyntax node)
        {
            _wrapper.IncompleteMemberVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitSkippedTokensTrivia(SkippedTokensTriviaSyntax node)
        {
            _wrapper.SkippedTokensTriviaVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitDocumentationCommentTrivia(DocumentationCommentTriviaSyntax node)
        {
            _wrapper.DocumentationCommentTriviaVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitTypeCref(TypeCrefSyntax node)
        {
            _wrapper.TypeCrefVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitQualifiedCref(QualifiedCrefSyntax node)
        {
            _wrapper.QualifiedCrefVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitNameMemberCref(NameMemberCrefSyntax node)
        {
            _wrapper.NameMemberCrefVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitIndexerMemberCref(IndexerMemberCrefSyntax node)
        {
            _wrapper.IndexerMemberCrefVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitOperatorMemberCref(OperatorMemberCrefSyntax node)
        {
            _wrapper.OperatorMemberCrefVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitConversionOperatorMemberCref(ConversionOperatorMemberCrefSyntax node)
        {
            _wrapper.ConversionOperatorMemberCrefVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitCrefParameterList(CrefParameterListSyntax node)
        {
            _wrapper.CrefParameterListVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitCrefBracketedParameterList(CrefBracketedParameterListSyntax node)
        {
            _wrapper.CrefBracketedParameterListVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitCrefParameter(CrefParameterSyntax node)
        {
            _wrapper.CrefParameterVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitXmlElement(XmlElementSyntax node)
        {
            _wrapper.XmlElementVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitXmlElementStartTag(XmlElementStartTagSyntax node)
        {
            _wrapper.XmlElementStartTagVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitXmlElementEndTag(XmlElementEndTagSyntax node)
        {
            _wrapper.XmlElementEndTagVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitXmlEmptyElement(XmlEmptyElementSyntax node)
        {
            _wrapper.XmlEmptyElementVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitXmlName(XmlNameSyntax node)
        {
            _wrapper.XmlNameVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitXmlPrefix(XmlPrefixSyntax node)
        {
            _wrapper.XmlPrefixVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitXmlTextAttribute(XmlTextAttributeSyntax node)
        {
            _wrapper.XmlTextAttributeVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitXmlCrefAttribute(XmlCrefAttributeSyntax node)
        {
            _wrapper.XmlCrefAttributeVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitXmlNameAttribute(XmlNameAttributeSyntax node)
        {
            _wrapper.XmlNameAttributeVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitXmlText(XmlTextSyntax node)
        {
            _wrapper.XmlTextVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitXmlCDataSection(XmlCDataSectionSyntax node)
        {
            _wrapper.XmlCDataSectionVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitXmlProcessingInstruction(XmlProcessingInstructionSyntax node)
        {
            _wrapper.XmlProcessingInstructionVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitXmlComment(XmlCommentSyntax node)
        {
            _wrapper.XmlCommentVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitIfDirectiveTrivia(IfDirectiveTriviaSyntax node)
        {
            _wrapper.IfDirectiveTriviaVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitElifDirectiveTrivia(ElifDirectiveTriviaSyntax node)
        {
            _wrapper.ElifDirectiveTriviaVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitElseDirectiveTrivia(ElseDirectiveTriviaSyntax node)
        {
            _wrapper.ElseDirectiveTriviaVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitEndIfDirectiveTrivia(EndIfDirectiveTriviaSyntax node)
        {
            _wrapper.EndIfDirectiveTriviaVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitRegionDirectiveTrivia(RegionDirectiveTriviaSyntax node)
        {
            _wrapper.RegionDirectiveTriviaVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitEndRegionDirectiveTrivia(EndRegionDirectiveTriviaSyntax node)
        {
            _wrapper.EndRegionDirectiveTriviaVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitErrorDirectiveTrivia(ErrorDirectiveTriviaSyntax node)
        {
            _wrapper.ErrorDirectiveTriviaVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitWarningDirectiveTrivia(WarningDirectiveTriviaSyntax node)
        {
            _wrapper.WarningDirectiveTriviaVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitBadDirectiveTrivia(BadDirectiveTriviaSyntax node)
        {
            _wrapper.BadDirectiveTriviaVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitDefineDirectiveTrivia(DefineDirectiveTriviaSyntax node)
        {
            _wrapper.DefineDirectiveTriviaVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitUndefDirectiveTrivia(UndefDirectiveTriviaSyntax node)
        {
            _wrapper.UndefDirectiveTriviaVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitLineDirectiveTrivia(LineDirectiveTriviaSyntax node)
        {
            _wrapper.LineDirectiveTriviaVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitLineDirectivePosition(LineDirectivePositionSyntax node)
        {
            _wrapper.LineDirectivePositionVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitLineSpanDirectiveTrivia(LineSpanDirectiveTriviaSyntax node)
        {
            _wrapper.LineSpanDirectiveTriviaVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitPragmaWarningDirectiveTrivia(PragmaWarningDirectiveTriviaSyntax node)
        {
            _wrapper.PragmaWarningDirectiveTriviaVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitPragmaChecksumDirectiveTrivia(PragmaChecksumDirectiveTriviaSyntax node)
        {
            _wrapper.PragmaChecksumDirectiveTriviaVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitReferenceDirectiveTrivia(ReferenceDirectiveTriviaSyntax node)
        {
            _wrapper.ReferenceDirectiveTriviaVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitLoadDirectiveTrivia(LoadDirectiveTriviaSyntax node)
        {
            _wrapper.LoadDirectiveTriviaVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitShebangDirectiveTrivia(ShebangDirectiveTriviaSyntax node)
        {
            _wrapper.ShebangDirectiveTriviaVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void VisitNullableDirectiveTrivia(NullableDirectiveTriviaSyntax node)
        {
            _wrapper.NullableDirectiveTriviaVisit?.Invoke(_wrapper, node);
            base.DefaultVisit(node);
        }

        public override void Visit(SyntaxNode? node)
        {
            if (node == null)
                return;

            var cancelToken = new NodeVisitorToken();
            _wrapper.OnBeforeNodeVisit(node, ref cancelToken);
            if (!cancelToken.IsCanceled)
            {
                base.Visit(node);
                _wrapper.OnAfterNodeVisit(node);
            }
        }
    }
}
