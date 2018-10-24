// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using CodeTranslator.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CodeTranslator.Java
{
    abstract class JavaTypeConversion : TypeConversion
    {
        string _Namespace;
        string _Basepath;

        public string Namespace
        {
            get { return _Namespace; }
            set
            {
                _Namespace = value;
                _Basepath = value.Replace('.', Path.DirectorySeparatorChar);
            }
        }

        public override string BasePath
        {
            get { return _Basepath; }
        }

        public override string FileName
        {
            get { return TypeName + ".java"; }
        }

        public abstract string TypeName
        {
            get;
        }

        public override string GeneratedPreamble
        {
            get { return "/* This file was generated. DO NOT EDIT! */"; }
        }

        public virtual IEnumerable<string> Imports
        {
            get { yield break; }
        }

        public override void Write(IndentStringBuilder builder)
        {
            builder.Append("package ");
            builder.Append(Namespace);
            builder.AppendLine(";");
            builder.AppendLine();
            bool hasImports = false;
            foreach (var import in Imports)
            {
                builder.AppendLine();
                hasImports = true;
            }

            if (hasImports)
                builder.AppendLine();

            WriteDeclaration(builder);
        }

        public abstract void WriteDeclaration(IndentStringBuilder builder);
    }
}
