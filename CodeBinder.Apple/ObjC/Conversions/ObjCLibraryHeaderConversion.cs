// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.Apple;

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
        if (IsInternalHeader)
        {
            // TODO: Move all this conditional in a better internal header JUST FOR THE COMPILATION UNITS (*.mm files) and not the headers
            // Ensure compilation units will export symbols
            builder.AppendLine($"#define {ObjCLibDefsHeaderConversion.GetLibraryExportMacro(Compilation)}");
            builder.AppendLine();

            // Ensure compilation has ARC enabled
            builder.AppendLine("#if !__has_feature(objc_arc)");
            builder.AppendLine("    #error \"Code Binder projects are ARC only. Use -fobjc-arc flag\"");
            builder.AppendLine("#endif");
            builder.AppendLine();

            // NOTE: shitty NS_OPTIONS on C++ does #typedef NS_OPTIONS(type, Flags) => typedef int32_t Flags; enum : int32_t ...
            // which causes clashes on CBToString(...). Undef it and redef it to tricky version that allows also
            // to def FlagsInternal so we can overload on that. See ObjCTypesHeaderConversion.writeCBToStringMethod()
            builder.AppendLine("#import <Foundation/Foundation.h>");
            builder.AppendLine("#undef NS_OPTIONS");
            builder.AppendLine("#define NS_OPTIONS(type, name) type name; enum name ## _Options : type");
            builder.AppendLine();
            builder.AppendLine("#import \"OCTypes.h\"");
        }
        builder.AppendLine("// Protocols");
        foreach (var iface in Compilation.Interfaces.Declarations)
        {
            string? basepath;
            if (ShouldIgnore(iface, out basepath))
                continue;

            builder.Append("#import").Space().AppendLine($"{basepath}{iface.GetObjCName(Compilation)}".ToObjCHeaderFilename(ObjCHeaderNameUse.IncludeRelativeFirst));
        }

        builder.AppendLine("// Classes");
        foreach (var cls in Compilation.StorageTypes.Declarations)
        {
            if (ShouldIgnore(cls))
                continue;

            builder.Append("#import").Space().AppendLine(cls.GetObjCName(Compilation).ToObjCHeaderFilename(ObjCHeaderNameUse.IncludeRelativeFirst));
        }
        builder.AppendLine();
        EndHeaderGuard(builder);
    }

    bool ShouldIgnore(BaseTypeDeclarationSyntax syntax)
    {
        // Ignore basepath
        return ShouldIgnore(syntax, out _);
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
