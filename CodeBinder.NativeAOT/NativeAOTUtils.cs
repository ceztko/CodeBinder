// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT
using CodeBinder.Attributes;

namespace CodeBinder.NativeAOT;

public static class NativeAOTUtils
{
    public static string GetCLangName(ITypeSymbol symbol, CLangTypeUsageKind usage)
    {
        // FIXME: This is very limited and basically works only for enums right now
        var name = symbol.GetAttributes().GetAttribute<NativeBindingAttribute>().GetConstructorArgument<string>(0);
        TryAdaptType(ref name, symbol, usage);
        return name;
    }

    static void TryAdaptType(ref string type, ITypeSymbol symbol, CLangTypeUsageKind usage)
    {
        if (symbol.IsValueType)
        {
            if (usage == CLangTypeUsageKind.DeclarationByRef)
                type = $"{type} *";
        }
        else
        {
            switch (usage)
            {
                case CLangTypeUsageKind.Declaration:
                {
                    type = $"{type} *";
                    break;
                }
                case CLangTypeUsageKind.DeclarationByRef:
                {
                    type = $"{type} **";
                    break;
                }
                case CLangTypeUsageKind.Normal:
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}

public enum CLangTypeUsageKind
{
    /// <summary>
    /// All other uses
    /// </summary>
    Normal,
    /// <summary>
    /// Local, member, parameter declarations
    /// </summary>
    Declaration,
    /// <summary>
    /// Parameter declarations by ref
    /// </summary>
    DeclarationByRef,
}
