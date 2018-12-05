BUILD
=====

Command line:
* Download nuget[1] and put it in a PATH visible folder, like 
	C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\
* Run build-release.cmd

[1] https://www.nuget.org/downloads, Windows x86 Commandline

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
    TypeOfExpression
    ArrayType
    GenericName
    IdentifierName
    NullableType
    OmittedTypeArgument
    PredefinedType
    RefType
	
Unsupported
    RefExpression // ref int i2 = ref i;
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