// Copyright(c) 2020 Francesco Pretto
// This file is subject to the MIT license
using CodeBinder.Util;
using CodeBinder.Shared;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using CodeBinder.Shared.CSharp;
using CodeBinder.Attributes;
using Microsoft.CodeAnalysis;
using System.Text.RegularExpressions;

namespace CodeBinder.Apple
{
    class ObjCBaseTypesHeaderConversion : ObjCBaseHeaderConversionWriter
    {
        protected override string GetFileName() => ConversionCSharpToObjC.BaseTypesHeader;

        protected override string? GetGeneratedPreamble() => ConversionCSharpToObjC.SourcePreamble;

        protected override void write(CodeBuilder builder)
        {
            BeginHeaderGuard(builder);
            builder.AppendLine();
            builder.AppendLine("// Foundation headers");
            builder.AppendLine("#import <Foundation/Foundation.h>");
            builder.AppendLine("#import <CoreFoundation/CoreFoundation.h>");
            builder.AppendLine();
            builder.AppendLine("// C Std headers");
            builder.AppendLine("#ifdef __cplusplus");
            builder.AppendLine("#include <cstdint>");
            builder.AppendLine("#else // __cplusplus");
            builder.AppendLine("#include <stdint.h>");
            builder.AppendLine("#endif // __cplusplus");
            builder.AppendLine();
            builder.AppendLine("// Interop array box types");
            foreach (var type in ObjCUtils.GetInteropTypes())
                builder.AppendLine($"#import {ObjCUtils.ToArrayBoxTypeName(type).ToObjCHeaderFilename(ConversionCSharpToObjC.SupportBasePath, ObjCHeaderNameUse.IncludeRelativeFirst)}");

            builder.AppendLine();
            builder.AppendLine("// Other types");
            builder.AppendLine($"#import {nameof(ObjCClasses.CBException_h).ToObjCHeaderFilename(ConversionCSharpToObjC.SupportBasePath, ObjCHeaderNameUse.IncludeRelativeFirst)}");
            builder.AppendLine($"#import {nameof(ObjCClasses.CBIEqualityCompararer_h).ToObjCHeaderFilename(ConversionCSharpToObjC.SupportBasePath, ObjCHeaderNameUse.IncludeRelativeFirst)}");
            builder.AppendLine($"#import {nameof(ObjCClasses.CBIReadOnlyList_h).ToObjCHeaderFilename(ConversionCSharpToObjC.SupportBasePath, ObjCHeaderNameUse.IncludeRelativeFirst)}");
            builder.AppendLine($"#import {nameof(ObjCClasses.CBIDisposable_h).ToObjCHeaderFilename(ConversionCSharpToObjC.SupportBasePath, ObjCHeaderNameUse.IncludeRelativeFirst)}");
            builder.AppendLine($"#import {nameof(ObjCClasses.CBKeyValuePair_h).ToObjCHeaderFilename(ConversionCSharpToObjC.SupportBasePath, ObjCHeaderNameUse.IncludeRelativeFirst)}");
            builder.AppendLine($"#import {nameof(ObjCClasses.CBHandleRef_h).ToObjCHeaderFilename(ConversionCSharpToObjC.SupportBasePath, ObjCHeaderNameUse.IncludeRelativeFirst)}");
            builder.AppendLine($"#import {nameof(ObjCResources.CBHandledObject_h).ToObjCHeaderFilename(ConversionCSharpToObjC.SupportBasePath, ObjCHeaderNameUse.IncludeRelativeFirst)}");
            builder.AppendLine();
            EndHeaderGuard(builder);
        }

        protected override string HeaderGuardStem => "BASE_TYPES";
    }
}
