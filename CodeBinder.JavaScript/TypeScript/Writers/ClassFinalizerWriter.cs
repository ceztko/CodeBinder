namespace CodeBinder.JavaScript.TypeScript;

class ClassFinalizerWriter : CodeWriter
{
    MethodDeclarationSyntax _finalizer;
    ITypeSymbol _finalizableType;
    TypeScriptCompilationContext _context;

    public ClassFinalizerWriter(MethodDeclarationSyntax finalizer, TypeScriptCompilationContext context)
    {
        _finalizer = finalizer;
        _finalizableType = finalizer.GetDeclaredSymbol<IMethodSymbol>(context).ContainingType;
        _context = context;
    }

    protected override void Write()
    {
        Builder.AppendLine($"class {_finalizableType.Name}Finalizer extends HandledObjectFinalizer");
        using (Builder.Block())
        {
            Builder.AppendLine("override freeHandle(handle: number): void");
            using (Builder.Block())
            {
                //// ENABLE-ME
                ////Builder.Append(_finalizer.Body!, _context, true).AppendLine();
            }
        }
    }
}
