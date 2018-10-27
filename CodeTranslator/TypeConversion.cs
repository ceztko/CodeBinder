// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using CodeTranslator.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator
{
    public abstract class TypeConversion
    {
        public string ToFullString()
        {
            var builder = new IndentStringBuilder();
            InitWrite();
            builder.AppendLine(GeneratedPreamble);
            Write(builder);
            return builder.ToString();
        }

        public virtual void InitWrite() { /* Do nothing */ }

        public abstract void Write(IndentStringBuilder builder);

        public virtual string GeneratedPreamble
        {
            get { return string.Empty; }
        }

        public abstract string FileName
        {
            get;
        }

        public virtual string BasePath
        {
            get { return ""; }
        }

        public TypeContext TypeContext
        {
            get { return GetTypeContext(); }
        }

        protected abstract TypeContext GetTypeContext();
    }

    public abstract class TypeConversion<TTypeContext> : TypeConversion
        where TTypeContext : TypeContext
    {
        public new TTypeContext TypeContext
        {
            get;
            internal set;
        }

        protected override TypeContext GetTypeContext()
        {
            return TypeContext;
        }
    }
}
