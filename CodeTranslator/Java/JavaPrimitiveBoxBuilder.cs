using CodeTranslator.Shared;
using CodeTranslator.Shared.Java;
using CodeTranslator.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CodeTranslator.Java
{
    class PrimitiveBoxBuilder : ConversionBuilder
    {
        private JavaInteropType _primitiveType;

        public PrimitiveBoxBuilder(JavaInteropType primitiveType)
        {
            _primitiveType = primitiveType;
        }

        public override string FileName
        {
            get { return BoxTypeName + ".java"; }
        }

        public override void Write(CodeBuilder builder)
        {
            builder.Append("package").Space().Append("codentranslator.util").EndOfLine();
            builder.AppendLine();
            builder.Append("class").Space().Append(BoxTypeName).Space();
            using (builder.BeginBlock())
            {
                builder.Append("public").Space().Append(JavaKeyword).Space().Append("value").EndOfLine();
                builder.AppendLine();

                builder.Append("public").Space().Append(BoxTypeName).Append("()").Space().AppendLine("{ }");
                builder.AppendLine();

                builder.Append("public").Space().Append(BoxTypeName).Append("(")
                    .Append(JavaKeyword).Space().Append("value )").Space();
                using (builder.BeginBlock())
                {
                    builder.Append("this.value = value").EndOfLine();
                }
            }
        }

        private string JavaKeyword
        {
            get { return JavaSharedUtils.ToJavaKeyword(_primitiveType); }
        }

        private string BoxTypeName
        {
            get { return _primitiveType + "Box"; }
        }

        public override string GeneratedPreamble
        {
            get { return CSToJavaConversion.GeneratedPreamble; }
        }

        public override string BasePath
        {
            get { return Path.Combine("codetranslator", "util"); }
        }
    }
}
