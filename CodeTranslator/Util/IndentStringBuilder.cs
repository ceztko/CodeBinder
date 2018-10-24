// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.Util
{
    // TODO: Add function to disable trim trailing whitespace
    public class IndentStringBuilder
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
        {
            _builder = builder;
            _doIndent = true;
            IndentSpaces = 4;
        }

        public void Append(string str)
        {
            if (str == string.Empty)
                return;

            append(str);
        }

        // TODO: Support custom newline neding
        private void append(string str)
        {
            appendIndent(str, false);
            _builder.Append(str);
            if (str.EndsWith(Environment.NewLine))
                _doIndent = true;
        }

        public void AppendLine(string str = "")
        {
            appendIndent(str, true);
            _builder.AppendLine(str);
            _doIndent = true;
        }

        public void IncreaseIndent()
        {
            _currentIndentLevel++;
        }

        public void DecreaseIndent()
        {
            if (_currentIndentLevel == 0)
                throw new Exception("Can't decrease indent more");

            _currentIndentLevel--;
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
    }
}
