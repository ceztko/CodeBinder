using CodeTranslator.Shared;
using CodeTranslator.Shared.Java;
using CodeTranslator.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CodeTranslator.Java
{
    class InteropBoxBuilder : ConversionBuilder
    {
        string _Basepath;
        JavaInteropType _primitiveType;
        CSToJavaConversion _conversion;

        public InteropBoxBuilder(JavaInteropType primitiveType, CSToJavaConversion conversion)
        {
            _conversion = conversion;
            _primitiveType = primitiveType;
            _Basepath = string.IsNullOrEmpty(conversion.BaseNamespace) ? null : conversion.BaseNamespace.Replace('.', Path.DirectorySeparatorChar);
        }

        public override string FileName
        {
            get { return BoxTypeName + ".java"; }
        }

        public override void Write(CodeBuilder builder)
        {
            builder.Append("package").Space().Append(_conversion.BaseNamespace).EndOfStatement();
            builder.AppendLine();
            builder.Append("class").Space().Append(BoxTypeName).AppendLine();
            using (builder.Block())
            {
                // Field
                builder.Append("public").Space().Append(JavaType).Space().Append("value").EndOfStatement();
                builder.AppendLine();

                // Default constructor
                builder.Append("public").Space().Append(BoxTypeName).Append("()").Space().AppendLine("{ }");
                builder.AppendLine();

                // Constructor with parameter
                builder.Append("public").Space().Append(BoxTypeName).Parenthesized()
                    .Append(JavaType).Space().Append("value").Close().AppendLine();
                using (builder.Block())
                {
                    builder.Append("this.value = value").EndOfStatement();
                }
            }
        }

        private string JavaType
        {
            get { return JavaUtils.ToJavaType(_primitiveType); }
        }

        private string BoxTypeName
        {
            get { return _primitiveType + "Box"; }
        }

        public override string GeneratedPreamble
        {
            get { return CSToJavaConversion.SourcePreamble; }
        }

        public override string BasePath
        {
            get { return _Basepath; }
        }
    }
}
