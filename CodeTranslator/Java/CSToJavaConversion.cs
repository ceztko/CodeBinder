// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using System;
using System.Collections.Generic;
using System.Linq;
using CodeTranslator.Shared.CSharp;

namespace CodeTranslator.Java
{
    public class CSToJavaConversion : CSharpLanguageConversion
    {
        /// <summary>Base namespace of the package, to be set outside</summary>
        public string BaseNamespace { get; set; }

        protected override TypeConversion<CSharpEnumTypeContext> GetEnumTypeConversion()
        {
            var ret = new JavaEnumConversion();
            ret.Namespace = BaseNamespace;
            return ret;
        }
    }
}
