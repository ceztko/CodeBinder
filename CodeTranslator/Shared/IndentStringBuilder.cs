// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.Shared
{
    public class IndentStringBuilder
    {
        StringBuilder _builder;
        int _currentIndentLevel;
        bool _newLine;
        bool _nonEmptyLine;

        public uint IndentSpaces { get; set; }

        public IndentStringBuilder()
            : this(new StringBuilder())
        {
        }

        public IndentStringBuilder(StringBuilder builder)
        {
            _builder = builder;
            _newLine = true;
            IndentSpaces = 4;
        }

        // TODO: Handle custom newline neding
        public void Append(string str)
        {
            appendIndent();
            _builder.Append(str);
            if (str.EndsWith(Environment.NewLine))
                _newLine = true;
        }

        public void AppendLine(string str = "")
        {
            appendIndent();
            _builder.AppendLine(str);
            _newLine = true;
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

        private void appendIndent()
        {
            if (!_newLine)
                return;

            _newLine = false;
            _builder.Append(' ', _currentIndentLevel * (int)IndentSpaces);
        }

        public override string ToString()
        {
            return _builder.ToString();
        }
    }
}
