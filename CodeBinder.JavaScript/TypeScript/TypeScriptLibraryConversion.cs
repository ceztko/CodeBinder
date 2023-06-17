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
import napi_ from './NAPIENLibPdf{{Context.Conversion.TypeScriptModuleLoadSuffix}}';
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
    }

    List<TTypeContext> getSortedTypes<TTypeContext>(IEnumerable<TTypeContext> types)
        where TTypeContext : CSharpTypeContext
    {
        var ret = new List<TTypeContext>();
        var typeDictionary = types.ToDictionary<TTypeContext, INamedTypeSymbol>((type) => type.Symbol, SymbolEqualityComparer.Default);
        var visitedTypes = new HashSet<ITypeSymbol>(SymbolEqualityComparer.Default);
        var typeChain = new List<INamedTypeSymbol>();
        foreach (var pair in typeDictionary)
        {
            var symbol = pair.Key;
            if (visitedTypes.Contains(symbol))
                continue;

            typeChain.Clear();
            do
            {
                typeChain.Add(symbol.OriginalDefinition);
                if (symbol.BaseType == null || visitedTypes.Contains(symbol.BaseType.OriginalDefinition) ||
                    !Context.IsCompilationDefined(symbol.BaseType.OriginalDefinition))
                {
                    break;
                }

                symbol = symbol.BaseType;
            } while (symbol != null);

            for (int i = typeChain.Count - 1; i >= 0; i--)
            {
                symbol = typeChain[i];
                visitedTypes.Add(symbol);
                ret.Add(typeDictionary[symbol]);
            }
        }

        return ret;
    }
}
