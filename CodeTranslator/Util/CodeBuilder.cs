// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace CodeTranslator.Util
{
    // TODO: Add function to disable trim trailing whitespace
    public class CodeBuilder : IDisposable
    {
        uint _instanceIndentedCount;
        TextWriter _writer;
        uint _currentIndentLevel;
        bool _doIndent;
        List<DisposeContext> _disposeContexts;

        public uint IndentSpaces { get; set; }

        public CodeBuilder(TextWriter writer)
            : this(writer, 0, true, 4, 0) { }

        private CodeBuilder(TextWriter writer, uint instanceIndentedCount,
            bool doIndent, uint indentSpaces, uint currentIndentLevel)
        {
            _writer = writer;
            _instanceIndentedCount = instanceIndentedCount;
            _doIndent = doIndent;
            IndentSpaces = indentSpaces;
            _currentIndentLevel = currentIndentLevel;
            _disposeContexts = new List<DisposeContext>();
        }

        public CodeBuilder Append(string str)
        {
            _instanceIndentedCount = 0;
            if (str == string.Empty)
                return this;

            append(str);
            return this;
        }

        public CodeBuilder AppendLine(string str = "")
        {
            _instanceIndentedCount = 0;
            appendIndent(str, true);
            _writer.WriteLine(str);
            _doIndent = true;
            return this;
        }


        public CodeBuilder Append(ContextWriter writer)
        {
            writer.Write(this);
            return this;
        }

        /// <summary>Reset an indented instance. NOTE: Only a freshly
        /// instance created with Idented() can be reset</summary>
        public void ResetIndented()
        {
            if (_instanceIndentedCount != 0)
            {
                _currentIndentLevel -= _instanceIndentedCount;
                _instanceIndentedCount = 0;
            }
        }

        public CodeBuilder Using(string appendString)
        {
            return disposable(0, appendString, false);
        }

        public CodeBuilder Indent(string appendString = null, bool appendLine = true)
        {
            return disposable(1, appendString, appendLine);
        }

        public CodeBuilder Indent(uint indentCount, string appendString = null, bool appendLine = true)
        {
            if (indentCount == 0)
                throw new Exception("Can't indent with non positive indent count");

            return disposable(indentCount, appendString, appendLine);
        }

        /// <summary>Return a new indented instace</summary>
        public CodeBuilder Indented(uint indentCount = 1)
        {
            if (indentCount == 0)
                throw new Exception("Can't indent with non positive indent count");

            return new CodeBuilder(_writer, indentCount, _doIndent, IndentSpaces, _currentIndentLevel + indentCount);
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

            _writer.Write(new string(' ', (int)(_currentIndentLevel * IndentSpaces)));
        }

        public override string ToString()
        {
            return _writer.ToString();
        }

        CodeBuilder disposable(uint indentCount, string appendString, bool appendLine)
        {
            _currentIndentLevel += indentCount;
            _disposeContexts.Add(new DisposeContext() { IndentCount = indentCount, AppendString = appendString, AppendLine = appendLine });
            return this;
        }

        void IDisposable.Dispose()
        {
            int contextCount = _disposeContexts.Count;
            if (contextCount == 0)
                throw new Exception("");

            disposeContext(_disposeContexts[contextCount - 1]);
            _disposeContexts.RemoveAt(contextCount - 1);
        }

        private void disposeContext(DisposeContext context)
        {
            Debug.Assert(_currentIndentLevel >= context.IndentCount);
            _currentIndentLevel -= context.IndentCount;
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
