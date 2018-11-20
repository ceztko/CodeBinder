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
        private JavaInteropType _primitiveType;

        public InteropBoxBuilder(JavaInteropType primitiveType)
        {
            _primitiveType = primitiveType;
        }

        public override string FileName
        {
            get { return BoxTypeName + ".java"; }
        }

        public override void Write(CodeBuilder builder)
        {
            builder.Append("package").Space().Append("codetranslator.utils").EndOfStatement();
            builder.AppendLine();
            builder.Append("public").Space().Append("class").Space().Append(BoxTypeName).AppendLine();
            using (builder.Block())
            {
                builder.Append("public").Space().Append(JavaKeyword).Space().Append("value").EndOfStatement();
                builder.AppendLine();

                builder.Append("public").Space().Append(BoxTypeName).Append("()").Space().AppendLine("{ }");
                builder.AppendLine();

                builder.Append("public").Space().Append(BoxTypeName).Append("(")
                    .Append(JavaKeyword).Space().Append("value)").AppendLine();
                using (builder.Block())
                {
                    builder.Append("this.value = value").EndOfStatement();
                }
            }
        }

        private string JavaKeyword
        {
            get { return JavaUtils.ToJavaKeyword(_primitiveType); }
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
            get { return Path.Combine("codetranslator", "util"); }
        }
    }
}
