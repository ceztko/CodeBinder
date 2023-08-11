
# Syntax

CodeBinder is a tool that transpiles C# projects in several languages/frameworks, specifically aimed for wrappers to native libraries using .NET native interop.  

Usage: `CodeBinder [OPTIONS]`

Options:
```
Options:
  -p, --project=VALUE        The project to be converted
  -s, --solution=VALUE       The solution to be converted
  -m, --nsmapping=VALUE      Mapping for the given, must be colon separated ns1:
                               ns2
  -l, --language=VALUE       The target language for the conversion
  -t, --targetpath=VALUE       The target root path for the conversion
  -h, --help                 Show this message and exit
      --interface-only       Only output public interface (CLang)
      --android              Output is compatible with android sdk (Java)
      --commonjs             Output is CommonJS compatible (TypeScript)
      --create-template      Create template project and definitions (NativeAOT)
```
Example:

```
CodeBinder --project=project.csproj --language=Java --namespace=Library:com.library --targetpath=D:\target\java
```

# Build

Command line:
* Run `build-release.cmd` (or `build-release.ps1`) to build the solution. Executables will be found in `bin/{Debug|Release}` folders

# Example test project

An example test project/solution can be found in `Test/CodeBinder.Test.sln`. You can generate code out of this project in two ways:

* Run `do-test-codegen.cmd` (or `do-test-codegen.ps1`) to build the solution and run the sample code gen;
* Execute the `Sample` project in the `CodeBinder.sln` solution.

Generated code will be found in `../CodeBinder-TestCodeGen`

# Language/syntax supported

## Supported languages

- Java/JNI (Android/SDK)
- ObjectiveC
- TypeScript (commonjs/ESModule, using NodeJS)

The following languages are supported only to create implementation templates (the definitions of the `DllImport` methods declared in the C# project being transpiled):

- C/C++
- NativeAOT (Experimental)

## Supported statements

- Block
- BreakStatement
- ForEachStatement
- ContinueStatement
- DoStatement
- EmptyStatement
- ExpressionStatement
- ForStatement
- IfStatement
- LocalDeclarationStatement
- LockStatement
- ReturnStatement
- SwitchStatement
- ThrowStatement
- TryStatement
- UsingStatement
- WhileStatement

## Unsupported statements

- ForEachVariableStatement
- CheckedStatement
- UnsafeStatement
- LabeledStatement
- GotoStatement
- FixedStatement
- LocalFunctionStatement
- YieldStatement

## Supported expressions

- ArrayCreationExpression
- OmittedArraySizeExpression
- AssignmentExpression
- BinaryExpression
- CastExpression
- ConditionalExpression
- ElementAccessExpression
- InitializerExpression
- BaseExpression
- ThisExpression
- InvocationExpression
- LiteralExpression
- MemberAccessExpression
- ObjectCreationExpression
- ParenthesizedExpression
- PostfixUnaryExpression
- PrefixUnaryExpression
- QualifiedName
- TypeOfExpression
- ArrayType
- GenericName
- IdentifierName
- NullableType
- OmittedTypeArgument
- PredefinedType
- RefType

## Unsupported expressions

- RefExpression (`ref int i2 = ref i;`)
- DeclarationExpression (out var declaration or a tuple deconstruction)
- ThrowExpression (https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/throw#the-throw-expression)
- DefaultExpression
- AnonymousMethodExpression
- ParenthesizedLambdaExpression
- SimpleLambdaExpression
- PointerType
- AliasQualifiedName (Except in attributes)
- RefValueExpression
- RefTypeExpression
- ImplicitArrayCreationExpression (`var names = new [] { "Bob", "Sam", "Jim", "Dan", "Mel" };`)
- ElementBindingExpression (`new Class() { Member = 1; }`)
- ImplicitElementAccess ???
- MemberBindingExpression
- SizeOfExpression
- MakeRefExpression
- ImplicitStackAllocArrayCreationExpression
- InterpolatedStringExpression
- AwaitExpression
- QueryExpression
- StackAllocArrayCreationExpression
- AnonymousObjectCreationExpression
- TupleType ???
- TupleExpression ???
- IsPatternExpression
- CheckedExpression
- ConditionalAccessExpression
