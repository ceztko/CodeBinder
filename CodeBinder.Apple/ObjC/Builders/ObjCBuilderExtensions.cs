// Copyright(c) 2020 Francesco Pretto
// This file is subject to the MIT license
using CodeBinder.Util;
using System;

namespace CodeBinder.Apple
{
    static partial class ObjCBuilderExtension
    {
        public static CodeBuilder Space(this CodeBuilder builder, ref bool first)
        {
            if (first)
                first = false;
            else
                return builder.Space();

            return builder;
        }

        public static CodeBuilder AppendLine(this CodeBuilder builder, ref bool first)
        {
            if (first)
                first = false;
            else
                return builder.AppendLine();

            return builder;
        }

        public static CodeBuilder ColonAppendLine(this CodeBuilder builder, ref bool first)
        {
            if (first)
                first = false;
            else
                return builder.Colon().AppendLine();

            return builder;
        }

        public static CodeBuilder ColonSeparator(this CodeBuilder builder, ref bool first)
        {
            if (first)
                first = false;
            else
                return builder.ColonSeparator();

            return builder;
        }

        public static CodeBuilder CommaSeparator(this CodeBuilder builder, ref bool first)
        {
            if (first)
                first = false;
            else
                return builder.CommaSeparator();

            return builder;
        }

        public static CodeBuilder AmpSeparator(this CodeBuilder builder, ref bool first)
        {
            if (first)
                first = false;
            else
                return builder.AmpSeparator();

            return builder;
        }

        public static CodeBuilder EndOfStatement(this CodeBuilder builder)
        {
            return builder.AppendLine(";");
        }

        public static CodeBuilder SemiColonSeparator(this CodeBuilder builder)
        {
            return builder.Append("; ");
        }

        public static CodeBuilder Colon(this CodeBuilder builder)
        {
            return builder.Append(":");
        }

        public static CodeBuilder SemiColon(this CodeBuilder builder)
        {
            return builder.Append(";");
        }

        public static CodeBuilder ExclamationMark(this CodeBuilder builder)
        {
            return builder.Append("!");
        }

        public static CodeBuilder QuestionMark(this CodeBuilder builder)
        {
            return builder.Append("?");
        }

        public static CodeBuilder Dot(this CodeBuilder builder)
        {
            return builder.Append(".");
        }

        public static CodeBuilder Dereference(this CodeBuilder builder)
        {
            return builder.Append("->");
        }

        public static CodeBuilder Comma(this CodeBuilder builder)
        {
            return builder.Append(",");
        }

        public static CodeBuilder AmpSeparator(this CodeBuilder builder)
        {
            return builder.Append(" & ");
        }

        public static CodeBuilder CommaSeparator(this CodeBuilder builder)
        {
            return builder.Append(", ");
        }

        public static CodeBuilder ColonSeparator(this CodeBuilder builder)
        {
            return builder.Append(": ");
        }

        public static CodeBuilder Space(this CodeBuilder builder)
        {
            return builder.Append(" ");
        }

        public static CodeBuilder EmptyRankSpecifier(this CodeBuilder builder)
        {
            return builder.Append("[]");
        }

        public static CodeBuilder EmptyParameterList(this CodeBuilder builder)
        {
            return builder.Append("()");
        }

        public static CodeBuilder EmptyBody(this CodeBuilder builder)
        {
            return builder.Append("{ }");
        }

        /// <param name="childIstance">False to use in using directive, true to use in a single line</param>
        public static CodeBuilder Parenthesized(this CodeBuilder builder, bool childIstance = true)
        {
            builder.Append("(");
            if (childIstance)
                return builder.UsingChild(")");
            else
                return builder.Using(")");
        }

        /// <param name="childIstance">False to use in using directive, true to use in a single line</param>
        public static CodeBuilder Braced(this CodeBuilder builder, bool childIstance = true)
        {
            builder.Append("{ ");
            if (childIstance)
                return builder.UsingChild(" }");
            else
                return builder.Using(" }");
        }

        /// <param name="childIstance">False to use in using directive, true to use in a single line</param>
        public static CodeBuilder Bracketed(this CodeBuilder builder, bool childIstance = true)
        {
            builder.Append("[");
            if (childIstance)
                return builder.UsingChild("]");
            else
                return builder.Using("]");
        }

        /// <param name="childIstance">False to use in using directive, true to use in a single line</param>
        public static CodeBuilder AngleBracketed(this CodeBuilder builder, bool childIstance = true)
        {
            builder.Append("<");
            if (childIstance)
                return builder.UsingChild(">");
            else
                return builder.Using(">");
        }

        /// <remarks>For one line calls</remarks>
        public static CodeBuilder Bracketed(this CodeBuilder builder, Action<CodeBuilder> bracketed)
        {
            builder.Append("[");
            bracketed(builder);
            return builder.Append("]");
        }

        /// <remarks>For one line calls</remarks>
        public static CodeBuilder Parenthesized(this CodeBuilder builder, Action<CodeBuilder> parenthesized)
        {
            builder.Append("(");
            parenthesized(builder);
            return builder.Append(")");
        }

        // Enum uses a C style type block
        public static CodeBuilder EnumBlock(this CodeBuilder builder, bool appendLine = true)
        {
            builder.AppendLine("{");
            return builder.Indent("};", appendLine);
        }

        public static CodeBuilder Block(this CodeBuilder builder, bool appendLine = true)
        {
            builder.AppendLine("{");
            return builder.Indent("}", appendLine);
        }

        // Used for CLang invocations
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

        public static CodeBuilder TypeParameterList(this CodeBuilder builder, bool multiLine = false)
        {
            if (multiLine)
            {
                builder.AppendLine("<");
                return builder.Indent(">", true);
            }
            else
            {
                builder.Append("<");
                return builder.Using(">");
            }
        }
    }
}
