using CodeBinder.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.CLang
{
    static class CLangBuilderExtensions
    {
        public static CodeBuilder EndOfLine(this CodeBuilder builder)
        {
            return builder.AppendLine(";");
        }

        public static CodeBuilder Space(this CodeBuilder builder)
        {
            return builder.Append(" ");
        }

        public static CodeBuilder Comma(this CodeBuilder builder)
        {
            return builder.Append(",");
        }

        public static CodeBuilder CommaSeparator(this CodeBuilder builder)
        {
            return builder.Append(", ");
        }

        public static CodeBuilder CommaSeparator(this CodeBuilder builder, ref bool first)
        {
            if (first)
                first = false;
            else
                return builder.CommaSeparator();

            return builder;
        }

        public static CodeBuilder TypeBlock(this CodeBuilder builder, bool appendLine = true)
        {
            builder.AppendLine("{");
            return builder.Indent("}", appendLine);
        }

        public static CodeBuilder Block(this CodeBuilder builder, bool appendLine = true)
        {
            builder.AppendLine("{");
            return builder.Indent("}", appendLine);
        }

        public static CodeBuilder ParameterList(this CodeBuilder builder, bool multiLine = false)
        {
            if (multiLine)
            {
                builder.AppendLine("(");
                return builder.Indent(")");
            }
            else
            {
                builder.Append("(");
                return builder.Using(")");
            }
        }
    }
}
