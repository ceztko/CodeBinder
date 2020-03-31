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

        protected override string GetBasePath() => ConversionCSharpToObjC.SupportBasePath;

        protected override void write(CodeBuilder builder)
        {
            if (IsHeader)
            {
                builder.AppendLine($"#ifndef CB_{BoxTypeName.ToUpper()}");
                builder.AppendLine($"#define CB_{BoxTypeName.ToUpper()}");
                builder.AppendLine("#pragma once");
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
                    builder.Append("NSUInteger").Space().Append("_length").EndOfStatement();
                    builder.Append("BOOL").Space().Append("_handled").EndOfStatement();
                    builder.Append("BOOL").Space().Append("_immutable").EndOfStatement();
                }
            }
            else
            {
                builder.Append("@implementation").Space().AppendLine(BoxTypeName);
            }

            builder.AppendLine();
            // Constructor with parameter
            builder.Append("-(id)init").Colon().Append("(NSUInteger)").Append("length");
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
                    builder.Append("_values = ").Append($"({ArrayTypeDeclaration})").Append($"calloc(length, sizeof({PrimitiveType.ToTypeName()}))").EndOfStatement();
                    builder.Append("_length = length").EndOfStatement();
                    builder.Append("_handled = YES").EndOfStatement();
                    builder.Append("_immutable = NO").EndOfStatement();
                    builder.Append("return self").EndOfStatement();
                }
            }
            builder.AppendLine();

            builder.Append("-(id)initWithValues").Colon().Append("(NSUInteger)").Append("length").Append(", ...");
            if (IsHeader)
            {
                builder.EndOfStatement();
            }
            else
            {
                builder.AppendLine();
                using (builder.Block())
                {
                    builder.Append("self = [self init:length]").EndOfStatement();
                    builder.Append("if (self == nil)").AppendLine();
                    builder.Append("    return nil").EndOfStatement();
                    builder.Append("va_list args").EndOfStatement();
                    builder.Append("va_start(args, length)").EndOfStatement();
                    builder.Append("for (int i = 0; i < length; i++)").AppendLine();
                    using (builder.Block())
                    {
                        builder.Append($"_values[i] = va_arg(args, {ToPromotedType(PrimitiveType)})").EndOfStatement();
                    }
                    builder.Append("return self").EndOfStatement();
                }
            }
            builder.AppendLine();

            builder.Append("-(id)initWithConstArray").Colon().Append($"({ConstArrayTypeDeclaration})arr")
                .Space().Colon().Append("(NSUInteger)length");
            if (IsHeader)
            {
                builder.EndOfStatement();
            }
            else
            {
                builder.AppendLine();
                using (builder.Block())
                {
                    builder.Append("self = [self initWithConstArray:arr :length :NO]").EndOfStatement();
                    builder.Append("if (self == nil)").AppendLine();
                    builder.Append("    return nil").EndOfStatement();
                    builder.Append("return self").EndOfStatement();
                }
            }
            builder.AppendLine();

            builder.Append("-(id)initWithConstArray").Colon().Append($"({ConstArrayTypeDeclaration})arr")
                .Space().Colon().Append("(NSUInteger)length").Space().Colon().Append("(BOOL)pinned");
            if (IsHeader)
            {
                builder.EndOfStatement();
            }
            else
            {
                builder.AppendLine();
                using (builder.Block())
                {
                    builder.AppendLine("if (pinned)");
                    using(builder.Block())
                    {
                        builder.Append("self = [super init]").EndOfStatement();
                        builder.Append("if (self == nil)").AppendLine();
                        builder.Append("    return nil").EndOfStatement();
                        builder.Append($"_values = ({ArrayTypeDeclaration})arr").EndOfStatement();
                        builder.Append("_length = length").EndOfStatement();
                        builder.Append("_handled = NO").EndOfStatement();
                        builder.Append("_immutable = YES").EndOfStatement();
                    }
                    builder.Append("else").AppendLine();
                    using (builder.Block())
                    {
                        builder.Append("self = [self init:length]").EndOfStatement();
                        builder.Append("if (self == nil)").AppendLine();
                        builder.Append("    return nil").EndOfStatement();
                        builder.Append("memcpy(_values, arr, length)").EndOfStatement();
                    }

                    builder.Append("return self").EndOfStatement();
                }
            }
            builder.AppendLine();

            builder.Append("-(id)initWithArray").Colon().Append($"({ArrayTypeDeclaration})arr")
                .Space().Colon().Append("(NSUInteger)length");
            if (IsHeader)
            {
                builder.EndOfStatement();
            }
            else
            {
                builder.AppendLine();
                using (builder.Block())
                {
                    builder.Append("self = [self initWithArray:arr :length :YES]").EndOfStatement();
                    builder.Append("if (self == nil)").AppendLine();
                    builder.Append("    return nil").EndOfStatement();
                    builder.Append("return self").EndOfStatement();
                }
            }
            builder.AppendLine();

            builder.Append("-(id)initWithArray").Colon().Append($"({ArrayTypeDeclaration})arr")
                .Space().Colon().Append("(NSUInteger)length").Space().Colon().Append("(BOOL)handled");
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
                    builder.Append("_values = arr").EndOfStatement();
                    builder.Append("_length = length").EndOfStatement();
                    builder.Append("_handled = handled").EndOfStatement();
                    builder.Append("return self").EndOfStatement();
                }
            }
            builder.AppendLine();

            // https://nshipster.com/object-subscripting/
            builder.Append($"-({PrimitiveType.ToTypeName()})objectAtIndexedSubscript").Colon().Append("(NSUInteger)idx");
            if (IsHeader)
            {
                builder.EndOfStatement();
            }
            else
            {
                builder.AppendLine();
                using (builder.Block())
                {
                    builder.AppendLine("if (idx >= _length)");
                    builder.Append("    @throw [NSException exceptionWithName:@\"IndexOutofRange\" reason:@\"Index was outside the bounds of the array.\" userInfo:nil]").EndOfStatement();
                    builder.Append("return _values[idx]").EndOfStatement();
                }
            }
            builder.AppendLine();

            builder.Append("-(void)setObject").Colon().Append($"({PrimitiveType.ToTypeName()})value").Space().Append("atIndexedSubscript").Colon().Append("(NSUInteger)idx");
            if (IsHeader)
            {
                builder.EndOfStatement();
            }
            else
            {
                builder.AppendLine();
                using (builder.Block())
                {
                    builder.AppendLine("if (_immutable)");
                    builder.Append("    @throw [NSException exceptionWithName:@\"UnsupportedOperationException\" reason:@\"Can't modify an immutable array.\" userInfo:nil]").EndOfStatement();
                    builder.AppendLine("if (idx >= _length)");
                    builder.Append("    @throw [NSException exceptionWithName:@\"IndexOutofRange\" reason:@\"Index was outside the bounds of the array.\" userInfo:nil]").EndOfStatement();
                    builder.Append("_values[idx] = value").EndOfStatement();
                }
            }
            builder.AppendLine();

            if (IsHeader)
            {
                builder.Append("@property (readonly,nonatomic)").Space().Append($"{ArrayTypeDeclaration}").Space().Append("data").EndOfStatement();
                builder.Append("@property (readonly,nonatomic)").Space().Append($"{ConstArrayTypeDeclaration}").Space().Append("constData").EndOfStatement();
                builder.Append("@property (readonly,nonatomic)").Space().Append("NSUInteger length").EndOfStatement();
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
                    builder.AppendLine("if(_handled)");
                    builder.Append("    free(_values)").EndOfStatement();
                }
            }
            builder.AppendLine();

            builder.Append($"-({ConstArrayTypeDeclaration})constValues");
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
                    builder.AppendLine("if (_immutable)");
                    builder.Append("    @throw [NSException exceptionWithName:@\"UnsupportedOperationException\" reason:@\"Can't modify an immutable array.\" userInfo:nil]").EndOfStatement();
                    builder.Append("return _values").EndOfStatement();
                }
            }
            builder.AppendLine();

            builder.Append($"-(NSUInteger)length");
            if (IsHeader)
            {
                builder.EndOfStatement();
            }
            else
            {
                builder.AppendLine();
                using (builder.Block())
                {
                    builder.Append("return _length").EndOfStatement();
                }
            }

            builder.AppendLine("@end");
            if (IsHeader)
            {
                builder.AppendLine();
                builder.AppendLine($"#endif // CB_{BoxTypeName.ToUpper()}");
            }
        }

        // Narrowed types are promoted https://stackoverflow.com/a/28054417/213871
        static string ToPromotedType(ObjCInteropType type)
        {
            switch (type)
            {
                case ObjCInteropType.NSUInteger:
                    return "NSUInteger";
                case ObjCInteropType.NSInteger:
                    return "NSInteger";
                case ObjCInteropType.UIntPtr:
                    return "void *";
                case ObjCInteropType.IntPtr:
                    return "void *";
                case ObjCInteropType.Boolean:
                    return "int32_t";
                case ObjCInteropType.Char:
                    return "int32_t";
                case ObjCInteropType.Byte:
                    return "int32_t";
                case ObjCInteropType.SByte:
                    return "int32_t";
                case ObjCInteropType.UInt16:
                    return "int32_t";
                case ObjCInteropType.Int16:
                    return "int32_t";
                case ObjCInteropType.UInt32:
                    return "uint32_t";
                case ObjCInteropType.Int32:
                    return "int32_t";
                case ObjCInteropType.UInt64:
                    return "uint64_t";
                case ObjCInteropType.Int64:
                    return "int64_t";
                case ObjCInteropType.Single:
                    return "double";
                case ObjCInteropType.Double:
                    return "double";
                default:
                    throw new Exception();
            }
        }

        public string HeaderFile => $"{BoxTypeName}.h";

        public string ImplementationFile => $"{BoxTypeName}.{ConversionCSharpToObjC.ImplementationExtension}";

        private string ArrayTypeDeclaration => PrimitiveType.ToArrayTypeName(false);

        private string ConstArrayTypeDeclaration => PrimitiveType.ToArrayTypeName(true);

        private string BoxTypeName => PrimitiveType.ToArrayBoxTypeName();
    }
}
