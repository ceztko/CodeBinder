// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using System;
using System.Collections.Generic;
using System.IO;

namespace CodeTranslator.Util
{
    // TODO: Add function to disable trim trailing whitespace
    public class CodeBuilder : IDisposable
    {
        TextWriter _writer;
        int _currentIndentLevel;
        bool _doIndent;
        string _disposeAppendString;
        bool _disposeAppendLine;

        public uint IndentSpaces { get; set; }

        public CodeBuilder(TextWriter writer)
            : this(writer, true, 4, 0, null, false)
        {
        }

        private CodeBuilder(TextWriter writer, bool doIndent,
            uint indentSpaces, int currentIndentLevel, string disposeAppendString,
            bool disposeAppendLine)
        {
            _writer = writer;
            _doIndent = doIndent;
            IndentSpaces = indentSpaces;
            _currentIndentLevel = currentIndentLevel;
        }

        public CodeBuilder Append(string str)
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
            _writer.Write(str);
            if (str.EndsWith(Environment.NewLine))
                _doIndent = true;
        }

        public CodeBuilder AppendLine(string str = "")
        {
            appendIndent(str, true);
            _writer.WriteLine(str);
            _doIndent = true;
            return this;
        }

        public CodeBuilder IncreaseIndent()
        {
            _currentIndentLevel++;
            return this;
        }

        public CodeBuilder DecreaseIndent()
        {
            if (_currentIndentLevel == 0)
                throw new Exception("Can't decrease indent more");

            _currentIndentLevel--;
            return this;
        }

        public CodeBuilder Indent(string disposeAppendString = null, bool disposeAppendLine = true)
        {
            IncreaseIndent();
            _disposeAppendString = disposeAppendString;
            _disposeAppendLine = disposeAppendLine;
            return this;
        }

        public CodeBuilder Indented(string disposeAppendString = null, bool disposeAppendLine = true)
        {
            return new CodeBuilder(_writer, _doIndent, IndentSpaces, _currentIndentLevel + 1, disposeAppendString, disposeAppendLine);
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

            _writer.Write(new string(' ', _currentIndentLevel * (int)IndentSpaces));
        }

        public override string ToString()
        {
            return _writer.ToString();
        }

        void IDisposable.Dispose()
        {
            DecreaseIndent();
            if (_disposeAppendString != null)
            {
                if (_disposeAppendLine)
                    AppendLine(_disposeAppendString);
                else
                    Append(_disposeAppendString);
            }
        }
    }
}
