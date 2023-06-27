// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT
using CodeBinder.Attributes;

namespace CodeBinder.Apple;

partial class ObjCTypeConversion<TTypeContext>
{
    public abstract class Header : ObjCTypeConversion<TTypeContext>
    {
        public ObjCHeaderType HeaderType { get; private set; }

        protected Header(TTypeContext context, ConversionCSharpToObjC conversion, ObjCHeaderType headerType)
            : base(context, conversion)
        {
            HeaderType = headerType;
        }

        protected sealed override string GetFileName() => HeaderFilename;

        protected override string? GetBasePath() =>
            HeaderType == ObjCHeaderType.Public ? null : ConversionCSharpToObjC.InternalBasePath;

        protected override void writePreamble(CodeBuilder builder)
        {
            builder.AppendLine($"#ifndef {HeaderGuard}");
            builder.AppendLine($"#define {HeaderGuard}");
            builder.AppendLine("#pragma once");
            builder.AppendLine();
        }

        protected override void writeEnding(CodeBuilder builder)
        {
            builder.AppendLine($"#endif // {HeaderGuard}");
        }

        string HeaderGuard => HeaderType == ObjCHeaderType.Public
            ? $"{TypeName.ToUpper()}_HEADER"
            : $"{TypeName.ToUpper()}_INTERNAL_HEADER";

        protected override IEnumerable<string> Imports
        {
            get
            {
                if (HeaderType == ObjCHeaderType.Public)
                    yield return $"\"{ObjCLibDefsHeaderConversion.HeaderFileName}\"";

                yield return $"\"{ConversionCSharpToObjC.TypesHeader}\"";
                if (HeaderType.IsPublicLikeHeader())
                {
                    var attributes = Context.Node.GetAttributes(this);
                    foreach (var attribute in attributes)
                    {
                        if (attribute.IsAttribute<ImportAttribute>())
                            yield return attribute.GetConstructorArgument<string>(0); ;
                    }

                    if (Context.Node.BaseList != null)
                    {
                        foreach (var baseType in Context.Node.BaseList.Types)
                        {
                            var typeInfo = baseType.Type.GetObjCTypeInfo(Compilation);
                            if (!ShouldIncludeHeader(typeInfo.Reachability, HeaderType))
                                continue;

                            yield return typeInfo.TypeName.ToObjCHeaderFilename(ObjCHeaderNameUse.IncludeRelativeFirst);
                        }
                    }
                }
                else
                {
                    // Include public header
                    yield return $"\"../{HeaderFilename}\"";
                }
            }
        }

        static bool ShouldIncludeHeader(ObjCTypeReachability reachability, ObjCHeaderType headerType)
        {
            switch (reachability)
            {
                case ObjCTypeReachability.Public:
                    return headerType == ObjCHeaderType.Public || headerType == ObjCHeaderType.InternalOnly;
                case ObjCTypeReachability.Internal:
                    return headerType == ObjCHeaderType.Internal || headerType == ObjCHeaderType.InternalOnly;
                case ObjCTypeReachability.External:
                    return false;
                default:
                    throw new NotSupportedException();
            }
        }

        public override ConversionType ConversionType => ConversionType.Header;
    }
}

class ObjCClassConversionHeader : ObjCTypeConversion<ObjCClassContext>.Header
{
    public ObjCClassConversionHeader(ObjCClassContext context, ConversionCSharpToObjC conversion, ObjCHeaderType headerType)
        : base(context, conversion, headerType) { }
    protected override CodeWriter GetTypeWriter()
    {
        return new ObjCClassWriter(Context.Node, Context.ComputePartialDeclarationsTree(), Context.Compilation, HeaderType.ToFileType());
    }
}

class ObjCStructConversionHeader : ObjCTypeConversion<ObjCStructContext>.Header
{
    public ObjCStructConversionHeader(ObjCStructContext context, ConversionCSharpToObjC conversion, ObjCHeaderType headerType)
        : base(context, conversion, headerType) { }

    protected override CodeWriter GetTypeWriter()
    {
        return new ObjCStructWriter(Context.Node, Context.ComputePartialDeclarationsTree(), Context.Compilation, HeaderType.ToFileType());
    }
}

class ObjCInterfaceConversionHeader : ObjCTypeConversion<ObjCInterfaceContext>.Header
{
    // NOTE: An header never generates both a public and private header, so we just
    // test for the interface being public or not. If is not, we just generate
    // an internal header
    public ObjCInterfaceConversionHeader(ObjCInterfaceContext context, ConversionCSharpToObjC conversion)
        : base(context, conversion, context.Node.HasAccessibility(Accessibility.Public, context) ? ObjCHeaderType.Public : ObjCHeaderType.InternalOnly) { }

    protected override CodeWriter GetTypeWriter()
    {
        return new ObjCInterfaceWriter(Context.Node, Context.ComputePartialDeclarationsTree(), Context.Compilation, HeaderType.ToFileType());
    }
}
