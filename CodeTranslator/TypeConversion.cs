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
    }
}
