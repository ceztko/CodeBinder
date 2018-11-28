using CodeBinder.Shared;
using CodeBinder.Shared.CSharp;
using CodeBinder.Util;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using CodeBinder.Shared.Java;
using Microsoft.CodeAnalysis;

namespace CodeBinder.Java
{
    static partial class JavaExtensions
    {
        public static IEnumerable<CodeWriter> GetWriters(this StatementSyntax member, ICompilationContextProvider context)
        {
            return null;
        }
    }
}
