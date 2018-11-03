// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using CodeTranslator.Shared;
using CodeTranslator.Shared.CSharp;
using CodeTranslator.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CodeTranslator.JNI
{
    public class JNIModuleConversion : TypeConversion<JNIModuleContext, CSToJNIConversion>
    {
        public string Namespace { get; private set; }
        string _Basepath;

        public JNIModuleConversion(CSToJNIConversion langConversion)
            : base(langConversion)
        {
            Namespace = langConversion.BaseNamespace;
            _Basepath = string.IsNullOrEmpty(Namespace) ? null : Namespace.Replace('.', Path.DirectorySeparatorChar);
        }

        public override string FileName
        {
            get { throw new NotImplementedException(); }
        }

        public sealed override void Write(CodeBuilder builder)
        {
            throw new NotImplementedException();
        }
    }
}
