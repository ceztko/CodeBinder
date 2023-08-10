// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT
using CodeBinder.Attributes;

namespace CodeBinder.NativeAOT;

class NAOTDelegatesConversion : NAOTConversionWriter
{
    public NAOTDelegatesConversion(NAOTCompilationContext compilation)
        : base(compilation) { }

    protected override string GetFileName() => "delegates.cs";

    protected override void write(CodeBuilder builder)
    {
        writeFunctionPointerDelegates(builder);
    }

    private void writeFunctionPointerDelegates(CodeBuilder builder)
    {
        // Function pointer delegates
        builder.AppendLine("// Function pointer delegates");
        builder.AppendLine();

        foreach (var callback in Compilation.Callbacks)
        {
            string name;
            AttributeData? bidingAttrib;
            if (callback.TryGetAttribute<NativeBindingAttribute>(Compilation, out bidingAttrib))
                name = bidingAttrib.GetConstructorArgument<string>(0);
            else
                name = callback.Identifier.Text;

            void writeDelegateType()
            {
                if (callback.ParameterList.Parameters.Count != 0)
                {
                    foreach (var param in callback.ParameterList.Parameters)
                        builder.Append(param.GetNAOTType(Compilation)).CommaSeparator();
                }
                builder.Append(callback.GetNAOTReturnType(Compilation));
            }

            builder.Append("global using unsafe").Space().Append(name).Append(" = delegate* unmanaged[Cdecl]")
                .AngleBracketed(writeDelegateType).EndOfStatement();
            builder.AppendLine();
        }
    }
}
