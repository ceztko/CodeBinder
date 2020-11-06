// Copyright(c) 2020 Francesco Pretto
// This file is subject to the MIT license
using System;
using CodeBinder.Shared;
using CodeBinder.Shared.CSharp;
using CodeBinder.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeBinder.Apple
{

    abstract class ObjCCodeWriter<TItem> : ObjCCodeWriterBase<TItem>
    {
        public ObjCFileType FileType { get; private set; }

        protected ObjCCodeWriter(TItem item, ObjCCompilationContext context, ObjCFileType fileType)
            : base(item, context)
        {
            FileType = fileType;
        }
    }

    abstract class ObjCCodeWriterBase<TItem> : CodeWriter<TItem, ObjCCompilationContext>, IObjCCodeWriter
    {
        protected ObjCCodeWriterBase(TItem item, ObjCCompilationContext context)
            : base(item, context) { }

        public abstract ObjWriterType Type { get; }
    }

    interface IObjCCodeWriter : ICodeWriter
    {
        ObjWriterType Type { get; }
    }

    enum ObjWriterType
    {
        Unknown,
        Type,
        Field,
        StaticField,
        Property,
        Method,
        CLangMethod,
        Constructor,
        Destructor,
    }
}
