Statements
==========

Supported:
    Block
    BreakStatement
    ForEachStatement
    ContinueStatement
    DoStatement
    EmptyStatement
    ExpressionStatement
    ForStatement
    IfStatement
    LocalDeclarationStatement
    LockStatement
    ReturnStatement
    SwitchStatement
    ThrowStatement
    TryStatement
    UsingStatement
    WhileStatement

Unsupported:
    ForEachVariableStatement
    CheckedStatement
    UnsafeStatement
    LabeledStatement
    GotoStatement
    FixedStatement
    LocalFunctionStatement
    YieldStatement

Expressions
===========

Supported:
    ArrayCreationExpression
    OmittedArraySizeExpression	
    AssignmentExpression
    BinaryExpression
    CastExpression
    ConditionalExpression
    ElementAccessExpression
    InitializerExpression
    BaseExpression
    ThisExpression
    InvocationExpression
    LiteralExpression
    MemberAccessExpression
    ObjectCreationExpression
    ParenthesizedExpression
    PostfixUnaryExpression
    PrefixUnaryExpression
    QualifiedName
    RefExpression
    TypeOfExpression
    ArrayType
    GenericName
    IdentifierName
    NullableType
    OmittedTypeArgument
    PredefinedType
    RefType
	
Unsupported
    DeclarationExpression # out var declaration or a tuple deconstruction 
    ThrowExpression # https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/throw#the-throw-expression
    DefaultExpression
    AnonymousMethodExpression
    ParenthesizedLambdaExpression
    SimpleLambdaExpression
    PointerType
    AliasQualifiedName // Except in attributes
    RefValueExpression
    RefTypeExpression
    ImplicitArrayCreationExpression (var names = new [] { "Bob", "Sam", "Jim", "Dan", "Mel" }; )
    ElementBindingExpression (new Class() { Member = 1; })
	ImplicitElementAccess ???
	MemberBindingExpression	
	SizeOfExpression
    MakeRefExpression
    ImplicitStackAllocArrayCreationExpression
    InterpolatedStringExpression
    AwaitExpression
    QueryExpression
    StackAllocArrayCreationExpression
    AnonymousObjectCreationExpression
    TupleType ???
    TupleExpression ???
    IsPatternExpression
    CheckedExpression
    ConditionalAccessExpression