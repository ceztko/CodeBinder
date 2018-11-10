using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.Shared
{
    public class MethodSignatureInfo
    {
        public string MethodName;
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
