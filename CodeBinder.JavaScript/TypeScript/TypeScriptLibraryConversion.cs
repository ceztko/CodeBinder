// SPDX-FileCopyrightText: (C) 2023 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

using System.Linq;

namespace CodeBinder.JavaScript.TypeScript;

class TypeScriptLibraryConversion : TypeScriptConversionWriter
{
    public TypeScriptLibraryConversion(TypeScriptCompilationContext context)
        : base(context)
    {
    }

    protected override string GetFileName()
    {
        return $"{Context.LibraryName}.{Context.Conversion.TypeScriptSourceExtension}";
    }

    protected override void write(CodeBuilder builder)
    {
        builder.AppendLine();
        builder.AppendLine($$"""
import { {{ string.Join(", ", TypeScriptCodeBinderClasses.Classes) }} } from './CodeBinder{{ Context.Conversion.TypeScriptModuleLoadSuffix }}';
import napi_ from './{{Context.NAPIWrapperName}}{{Context.Conversion.TypeScriptModuleLoadSuffix}}';
let napi: any = napi_;
""");
        builder.AppendLine();

        builder.AppendLine("// Enums");
        builder.AppendLine();
        foreach (var enm in Context.Enums)
            builder.Append(new TypeScriptEnumWriter(enm)).AppendLine();

        builder.AppendLine("// Interfaces");
        builder.AppendLine();
        foreach (var iface in Context.Interfaces)
            builder.Append(new TypeScriptInterfaceWriter(iface)).AppendLine();

        builder.AppendLine("// Finalizers");
        builder.AppendLine();
        foreach (var finalizer in Context.Finalizers)
            builder.Append(new ClassFinalizerWriter(finalizer, this.Context)).AppendLine();

        builder.AppendLine("// Classes");
        builder.AppendLine();
        foreach (var cls in getSortedTypes(Context.Classes))
            builder.Append(new TypeScriptClassWriter(cls)).AppendLine();
        foreach (var strct in Context.Structs)
            builder.Append(new TypeScriptStructWriter(strct)).AppendLine();

        /* TODO: Handle namespace mapping
        var types = new Dictionary<string, List<CSharpBaseTypeContext>>();
        foreach (var type in Context.NestingTypes)
        {
            var ns = type.Node.GetMappedNamespaceName(Context.Conversion.NamespaceMapping, Context);
            List<CSharpBaseTypeContext>? nsTypes;
            if (!types.TryGetValue(ns, out nsTypes))
            {
                nsTypes = new List<CSharpBaseTypeContext>();
                types.Add(ns, nsTypes);
            }

            nsTypes.Add(type);
        }
        */
    }

    List<TTypeContext> getSortedTypes<TTypeContext>(IEnumerable<TTypeContext> types)
        where TTypeContext : CSharpTypeContext
    {
        // Prepare a dictionary of types indexed on their unique symbol
        var typeDictionary = types.ToDictionary<TTypeContext, INamedTypeSymbol>((type) => type.Symbol, SymbolEqualityComparer.Default);
        var visitedTypes = new HashSet<ITypeSymbol>(SymbolEqualityComparer.Default);

        var ret = new List<TTypeContext>();
        foreach (var pair in typeDictionary)
        {
            if (visitedTypes.Contains(pair.Key))
                continue;

            fetchOrderedTypeRecursive(pair.Value, pair.Key, Context,
                typeDictionary, visitedTypes, ret);
        }

        return ret;
    }

    // Ordered types, recursively searching for other types
    // used in static constructors.
    // NOTE: There are still situations where C# can't be transpiled to
    // TS, such as calling a static method of a inherited class
    // inside the static initialization block of the current class.
    // This should be threated as an error in C#, if it happens
    static void fetchOrderedTypeRecursive<TTypeContext>(TTypeContext type, INamedTypeSymbol symbol, TypeScriptCompilationContext context,
        IReadOnlyDictionary<INamedTypeSymbol, TTypeContext> typeDictionary, HashSet<ITypeSymbol> visitedTypes,
        List<TTypeContext> orderedTypes)
        where TTypeContext : CSharpTypeContext
    {
        visitedTypes.Add(symbol);

        // Visit types used in static constructors
        var staticWalker = new StaticUsageWalker(context);
        fetchStaticUsedTypes(type, context, staticWalker);
        foreach (var usedTypeSymbol in staticWalker.ReferredTypes)
        {
            if (usedTypeSymbol.Equals(symbol, SymbolEqualityComparer.Default) || visitedTypes.Contains(usedTypeSymbol))
                continue;

            fetchOrderedTypeRecursive(typeDictionary[usedTypeSymbol], usedTypeSymbol, context,
                 typeDictionary, visitedTypes, orderedTypes);
        }

        // Visit ancentors
        var ancestorSymbol = symbol.BaseType;
        while (ancestorSymbol != null)
        {
            if (visitedTypes.Contains(ancestorSymbol.OriginalDefinition) ||
                !context.IsCompilationDefined(ancestorSymbol.OriginalDefinition))
            {
                break;
            }

            var ancestorSyntax = typeDictionary[ancestorSymbol];
            fetchOrderedTypeRecursive(ancestorSyntax, ancestorSymbol, context,
                typeDictionary, visitedTypes, orderedTypes);

            ancestorSymbol = symbol.BaseType;
        }

        // Add self to ordered types
        orderedTypes.Add(type);
    }

    // Fetch types used in static constructors. This is required in TypeScript
    static void fetchStaticUsedTypes<TTypeContext>(TTypeContext type, TypeScriptCompilationContext context,
            StaticUsageWalker staticWalker)
        where TTypeContext : CSharpTypeContext
    {
        foreach (var member in type.Node.Members)
        {
            // Search for a static constructor
            if (!member.IsKind(SyntaxKind.ConstructorDeclaration) || !member.GetDeclaredSymbol<IMethodSymbol>(context).IsStatic)
                continue;

            staticWalker.Clear();
            staticWalker.Visit(member);
            break;
        }
    }    

    /// <summary>
    /// Walk syntax searching for statically used types
    /// </summary>
    class StaticUsageWalker : CSharpSyntaxWalker
    {
        HashSet<INamedTypeSymbol> _ReferredTypes;
        TypeScriptCompilationContext _context;

        public StaticUsageWalker(TypeScriptCompilationContext context)
        {
            _ReferredTypes = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
            _context = context;
        }

        public override void VisitIdentifierName(IdentifierNameSyntax node)
        {
            tryHandleStaticAccess(node);
            DefaultVisit(node);
        }

        public IEnumerable<INamedTypeSymbol> ReferredTypes => _ReferredTypes;

        public void Clear()
        {
            _ReferredTypes.Clear();
        }

        private void tryHandleStaticAccess(TypeSyntax syntax)
        {
            var symbol = syntax.GetSymbol(_context);
            if (symbol == null || !symbol.IsStatic)
                return;

            switch (symbol.Kind)
            {
                case SymbolKind.Field:
                case SymbolKind.Property:
                case SymbolKind.Method:
                    break;
                default:
                    return;
            }

            _ReferredTypes.Add(symbol.ContainingType);
        }
    }
}
