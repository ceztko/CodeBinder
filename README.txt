Syntax
======

Supported:
    BlockSyntax
    BreakStatementSyntax
    ForEachStatementSyntax
    ContinueStatementSyntax
    DoStatementSyntax
    EmptyStatementSyntax
    ExpressionStatementSyntax
    ForStatementSyntax
    IfStatementSyntax
    LocalDeclarationStatementSyntax
    LockStatementSyntax
    ReturnStatementSyntax
    SwitchStatementSyntax
    ThrowStatementSyntax
    TryStatementSyntax
    UsingStatementSyntax
    WhileStatementSyntax
    YieldStatementSyntax

Unsupported:
    ForEachVariableStatementSyntax
    CheckedStatementSyntax
    UnsafeStatementSyntax
    LabeledStatementSyntax
    GotoStatementSyntax
    FixedStatementSyntax
    LocalFunctionStatementSyntax

Expressions
===========

Supported:
    ArrayCreationExpressionSyntax
    OmittedArraySizeExpressionSyntax	
    AssignmentExpressionSyntax
    BinaryExpressionSyntax
    CastExpressionSyntax
    ConditionalExpressionSyntax
    ElementAccessExpressionSyntax
    InitializerExpressionSyntax
    BaseExpressionSyntax
    ThisExpressionSyntax
    InvocationExpressionSyntax
    LiteralExpressionSyntax
    MemberAccessExpressionSyntax
    ObjectCreationExpressionSyntax
    ParenthesizedExpressionSyntax
    PostfixUnaryExpressionSyntax
    PrefixUnaryExpressionSyntax
    RefExpressionSyntax
    TypeOfExpressionSyntax
    ArrayTypeSyntax
    GenericNameSyntax
    IdentifierNameSyntax
    NullableTypeSyntax
    OmittedTypeArgumentSyntax
    PredefinedTypeSyntax
    RefTypeSyntax
	
Unsupported
    DeclarationExpressionSyntax # out var declaration or a tuple deconstruction 
    ThrowExpressionSyntax # https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/throw#the-throw-expression
    QualifiedNameSyntax # Except in namespaces, using directives and attributes
    DefaultExpressionSyntax
    AnonymousMethodExpressionSyntax
    ParenthesizedLambdaExpressionSyntax
    SimpleLambdaExpressionSyntax
    PointerTypeSyntax
    AliasQualifiedNameSyntax
    RefValueExpressionSyntax
    RefTypeExpressionSyntax
    ImplicitArrayCreationExpressionSyntax (var names = new [] { "Bob", "Sam", "Jim", "Dan", "Mel" }; )
    ElementBindingExpressionSyntax (new Class() { Member = 1; })
	ImplicitElementAccessSyntax ???
	MemberBindingExpressionSyntax	
	SizeOfExpressionSyntax
    MakeRefExpressionSyntax
    ImplicitStackAllocArrayCreationExpressionSyntax
    InterpolatedStringExpressionSyntax
    AwaitExpressionSyntax
    QueryExpressionSyntax
    StackAllocArrayCreationExpressionSyntax
    AnonymousObjectCreationExpressionSyntax
    TupleTypeSyntax ???
    TupleExpressionSyntax ???
    IsPatternExpressionSyntax
    CheckedExpressionSyntax
    ConditionalAccessExpressionSyntax