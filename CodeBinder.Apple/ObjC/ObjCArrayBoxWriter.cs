using CodeBinder.Shared;
using CodeBinder.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CodeBinder.Apple
{
    class ObjCArrayBoxWriter : ConversionWriter
    {
        public ObjCInteropType PrimitiveType { get; private set; }
        public bool IsHeader { get; private set; }

        public ObjCArrayBoxWriter(ObjCInteropType primitiveType, bool isheader)
        {
            PrimitiveType = primitiveType;
            IsHeader = isheader;
        }

        protected override string GetFileName() => IsHeader ? HeaderFile : ImplementationFile;

        protected override string GetBasePath() => ConversionCSharpToObjC.InternalBasePath;

        protected override void write(CodeBuilder builder)
        {
            if (IsHeader)
            {
                builder.AppendLine($"#ifndef CB_{BoxTypeName.ToUpper()}");
                builder.AppendLine($"#define CB_{BoxTypeName.ToUpper()}");
                builder.AppendLine();
                builder.AppendLine($"#import <Foundation/Foundation.h>");
            }
            else
            {
                builder.AppendLine($"#import \"{HeaderFile}\"");
                builder.AppendLine($"#include <cstdlib>");
            }

            builder.AppendLine();
            if (IsHeader)
            {
                builder.Append("@interface").Space().Append(BoxTypeName).Space().AppendLine(": NSObject");
                using (builder.Block())
                {
                    // Fields
                    builder.AppendLine("@private");
                    builder.Append(ArrayTypeDeclaration).Space().Append("_values").EndOfStatement();
                    builder.Append("NSInteger").Space().Append("_count").EndOfStatement();
                }
            }
            else
            {
                builder.Append("@implementation").Space().AppendLine(BoxTypeName);
            }

            builder.AppendLine();
            // Constructor with parameter
            builder.Append("-(id)init").Colon().Append("(NSInteger)").Append("count");
            if (IsHeader)
            {
                builder.EndOfStatement();
            }
            else
            {
                builder.AppendLine();
                using (builder.Block())
                {
                    builder.Append("self = [super init]").EndOfStatement();
                    builder.Append("if (self == nil)").AppendLine();
                    builder.Append("    return nil").EndOfStatement();
                    builder.Append("_values = ").Append($"({ArrayTypeDeclaration})").Append($"calloc(count, sizeof({PrimitiveType.ToTypeName()}))").EndOfStatement();
                    builder.Append("_count = count").EndOfStatement(); ;
                    builder.Append("return self").EndOfStatement();
                }
            }
            builder.AppendLine();

            if (IsHeader)
            {
                builder.Append("@property (nonatomic)").Space().Append($"{ArrayTypeDeclaration}").Space().Append("values").EndOfStatement();
                builder.Append("@property (nonatomic)").Space().Append("NSInteger count").EndOfStatement();
                builder.AppendLine();
            }

            builder.Append("-(void)dealloc");
            if (IsHeader)
            {
                builder.EndOfStatement();
            }
            else
            {
                builder.AppendLine();
                using (builder.Block())
                {
                    builder.Append("free(_values)").EndOfStatement();
                }
            }
            builder.AppendLine();

            builder.Append($"-({ArrayTypeDeclaration})values");
            if (IsHeader)
            {
                builder.EndOfStatement();
            }
            else
            {
                builder.AppendLine();
                using (builder.Block())
                {
                    builder.Append("return _values").EndOfStatement();
                }
            }
            builder.AppendLine();

            builder.Append($"-(NSInteger)count");
            if (IsHeader)
            {
                builder.EndOfStatement();
            }
            else
            {
                builder.AppendLine();
                using (builder.Block())
                {
                    builder.Append("return _count").EndOfStatement();
                }
            }

            builder.AppendLine("@end");
            if (IsHeader)
            {
                builder.AppendLine();
                builder.AppendLine($"#endif // CB_{BoxTypeName.ToUpper()}");
            }
        }

        public string HeaderFile => $"{BoxTypeName}.h";

        public string ImplementationFile => $"{BoxTypeName}.{ConversionCSharpToObjC.ImplementationExtension}";

        private string ArrayTypeDeclaration => PrimitiveType.ToArrayTypeName();

        private string BoxTypeName => PrimitiveType.ToArrayBoxTypeName();
    }
}
