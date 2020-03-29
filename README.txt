SYNTAX
======

Usage: CodeBinder [OPTIONS]

Options:
  -p, --project=VALUE        The project to be converted
  -s, --solution=VALUE       The solution to be converted
  -d, --def=VALUE            Preprocessor definition to be added during
                               conversion
  -n, --nodef=VALUE          Preprocessor definition to be removed during
                               conversion
  -m, --nsmapping=VALUE      Mapping for the given, must be colon separated ns1:
                               ns2
  -l, --language=VALUE       The target language for the conversion
  -r, --rootpath=VALUE       The target root path for the conversion
      --publiciface          Only output public interface (CLang)
  -h, --help                 Show this message and exit

Example:

CodeBinder --project=project.csproj --nodef=CSHARP --nodef=NET_FRAMEWORK --language=Java --namespace=com.library --rootpath=D:\target\java

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