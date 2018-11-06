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
        List<DisposeContext> _disposeContexts;

        public uint IndentSpaces { get; set; }

        public CodeBuilder(TextWriter writer)
            : this(writer, true, 4, 0)
        {
        }

        private CodeBuilder(TextWriter writer, bool doIndent,
            uint indentSpaces, int currentIndentLevel)
        {
            _writer = writer;
            _doIndent = doIndent;
            IndentSpaces = indentSpaces;
            _currentIndentLevel = currentIndentLevel;
            _disposeContexts = new List<DisposeContext>();
        }

        public CodeBuilder Append(string str)
        {
            if (str == string.Empty)
                return this;

            append(str);
            return this;
        }

        public CodeBuilder AppendLine(string str = "")
        {
            appendIndent(str, true);
            _writer.WriteLine(str);
            _doIndent = true;
            return this;
        }


        public CodeBuilder Append(ISyntaxWriter writer)
        {
            writer.Write(this);
            return this;
        }


        public CodeBuilder AppendLine(ISyntaxWriter writer)
        {
            writer.Write(this);
            AppendLine();
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

        public CodeBuilder Indent(string appendString = null, bool appendLine = true)
        {
            return Indent(1, appendString, appendLine);
        }

        public CodeBuilder Indent(uint indentCount, string appendString = null, bool appendLine = true)
        {
            for (uint i = 0; i < indentCount; i++)
                IncreaseIndent();

            _disposeContexts.Add(new DisposeContext() { IndentCount = indentCount, AppendString = appendString, AppendLine = appendLine });
            return this;
        }

        public CodeBuilder Indented()
        {
            return new CodeBuilder(_writer, _doIndent, IndentSpaces, _currentIndentLevel + 1);
        }

        // TODO: Support custom newline neding
        private void append(string str)
        {
            appendIndent(str, false);
            _writer.Write(str);
            if (str.EndsWith(Environment.NewLine))
                _doIndent = true;
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
            int contextCount = _disposeContexts.Count;
            if (contextCount > 0)
            {
                disposeContext(_disposeContexts[contextCount - 1]);
                _disposeContexts.RemoveAt(contextCount - 1);
            }
        }

        private void disposeContext(DisposeContext context)
        {
            for (int i = 0; i < context.IndentCount; i++)
                DecreaseIndent();

            if (context.AppendString != null)
            {
                if (context.AppendLine)
                    AppendLine(context.AppendString);
                else
                    Append(context.AppendString);
            }
        }

        #region Support

        class DisposeContext
        {
            public uint IndentCount;
            public string AppendString;
            public bool AppendLine;
        }

        #endregion // Support
    }
}
