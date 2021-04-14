// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using CodeBinder.Apple.Attributes;
using CodeBinder.Attributes;
using CodeBinder.Shared;
using CodeBinder.Shared.CSharp;
using CodeBinder.Util;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using System.Collections.Generic;

namespace CodeBinder.Apple
{
    abstract partial class ObjCTypeConversion<TTypeContext> : CSharpTypeConversion<TTypeContext, ObjCCompilationContext, ConversionCSharpToObjC>
        where TTypeContext : CSharpBaseTypeContext, ITypeContext<ObjCCompilationContext>
    {
        protected ObjCTypeConversion(TTypeContext context, ConversionCSharpToObjC conversion)
            : base(context, conversion) { }
        protected override string? GetGeneratedPreamble() => ConversionCSharpToObjC.SourcePreamble;

        protected override void write(CodeBuilder builder)
        {
            var verbatimConversions = Context.Node.GetAttributes<VerbatimConversionAttribute>(Context);
            var conversion = verbatimConversions.FirstOrDefault((attrib)=>
                attrib.ConstructorArguments.Length == 1 && ConversionType == ConversionType.Implementation
                    || attrib.GetConstructorArgument<ConversionType>(0) == ConversionType);
            if (conversion != null)
            {
                // Use the verbatim conversion instead
                string verbatimStr = conversion.ConstructorArguments.Length == 1
                    ? conversion.GetConstructorArgument<string>(0)
                    : conversion.GetConstructorArgument<string>(1);
                builder.Append(verbatimStr);
                return;
            }

            writePreamble(builder);
            bool hasImports = false;
            foreach (var import in Imports)
            {
                builder.Append("#import").Space().AppendLine(import);
                hasImports = true;
            }

            if (hasImports)
                builder.AppendLine();

            builder.Append(GetTypeWriter());
            writeEnding(builder);
        }

        public abstract ConversionType ConversionType { get; }

        protected virtual void writePreamble(CodeBuilder builder)
        {
            // Do nothing
        }

        protected virtual void writeEnding(CodeBuilder builder)
        {
            // Do nothing
        }

        protected abstract CodeWriter GetTypeWriter();

        protected virtual IEnumerable<string> Imports
        {
            get { yield break; }
        }

        public string InternalHeaderFilename => $"{ConversionCSharpToObjC.InternalBasePath}/{HeaderFilename}";

        public string HeaderFilename => TypeName.ToObjCHeaderFilename();
        public string ImplementationsFilename => $"{Context.Node.GetObjCName(Compilation)}.{ConversionCSharpToObjC.ImplementationExtension}";

        public string TypeName => Context.Node.GetObjCName(Compilation);
    }
}
