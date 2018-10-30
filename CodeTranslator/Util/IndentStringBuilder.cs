// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.Util
{
    // TODO: Add function to disable trim trailing whitespace
    public class IndentStringBuilder : IDisposable
    {
        StringBuilder _builder;
        int _currentIndentLevel;
        bool _doIndent;

        public uint IndentSpaces { get; set; }

        public IndentStringBuilder()
            : this(new StringBuilder())
        {
        }

        public IndentStringBuilder(StringBuilder builder)
            : this(builder, true, 4, 0)
        {
        }

        private IndentStringBuilder(StringBuilder builder, bool doIndent,
            uint indentSpaces, int currentIndentLevel)
        {
            _builder = builder;
            _doIndent = doIndent;
            IndentSpaces = indentSpaces;
            _currentIndentLevel = currentIndentLevel;
        }

        public IndentStringBuilder Append(string str)
        {
            if (str == string.Empty)
                return this;

            append(str);
            return this;
        }

        // TODO: Support custom newline neding
        private void append(string str)
        {
            appendIndent(str, false);
            _builder.Append(str);
            if (str.EndsWith(Environment.NewLine))
                _doIndent = true;
        }

        public IndentStringBuilder AppendLine(string str = "")
        {
            appendIndent(str, true);
            _builder.AppendLine(str);
            _doIndent = true;
            return this;
        }

        public IndentStringBuilder IncreaseIndent()
        {
            _currentIndentLevel++;
            return this;
        }

        public IndentStringBuilder DecreaseIndent()
        {
            if (_currentIndentLevel == 0)
                throw new Exception("Can't decrease indent more");

            _currentIndentLevel--;
            return this;
        }

        public IndentStringBuilder Indent()
        {
            IncreaseIndent();
            return this;
        }

        public IndentStringBuilder Indented()
        {
            return new IndentStringBuilder(_builder, _doIndent, IndentSpaces, _currentIndentLevel + 1);
        }

        private void appendIndent(string str, bool appendLine)
        {
            if (!_doIndent)
                return;

            _doIndent = false;
            if (str.StartsWith(Environment.NewLine) ||
                appendLine && str == string.Empty)
            {
                // Trim trailing whitespace
                return;
            }

            _builder.Append(' ', _currentIndentLevel * (int)IndentSpaces);
        }

        public override string ToString()
        {
            return _builder.ToString();
        }

        void IDisposable.Dispose()
        {
            DecreaseIndent();
        }
    }
}
