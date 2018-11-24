using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Shared.CSharp
{
    public class MethodSignatureInfo
    {
        public string MethodName;
        public SyntaxKind[] Modifiers;
        public MethodParameterInfo ReturnType;
        public MethodParameterInfo[] Parameters;
    }

    public struct MethodParameterInfo
    {
        public MethodParameterInfo(TypedConstant type, string name)
        {
            Type = type;
            Name = name;
        }

        public TypedConstant Type;
        public string Name;
    }
}
