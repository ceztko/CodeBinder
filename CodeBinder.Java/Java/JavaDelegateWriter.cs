// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.Java;

class JavaDelegateWriter : JavaConversionWriterBase
{
    bool _isFunction;
    int _argumentCount;

    public JavaDelegateWriter(bool isFunction, int argumentCount)
    {
        _isFunction = isFunction;
        _argumentCount = argumentCount;
    }

    protected override string GetFileName() => $"{DelegateName}.java";

    protected override string GetBasePath() => ConversionCSharpToJava.CodeBinderNamespace;

    protected override void write(CodeBuilder builder)
    {
        void writeArguments()
        {
            bool first = true;
            for (int i = 0; i < _argumentCount; i++)
            {
                builder.CommaSeparator(ref first).Append($"T{i} arg{i}");
            }
        }

        builder.Append("package").Space().Append(ConversionCSharpToJava.CodeBinderNamespace).EndOfStatement();
        builder.AppendLine();
        builder.AppendLine("@FunctionalInterface");
        builder.Append("public interface").Space().Append(DelegateName);
        if (_isFunction || _argumentCount != 0)
        {
            using (builder.TypeParameterList())
            {
                bool first = true;
                for (int i = 0; i < _argumentCount; i++)
                    builder.CommaSeparator(ref first).Append($"T{i}");

                if (_isFunction)
                {
                    if (_argumentCount != 0)
                        builder.CommaSeparator();

                    builder.Append("R");
                }
            }
        }

        using (builder.AppendLine().Block())
        {
            if (_isFunction)
                builder.Append("R");
            else
                builder.Append("void");

            builder.Space().Append("apply").Parenthesized(writeArguments).EndOfStatement();
        }
    }

    private string DelegateName
    {
        get
        {
            if (_isFunction)
                return $"Function{_argumentCount}";
            else
                return $"Action{_argumentCount}";
        }
    }
}
