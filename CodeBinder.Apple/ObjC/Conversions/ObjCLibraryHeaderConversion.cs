using CodeBinder.Shared;
using CodeBinder.Shared.CSharp;
using CodeBinder.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Apple
{
    class ObjCLibraryHeaderConversion : ObjCHeaderConversionWriter
    {
        public bool IsInternalHeader { get; private set; }

        public ObjCLibraryHeaderConversion(ObjCCompilationContext compilation, bool isInternalHeader)
            : base(compilation)
        {
            IsInternalHeader = isInternalHeader;
        }

        protected override void write(CodeBuilder builder)
        {
            BeginHeaderGuard(builder);
            builder.AppendLine();
            builder.AppendLine("// Protocols");
            foreach (var iface in Compilation.Interfaces)
            {
                string? basepath;
                if (ShouldIgnore(iface, out basepath))
                    continue;

                builder.Append("#include").Space().AppendLine($"{basepath}{iface.GetObjCName(Compilation)}".ToObjCHeaderFilename(ObjCHeaderNameUse.IncludeRelativeFirst));
            }

            builder.AppendLine("// Classes");
            foreach (var cls in Compilation.Classes)
            {
                if (ShouldIgnore(cls))
                    continue;

                builder.Append("#include").Space().AppendLine(cls.GetObjCName(Compilation).ToObjCHeaderFilename(ObjCHeaderNameUse.IncludeRelativeFirst));
            }
            builder.AppendLine();
            EndHeaderGuard(builder);
        }

        bool ShouldIgnore(BaseTypeDeclarationSyntax syntax)
        {
            // Ignore basepath
            return ShouldIgnore(syntax, out var basepath);
        }

        bool ShouldIgnore(BaseTypeDeclarationSyntax syntax, out string? basepath)
        {
            var accessibility = syntax.GetAccessibility(Compilation);
            if (IsInternalHeader)
            {
                if (accessibility == Accessibility.Public)
                {
                    // Public type headers are at higher level
                    basepath = $"../";
                }
                else
                {
                    // Internal type headers are at the same level
                    basepath = null;
                }

                // Internal header has all types
                return false;
            }
            else
            {
                if (accessibility == Accessibility.Public)
                {
                    // Public type headers are at the same level
                    basepath = null;
                    return false;
                }
                else
                {
                    // Ignore public types on internal header
                    basepath = null;
                    return true;
                }

            }
        }

        protected override string HeaderGuardStem => IsInternalHeader ? "LIBRARY_INTERNAL" : "LIBRARY";

        protected override string GetGeneratedPreamble() => ConversionCSharpToObjC.SourcePreamble;

        protected override string GetFileName() => Compilation.ObjCLibraryHeaderName;

        protected override string? GetBasePath() => IsInternalHeader ? ConversionCSharpToObjC.InternalBasePath : null;
    }
}
