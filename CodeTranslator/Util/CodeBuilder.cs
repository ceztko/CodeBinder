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
        CodeBuilder _parent;
        private CodeBuilder Child { get; set; }
        bool _closed;
        uint _instanceIndentedCount;
        TextWriter _writer;
        uint _currentIndentLevel;
        bool _doIndent;
        List<DisposeContext> _disposeContexts;

        public uint IndentSpaces { get; set; }

        public CodeBuilder()
            : this(new StringWriter()) { }

        public CodeBuilder(TextWriter writer)
            : this(null, writer, 0, true, 4, 0) { }

        private CodeBuilder(CodeBuilder parent, TextWriter writer, uint instanceIndentedCount,
            bool doIndent, uint indentSpaces, uint currentIndentLevel)
        {
            _parent = parent;
            _writer = writer;
            _instanceIndentedCount = instanceIndentedCount;
            _doIndent = doIndent;
            IndentSpaces = indentSpaces;
            _currentIndentLevel = currentIndentLevel;
            _disposeContexts = new List<DisposeContext>();
        }

        ~CodeBuilder()
        {
            if (_closed)
                return;

            close();
        }

        #region Public methods

        public CodeBuilder Append(string str)
        {
            doChecks(ref str);
            _instanceIndentedCount = 0;
            if (str == string.Empty)
                return this;

            append(str);
            return this;
        }

        public CodeBuilder AppendLine(string str = "")
        {
            doChecks(ref str);
            _instanceIndentedCount = 0;
            appendIndent(str, true);
            _writer.WriteLine(str);
            _doIndent = true;
            return this;
        }

        public CodeBuilder Append(CodeWriter writer)
        {
            doChecks();
            writer.Write(this);
            return this;
        }

        /// <summary>Reset an indented instance instance. NOTE: Only a freshly
        /// instance created with Idented() can be reset</summary>
        public void ResetChildIndent()
        {
            doChecks();
            if (_instanceIndentedCount != 0)
            {
                _currentIndentLevel -= _instanceIndentedCount;
                _instanceIndentedCount = 0;
            }
        }

        public CodeBuilder Using(string appendString)
        {
            doChecks();
            return disposable(0, appendString, false);
        }

        public CodeBuilder Indent(string appendString = null, bool appendLine = false)
        {
            doChecks();
            return disposable(1, appendString, appendLine);
        }

        public CodeBuilder Indent(uint indentCount, string appendString = null, bool appendLine = false)
        {
            doChecks();
            if (indentCount == 0)
                throw new Exception("Can't indent with non positive indent count");

            return disposable(indentCount, appendString, appendLine);
        }

        /// <summary>Return a new child instance that can be used in a using directive</summary>
        public CodeBuilder UsingChild(string appendString)
        {
            doChecks();
            return newChild(0).disposable(0, appendString, false);
        }

        /// <summary>Return a new indented child instance</summary>
        public CodeBuilder IndentChild(uint indentCount = 1)
        {
            doChecks();
            if (indentCount == 0)
                throw new Exception("Can't indent with non positive indent count");

            return newChild(indentCount);
        }

        public CodeBuilder Close()
        {
            if (_closed)
                return _parent;

            if (_parent != null)
                _parent.Child = null;
            close();
            return _parent;
        }

        public override string ToString()
        {
            return _writer.ToString();
        }

        #endregion // Public methods

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

        CodeBuilder newChild(uint indentCount)
        {
            if (Child != null)
                throw new Exception("A child is already active");

            Child = new CodeBuilder(this, _writer, indentCount, _doIndent, IndentSpaces, _currentIndentLevel + indentCount);
            return Child;
        }

        CodeBuilder disposable(uint indentCount, string appendString, bool appendLine)
        {
            _currentIndentLevel += indentCount;
            _disposeContexts.Add(new DisposeContext() { IndentCount = indentCount, AppendString = appendString, AppendLine = appendLine });
            return this;
        }

        void doChecks(ref string str)
        {
            if (str == null)
                str = string.Empty;

            doChecks();
        }

        void doChecks()
        {
            if (_closed)
                throw new ObjectDisposedException(nameof(CodeBuilder));

            closeChild();
        }

        private void closeChild()
        {
            if (Child != null)
            {
                Child.close();
                Child = null;
            }
        }

        void close()
        {
            closeChild();
            for (int i = _disposeContexts.Count - 1; i >= 0; i--)
                disposeContext(i);

            _closed = true;
        }

        void IDisposable.Dispose()
        {
            // NOTE: we don't do close() here by purpose to allow using
            // statements to just remove last indent operation
            if (_disposeContexts.Count == 0)
                throw new Exception("Unbalanced dispose operation. Ensure "
                    + "to not spawn children inside using statements");

            disposeContext(_disposeContexts.Count - 1);
        }

        void disposeContext(int contextIndex)
        {
            var context = _disposeContexts[contextIndex];
            Debug.Assert(_currentIndentLevel >= context.IndentCount);
            _currentIndentLevel -= context.IndentCount;
            if (context.AppendLine)
                AppendLine(context.AppendString);
            else if (context.AppendString != null)
                Append(context.AppendString);

            _disposeContexts.RemoveAt(contextIndex);
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
