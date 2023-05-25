// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace CodeBinder.Utils
{
    // TODO: Add function to disable trim trailing whitespace
    /// <summary>
    /// Low level util class to append code on a TextWriter, with facilities to ease creation of blocks or parenthesized statements
    /// </summary>
    public sealed class CodeBuilder : IDisposable
    {
        CodeBuilder? _parent;
        private CodeBuilder? Child { get; set; }
        bool _closed;
        uint _instanceIndentedCount;
        TextWriter _writer;
        // Determines if we are at the the line beginning
        // and we can do indent content
        bool _atLineBeginning;
        uint _currentIndentLevel;
        List<DisposeContext> _disposeContexts;

        public uint IndentSpaces { get; set; }

        public CodeBuilder()
            : this(new StringWriter()) { }

        public CodeBuilder(TextWriter writer)
            : this(null, writer, 0, true, 4, 0) { }

        private CodeBuilder(CodeBuilder? parent, TextWriter writer, uint instanceIndentedCount,
            bool atLineBeginning, uint indentSpaces, uint currentIndentLevel)
        {
            // Write invariant newline
            writer.NewLine = "\r\n";
            _parent = parent;
            _writer = writer;
            _instanceIndentedCount = instanceIndentedCount;
            _atLineBeginning = atLineBeginning;
            _currentIndentLevel = currentIndentLevel;
            IndentSpaces = indentSpaces;
            _disposeContexts = new List<DisposeContext>();
        }

        ~CodeBuilder()
        {
            if (_closed)
                return;

            close();
        }

        #region Public methods

        // CHECK-ME: Evaluate if performings checks, and which builder actually return
        public CodeBuilder Append(Action<CodeBuilder> action)
        {
            action(this);
            return this;
        }

        public CodeBuilder Append(string str)
        {
            doChecks();
            _instanceIndentedCount = 0;
            if (str == string.Empty)
                return this;

            bool endsWithNewLine;
            var tokens = processLines(str, _atLineBeginning, out endsWithNewLine);
            foreach (var token in tokens)
                _writer.Write(token);

            // If the last line was a newline, then we are
            // now technically at line begin
            _atLineBeginning = endsWithNewLine;
            return this;
        }

        public CodeBuilder AppendLine(string str = "")
        {
            doChecks();
            _instanceIndentedCount = 0;
            if (str != string.Empty)
            {
                var tokens = processLines(str, _atLineBeginning, out _);
                foreach (var token in tokens)
                    _writer.Write(token);
            }

            _writer.WriteLine();
            _atLineBeginning = true;
            return this;
        }

        public CodeBuilder Append(ICodeWriter writer)
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

        /// <summary>
        /// Create a disposable context on this istance
        /// </summary>
        /// <param name="appendString">The string that will be appended when the context has been disposed</param>
        /// <returns></returns>
        public CodeBuilder Using(string appendString)
        {
            doChecks();
            return disposable(0, appendString, false);
        }

        /// <summary>
        /// Create an indented disposable context on this istance
        /// </summary>
        /// <param name="appendString">The string that will be appended when the context has been disposed</param>
        /// <param name="appendLine">True if a line should be appended on the disposing of this context</param>
        public CodeBuilder Indent(string? appendString = null, bool appendLine = false)
        {
            doChecks();
            return disposable(1, appendString, appendLine);
        }

        /// <summary>
        /// Create an indented disposable context on this istance
        /// </summary>
        /// <param name="indentCount">The number of indentation levels</param>
        /// <param name="appendString">The string that will be appended when the context has been disposed</param>
        /// <param name="appendLine">True if a line should be appended on the disposing of this context</param>
        public CodeBuilder Indent(uint indentCount, string? appendString = null, bool appendLine = false)
        {
            doChecks();
            if (indentCount == 0)
                throw new Exception("Can't indent with non positive indent count");

            return disposable(indentCount, appendString, appendLine);
        }

        /// <summary>Return a new disposable child instance</summary>
        /// A child is similar to a regular disposable conext but can be used inline,
        /// example builder.IndentChild().Append("Indented").Close()
        /// <param name="appendString">The string that will be appended when the context has been disposed</param>
        public CodeBuilder UsingChild(string appendString)
        {
            doChecks();
            return newChild(0).disposable(0, appendString, false);
        }

        /// <summary>Return a new indented disposable child instance</summary>
        /// A child is similar to a regular disposable conext but can be used inline,
        /// example builder.IndentChild().Append("Indented").Close()
        /// <param name="indentCount">The number of indentation levels</param>
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
                return _parent ?? this;

            if (_parent != null)
                _parent.Child = null;

            close();
            return _parent ?? this;
        }

        /// <summary>
        /// This method is different from Close, allowing just to remove last indent-like operation
        /// </summary>
        public void CloseUsing()
        {
            // NOTE: we don't do close() here by purpose to allow using
            // statements to just remove last indent operation
            if (_disposeContexts.Count == 0)
                throw new Exception("Unbalanced dispose operation. Ensure "
                    + "to not spawn children inside using statements");

            disposeContext(_disposeContexts.Count - 1);
        }

        public override string ToString()
        {
            closeChild();
            return _writer.ToString() ?? string.Empty;
        }

        #endregion // Public methods

        // Split string into lines and add indentation if required
        private IReadOnlyList<string> processLines(string str, bool atLineBegin, out bool endsWithNewLine)
        {
            bool isNewLineSequence(string str)
            {
                // We assume that the sequence is a newline
                // if it contains any newline character
                switch (str[0])
                {
                    case '\n':
                    case '\r':
                        return true;
                    default:
                        return false;
                }
            }

            var lines = splitString(str);
            Debug.Assert(lines.Count > 0);
            if (_currentIndentLevel == 0)
            {
                // We are not in a indent context, just exit
                endsWithNewLine = isNewLineSequence(lines[^1]);
                return lines;
            }

            var newLines = new List<string>();
            int i;
            if (atLineBegin)
            {
                i = 0;
            }
            else
            {
                // If we are not at line beginning we
                // don't indent the first line
                newLines.Add(lines[0]);
                i = 1;
            }

            var intentStr = new string(' ', (int)(_currentIndentLevel * IndentSpaces));
            bool lastNewLine = false;
            for (; i < lines.Count; i++)
            {
                 var line = lines[i];
                if (isNewLineSequence(line))
                {
                    // Trim trailing whitespace: add the current
                    // newline sequence and continue to next line
                    newLines.Add(line);
                    lastNewLine = true;
                    continue;
                }

                // Add the indentation and the current string
                newLines.Add(intentStr);
                newLines.Add(line);
                lastNewLine = false;
            }

            endsWithNewLine = lastNewLine;
            return newLines;
        }

        CodeBuilder newChild(uint indentCount)
        {
            if (Child != null)
                throw new Exception("A child is already active");

            Child = new CodeBuilder(this, _writer, indentCount, _atLineBeginning, IndentSpaces, _currentIndentLevel + indentCount);
            return Child;
        }

        CodeBuilder disposable(uint indentCount, string? appendString, bool appendLine)
        {
            _currentIndentLevel += indentCount;
            _disposeContexts.Add(new DisposeContext() { IndentCount = indentCount, AppendString = appendString, AppendLine = appendLine });
            return this;
        }

        void doChecks()
        {
            if (_closed)
                throw new ObjectDisposedException($"Can't close {nameof(CodeBuilder)}, ensure you are attempting to close a child instance");

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
            CloseUsing();
        }

        void disposeContext(int contextIndex)
        {
            var context = _disposeContexts[contextIndex];
            Debug.Assert(_currentIndentLevel >= context.IndentCount);
            _currentIndentLevel -= context.IndentCount;
            if (context.AppendLine)
                AppendLine(context.AppendString ?? string.Empty);
            else if (context.AppendString != null)
                Append(context.AppendString);

            _disposeContexts.RemoveAt(contextIndex);
        }

        // Split strings by new lines, preserving them as separate
        // entries. It tries to handle multiple \r or \n new line
        // formats and normalize them in the output
        IReadOnlyList<string> splitString(string str)
        {
            Debug.Assert(str != string.Empty);

            var ret = new List<string>();
            int prevStrIndex = 0;
            int i = 0;
            void pushSubString()
            {
                var substr = str.Substring(prevStrIndex, i - prevStrIndex);
                if (substr.Length != 0)
                    ret.Add(substr);

                prevStrIndex = i + 1;
            }

            bool prevCrlf = false;
            for (; i < str.Length; i++)
            {
                switch (str[i])
                {
                    case '\r':
                    {
                        prevCrlf = true;
                        pushSubString();
                        ret.Add(_writer.NewLine);
                        break;
                    }
                    case '\n':
                    {
                        if (prevCrlf)
                        {
                            prevCrlf = false;
                            prevStrIndex++;
                            continue;
                        }

                        prevCrlf = false;
                        pushSubString();
                        ret.Add(_writer.NewLine);
                        break;
                    }
                    default:
                    {
                        prevCrlf = false;
                        break;
                    }
                }
            }

            pushSubString();
            return ret;
        }


        #region Support

        class DisposeContext
        {
            public uint IndentCount;
            public string? AppendString;
            public bool AppendLine;
        }

        #endregion // Support
    }
}
