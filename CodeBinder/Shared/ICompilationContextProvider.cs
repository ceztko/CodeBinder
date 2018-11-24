using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Shared
{
    public interface ICompilationContextProvider
    {
        CompilationContext Compilation { get; }
    }
}
