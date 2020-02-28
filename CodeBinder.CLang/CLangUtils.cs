using CodeBinder.Attributes;
using CodeBinder.Shared;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.CLang
{
    public static class CLangUtils
    {
        public static string GetCLangName(ITypeSymbol symbol)
        {
            return symbol.GetAttributes().GetAttribute<NativeBindingAttribute>().GetConstructorArgument<string>(0);
        }
    }
}
