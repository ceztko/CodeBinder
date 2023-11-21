// Copyright(c) 2020 Francesco Pretto
// This file is subject to the MIT license

using CodeBinder.Attributes;

namespace CodeBinder.JNI;

class JNIMethodInitConversion : ConversionWriter
{
    JNICompilationContext _compilation;

    public JNIMethodInitConversion(JNICompilationContext compilation)
    {
        _compilation = compilation;
    }

    protected override void write(CodeBuilder builder)
    {
        foreach (var module in _compilation.Modules)
            builder.Append("#include \"JNI").Append(module.Name).AppendLine(".h\"");

        builder.AppendLine();
        builder.AppendLine("// Reference this symbol to ensure all functions are defined");
        builder.AppendLine("// See https://github.com/dotnet/samples/tree/3870722f5c5e80fd6a70946e6e96a5c990620e42/core/nativeaot/NativeLibrary#user-content-building-static-libraries");
        builder.AppendLine("extern \"C\" void* CB_JNIExports[] = {");
        using (builder.Indent())
        {
            foreach (var module in _compilation.Modules)
            {
                foreach (var method in module.Methods)
                {
                    string? condition = null;
                    if (method.TryGetAttribute<ConditionAttribute>(_compilation, out var attr))
                    {
                        condition = attr.GetConstructorArgument<string>(0);
                        builder.Append("#ifdef").Space().Append(condition).AppendLine();
                    }
                    builder.Append("(void *)").Append(method.GetJNIMethodName(module)).AppendLine(",");
                    if (condition != null)
                        builder.Append("#endif //").Space().Append(condition).AppendLine();
                }
            }
        }

        builder.Append("}").EndOfLine();
    }

    protected override string GetGeneratedPreamble() => ConversionCSharpToJNI.SourcePreamble;

    protected override string GetFileName() => "MethodInit.cpp";
}
