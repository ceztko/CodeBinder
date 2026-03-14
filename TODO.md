# Codebase Review: C# to Java Conversion

The CodeBinder transpiler is a tool designed to convert C# wrapper projects for native libraries using .NET native interop into several target languages, primarily for JDK and Android (via JNI). It utilizes the Roslyn compiler API to parse the syntax trees and semantics of the C# projects to generate output source files (like Java, Objective-C, TypeScript, etc.).

For Java specifically, CodeBinder maps namespaces, translates types (like boxed primitives, enums, structs, interfaces, and classes), generates JNI bindings (including native handle management), and handles the lifecycle of objects using references (`NativeHandle`, `HandleRef`, and a `FinalizableObject` system via `java.lang.ref.Cleaner`).

The primary limitations regarding the conversion include the absence of certain language features, like some generic scenarios, enumerations (currently transpiled mainly to TypeScript natively, though partially implemented as integers for Java), and full delegate interop mapping.

## Prioritized To-Do List

Based on the explicit `#TODO` comments within the codebase and the limitations explicitly stated in the project documentation (`TODO.txt` and `README.md`), here is an exhaustive, prioritized to-do list for improving the C# to Java conversion.

### High Priority
These issues represent core language feature deficiencies or blockers that could prevent the transpiler from successfully converting common C# wrapper patterns to Java.

1. **Boxed Enum Support:**
   - **Issue:** `JavaExtensions_Types.cs` (`isKnowSimpleJavaType`) hardcodes `IntegerBox` for `enum` arguments passed by reference but lacks comprehensive support for boxing enums on non-native methods.
   - **Action:** Implement a complete boxing mechanism for C# enumerations, mapping them safely to `IntegerBox` or creating custom wrapper classes in Java representing the enumerations.
2. **Missing `Enum` Type Constraints & Type Arguments:**
   - **Issue:** Throughout `JavaExtensions_Types.cs`, methods resolving nullable types and generic type constraints throw an explicit exception `throw new Exception("TODO");` when encountering `TypeKind.Enum`.
   - **Action:** Support generic resolution, nullability, and constraints for `Enum` types.
3. **Finish Delegate Interop Support:**
   - **Issue:** Delegate interop is explicitly marked as unsupported in `README.md` across all targets. The `TODO.txt` notes that `cbstring` in return types for delegates fails.
   - **Action:** Complete delegate interoperability mapping, likely generating Java interfaces with `@FunctionalInterface` and proper C native function pointer JNI bindings (trampolines).
4. **Symbol Replacement for Property/Indexer and Generic Method Calls:**
   - **Issue:** Code relies on a limited `SymbolReplacement` framework. The TODO notes the need to replace method generic calls, property/indexer usages, or completely replace "ABI" compatible calls (e.g., replacing a field access with a method call like `IntPtr.Zero -> 0`).
   - **Action:** Expand the syntax tree manipulation to robustly detect and replace full `SyntaxNode` expressions (e.g., `ElementAccessExpressionSyntax` and `InvocationExpressionSyntax`) based on comprehensive compilation context dictionaries rather than raw text matching.

### Medium Priority
These issues involve optimization, type safety, and improving the generated code's idiomatic Java representation.

1. **Struct Support and Blittable Types:**
   - **Issue:** C# `struct`s (value types) support is limited. The `TODO.txt` mentions supporting blittable structs annotated with `[StructLayout(LayoutKind.Sequential)]` by generating C getter/setter methods and corresponding JNI code.
   - **Action:** Extend `JavaStructWriter` to fully generate Java classes with direct JNI field accessors or memory buffer mappings for blittable value types.
2. **`ByRef` Support for Generics:**
   - **Issue:** `TODO.txt` notes a required `ByRef<T>` Java generic class implementation to support C# `ref` arguments across the board safely (especially for object types, not just primitives).
   - **Action:** Implement the `ByRef` and `ByRef<T>` utility classes in `JavaClasses.cs` and ensure the transpiler maps `ref T` to these classes correctly.
3. **Optional Parameters in Constructors:**
   - **Issue:** General `TODO.txt` indicates a lack of support for optional parameter constructors.
   - **Action:** Create method overloads in Java corresponding to the optional parameters present in C# constructors to emulate default arguments.
4. **String Type in DllImport Validation:**
   - **Issue:** An error should be emitted when `System.String` is used directly in `DllImport` methods/delegates instead of the safe `CodeBinder.cbstring` native representation.
   - **Action:** Add a semantic validation rule in `JavaValidationContext.cs` that flags an error when string arguments are encountered in native signatures.

### Low Priority
These issues involve edge cases, specific C# constructs that are rarely used in interop scenarios, or cleanup tasks.

1. **Explicit Interface Implementation Validation:**
   - **Issue:** Explicit Interface Implementation should be explicitly disallowed or gracefully handled, as Java does not support it.
   - **Action:** Add validation to detect explicit implementations and either issue a transpilation error or generate uniquely named methods.
2. **Support for `yield` statement:**
   - **Issue:** `yield` is currently unsupported for Java conversion.
   - **Action:** Generate state-machine classes in Java implementing `Iterator<T>` and `Iterable<T>` to mimic C# `yield return` behaviors.
3. **Main Partial Declarations Detection:**
   - **Issue:** Better support for detecting the main file of partial declarations is needed (`TODO.txt`).
   - **Action:** Refactor the `PartialDeclarationsTree` logic to deterministically pick a "primary" syntax node to attach class-level Javadoc/comments or specific base class derivations.
4. **Custom `jni.h` Wrapper Generation:**
   - **Issue:** JNI generation (`JNITypes.h` / `JNIResources.Designer.cs`) notes: `// TODO: Create a custom jni.h wrapper header and include it`.
   - **Action:** Generate a custom header mapping `.NET` specific handles to standard JNI types dynamically instead of relying purely on fixed strings.
5. **Alternative Finalizers in `JavaClasses.cs`:**
   - **Issue:** `HandledObjectFinalizer` and `IObjectFinalizer` have `#TODO` comments regarding whether they should implement `Runnable` natively or generate a `finalize()` block based on platform constraints.
   - **Action:** Refine the finalization strategy by fully adopting `java.lang.ref.Cleaner` for Java 9+ while maintaining robust legacy `finalize()` fallbacks strictly separated by API level (especially for Android).

### Unhandled Language Edge Cases & Bugs
Additional issues discovered during code review, beyond explicit comments.

1. **Unsupported Expressions Exception Handling:**
   - **Issue:** `JavaBuilderExtensions_Expressions.cs` natively throws raw `Exception` instances for syntax items like `CoalesceExpression`, `PointerMemberAccessExpression`, `AliasQualifiedName`, `TupleType`, etc.
   - **Action:** Introduce specific diagnostic error reporting (e.g., `CompilationError`) and skip generation instead of throwing runtime exceptions that break the transpilation process.
2. **Missing `SymbolReplacementKind` implementations:**
   - **Issue:** `JavaExtensions_Types.cs` and `JavaBuilderExtensions_Expressions.cs` throw `NotImplementedException` for property symbol replacements configured as `SymbolReplacementKind.Method` (in setters) or unknown symbol replacement types in binary expressions.
   - **Action:** Complete the logic to allow mapping properties to setter methods properly, or restrict definition of those unsupported types via validation.
3. **Catch-All Exception Generation `Exception e`:**
   - **Issue:** `JavaBuilderExtensions_Statements.cs` handles parameterless catch blocks `catch { ... }` by translating them directly to `catch (Exception e) { ... }`.
   - **Action:** Java requires catching checked exceptions explicitly or catching `Throwable` for true catch-all parity with C#'s parameterless catch. This could lead to compiler errors in generated Java files or missing JVM Errors.
4. **Unsupported Statements Exception Handling:**
   - **Issue:** `GotoStatement`, `GotoCaseStatement`, and `GotoDefaultStatement` throw `NotSupportedException`.
   - **Action:** Like expressions, these should provide an appropriate diagnostic message or transpiler warning instead of aborting the operation.
5. **Generic `Exception` Throws:**
   - **Issue:** Across `JavaUtils.cs` and the extensions files, dozens of `switch` blocks fall back to `throw new Exception()` for simple mismatches (e.g., trying to identify modifiers, checking parenthesis directions).
   - **Action:** Ensure meaningful exception messages are always thrown, such as `throw new ArgumentOutOfRangeException(nameof(modifier));`, or refactor the code to fail gracefully with logging.
