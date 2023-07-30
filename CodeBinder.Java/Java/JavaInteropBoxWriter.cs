// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT
using CodeBinder.Java.Shared;

namespace CodeBinder.Java;

class JavaInteropBoxWriter : JavaConversionWriterBase
{
    JavaInteropType _primitiveType;

    public JavaInteropBoxWriter(JavaInteropType primitiveType)
    {
        _primitiveType = primitiveType;
    }

    protected override string GetFileName() => $"{BoxTypeName}.java";

    protected override string GetBasePath() => ConversionCSharpToJava.CodeBinderNamespace;

    protected override void write(CodeBuilder builder)
    {
        builder.Append("package").Space().Append(ConversionCSharpToJava.CodeBinderNamespace).EndOfStatement();
        builder.AppendLine();
        builder.Append("public class").Space().Append(BoxTypeName).AppendLine();
        using (builder.Block())
        {
            // Field
            builder.Append("public").Space().Append(JavaType).Space().Append("value").EndOfStatement();
            builder.AppendLine();

            // Default constructor
            builder.Append("public").Space().Append(BoxTypeName).EmptyParameterList().Space().AppendLine("{ }");
            builder.AppendLine();

            // Constructor with parameter
            builder.Append("public").Space().Append(BoxTypeName).Parenthesized()
                .Append(JavaType).Space().Append("value").Close().AppendLine();
            using (builder.Block())
            {
                builder.Append("this.value = value").EndOfStatement();
            }
        }
    }

    private string JavaType
    {
        get { return JavaUtils.ToJavaType(_primitiveType); }
    }

    private string BoxTypeName
    {
        get { return _primitiveType + "Box"; }
    }
}
