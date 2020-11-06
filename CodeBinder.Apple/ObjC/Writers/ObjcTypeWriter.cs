// Copyright(c) 2020 Francesco Pretto
// This file is subject to the MIT license
using CodeBinder.Shared;
using CodeBinder.Shared.CSharp;
using CodeBinder.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CodeBinder.Apple
{
    /// <remarks>The type code writer should not be bound to type context</remarks>
    /// <typeparam name="TTypeDeclaration"></typeparam>
    [DebuggerDisplay("TypeName = " + nameof(TypeName))]
    abstract class ObjCBaseTypeWriter<TTypeDeclaration>
            : ObjCCodeWriter<TTypeDeclaration>
        where TTypeDeclaration : BaseTypeDeclarationSyntax
    {
        protected ObjCBaseTypeWriter(TTypeDeclaration syntax, ObjCCompilationContext context, ObjCFileType fileType)
            : base(syntax, context, fileType) { }

        protected override void Write()
        {
            if (FileType.IsPublicLikeHeader() && Item.GetAccessibility(Context).RequiresApiAttribute())
                Builder.Append(ObjCLibDefsHeaderConversion.GetLibraryApiMacro(Context)).Space();

            Builder.Append(Item.GetObjcTypeDeclaration(FileType.IsHeader())).Space();
            Builder.Append(ObjCTypeName);
            WriteTypeSpecifiers();
            Builder.AppendLine();
            WriteTypeMembers();
            Builder.AppendLine("@end");
        }

        protected virtual void WriteTypeSpecifiers()
        {
            if (FileType.IsImplementation())
                return;

            if (FileType.IsPublicLikeHeader())
            {
                // Write type parameters and base types
                if (Arity > 0)
                {
                    Builder.Space();
                    // generics declaration
                    WriteTypeParameters();
                }
            }

            WriteBaseTypes();
        }

        protected virtual void WriteBaseTypes()
        {
            Builder.Space();
            // inheritance declaration
            WriteBaseTypes(Item.BaseList);
        }

        private void WriteBaseTypes(BaseListSyntax? baseList)
        {
            string? classType = null;
            var interfaces = new List<string>();
            if (baseList != null)
            {
                foreach (var type in baseList.Types)
                {
                    var typeInfo = type.Type.GetObjCTypeInfo(Context);
                    if (typeInfo.Kind == ObjCTypeKind.Protocol)
                    {
                        if (!ShouldWriteBaseProtocol(typeInfo.Reachability, FileType))
                            continue;

                        interfaces.Add(typeInfo.TypeName);
                    }
                    else if (typeInfo.Kind == ObjCTypeKind.Class && classType == null)
                    {
                        classType = typeInfo.TypeName;
                    }
                    else
                    {
                        throw new NotSupportedException();
                    }
                }
            }

            if (classType == null)
                classType = "NSObject";

            if (FileType == ObjCFileType.InternalHeader)
                Builder.Append("()");
            else
                Builder.Colon().Space().Append(classType);

            if (interfaces.Count != 0)
            {
                Builder.Append("<");
                bool first = true;

                foreach (var iface in interfaces)
                {
                    if (first)
                        first = false;
                    else
                        Builder.CommaSeparator();

                    Builder.Append(iface);
                }

                Builder.Append(">");
            }
        }

        protected void WriteTypeMembers(IEnumerable<MemberDeclarationSyntax> members, PartialDeclarationsTree partialDeclarations)
        {
            var memberFieldWriters = new List<IObjCCodeWriter>();
            var clangMethodWriters = new List<IObjCCodeWriter>();
            var staticFieldWriters = new List<IObjCCodeWriter>();
            var otherTypeWriters = new List<IObjCCodeWriter>();
            foreach (var member in members)
            {
                if (member.ShouldDiscard(Context))
                    continue;

                // On the public header we show only public members
                if (!ShouldEmit(member))
                    continue;

                foreach (var writer in GetWriters(member, partialDeclarations, Context, FileType))
                {
                    switch (writer.Type)
                    {
                        case ObjWriterType.Field:
                        {
                            memberFieldWriters.Add(writer);
                            break;
                        }
                        case ObjWriterType.StaticField:
                        {
                            staticFieldWriters.Add(writer);
                            break;
                        }
                        case ObjWriterType.CLangMethod:
                        {
                            clangMethodWriters.Add(writer);
                            break;
                        }
                        default:
                        {
                            otherTypeWriters.Add(writer);
                            break;
                        }
                    }
                }
            }

            if (FileType.IsPublicLikeHeader() && memberFieldWriters.Count != 0)
            {
                // Write ivar fields block
                bool first = true;
                using (Builder.Block())
                {
                    foreach (var writer in memberFieldWriters)
                        Builder.AppendLine(ref first).Append(writer);
                }

                Builder.AppendLine();
            }

            if (FileType.IsImplementation() && staticFieldWriters.Count != 0)
            {
                // Write static fields
                bool first = true;
                using (Builder.Indent())
                {
                    foreach (var writer in staticFieldWriters)
                        Builder.AppendLine(ref first).Append(writer);
                }

                Builder.AppendLine();
            }

            using (Builder.Indent())
            {
                bool first = true;
                // Write the other members
                foreach (var writer in otherTypeWriters)
                {
                    Builder.AppendLine(ref first).Append(writer);
                }
            }
        }

        static IEnumerable<IObjCCodeWriter> GetWriters(MemberDeclarationSyntax member,
            PartialDeclarationsTree partialDeclarations, ObjCCompilationContext context, ObjCFileType fileType)
        {
            var kind = member.Kind();
            switch (kind)
            {
                case SyntaxKind.ConstructorDeclaration:
                    return new[] { new ObjCConstructorWriter((ConstructorDeclarationSyntax)member, context, fileType) };
                case SyntaxKind.DestructorDeclaration:
                    return new[] { new ObjCDestructorWriter((DestructorDeclarationSyntax)member, context, fileType) };
                case SyntaxKind.MethodDeclaration:
                    return GetWriters((MethodDeclarationSyntax)member, context, fileType);
                case SyntaxKind.PropertyDeclaration:
                    return GetWriters((PropertyDeclarationSyntax)member, context, fileType);
                case SyntaxKind.IndexerDeclaration:
                    return new[] { new ObjCIndexerWriter((IndexerDeclarationSyntax)member, context, fileType) };
                case SyntaxKind.FieldDeclaration:
                {
                    var field = (FieldDeclarationSyntax)member;
                    if (fileType == ObjCFileType.Implementation && !field.IsStatic(context))
                        return Enumerable.Empty<IObjCCodeWriter>();

                    return new[] { new ObjCFieldWriter(field, context) };
                }
                default:
                    throw new NotSupportedException();
            }
        }

        static IEnumerable<IObjCCodeWriter> GetWriters(PropertyDeclarationSyntax property, ObjCCompilationContext context, ObjCFileType fileType)
        {
            var list = new List<IObjCCodeWriter>();
            list.Add(new ObjCPropertyWriter(property, context, fileType));
            if (property.IsAutomatic(context) && fileType.IsInternalLikeHeader())
                list.Add(new ObjCUnderlyingFieldWriter(property, context));

            return list;
        }

        static IEnumerable<IObjCCodeWriter> GetWriters(MethodDeclarationSyntax method, ObjCCompilationContext context, ObjCFileType fileType)
        {
            if (method.IsNative(context))
                yield break;

            if (method.GetCSharpModifiers().Contains(SyntaxKind.PartialKeyword) && method.Body == null)
                yield break;

            for (int i = method.ParameterList.Parameters.Count - 1; i >= 0; i--)
            {
                var parameter = method.ParameterList.Parameters[i];
                if (parameter.Default == null)
                    break;

                yield return new MethodWriter(method, i, context, fileType);
            }

            yield return new MethodWriter(method, -1, context, fileType);
        }

        bool ShouldEmit(MemberDeclarationSyntax member)
        {
            return ShouldEmitSyntax(member.Kind(), member.GetAccessibility(Context), member.IsStatic(Context), FileType);
        }

        // Valid for all type members, excpet property accessors
        static bool ShouldEmitSyntax(SyntaxKind kind, Accessibility accessibility, bool isStatic, ObjCFileType filetype)
        {
            switch (accessibility)
            {
                case Accessibility.Public:
                    return ShouldEmitPublicSyntax(kind, isStatic, filetype);
                case Accessibility.Protected:
                case Accessibility.Private:
                case Accessibility.ProtectedAndInternal:
                case Accessibility.Internal:
                    return ShouldEmitInternalSyntax(kind, isStatic, filetype);
                default:
                    throw new NotSupportedException();
            }
        }

        static bool ShouldEmitPublicSyntax(SyntaxKind kind, bool isStatic, ObjCFileType filetype)
        {
            switch (kind)
            {
                case SyntaxKind.ConstructorDeclaration:
                case SyntaxKind.DestructorDeclaration:
                case SyntaxKind.MethodDeclaration:
                case SyntaxKind.PropertyDeclaration:
                case SyntaxKind.IndexerDeclaration:
                {
                    // Public method like syntax is not emited in the internal header
                    if (filetype == ObjCFileType.InternalHeader)
                        return false;
                    else
                        return true;
                }
                case SyntaxKind.FieldDeclaration:
                {
                    return ShouldEmitFieldSyntax(isStatic, filetype);
                }
                default:
                    throw new NotSupportedException();
            }
        }

        static bool ShouldEmitInternalSyntax(SyntaxKind kind, bool isStatic, ObjCFileType filetype)
        {
            switch (kind)
            {
                case SyntaxKind.ConstructorDeclaration:
                case SyntaxKind.DestructorDeclaration:
                case SyntaxKind.MethodDeclaration:
                case SyntaxKind.PropertyDeclaration:
                case SyntaxKind.IndexerDeclaration:
                {
                    // Internal method like syntax is not emited in the public header
                    if (filetype == ObjCFileType.PublicHeader)
                        return false;
                    else
                        return true;
                }
                case SyntaxKind.FieldDeclaration:
                {
                    return ShouldEmitFieldSyntax(isStatic, filetype);
                }
                default:
                    throw new NotSupportedException();
            }
        }

        static bool ShouldEmitFieldSyntax(bool isStatic, ObjCFileType filetype)
        {
            if (isStatic)
            {
                switch (filetype)
                {
                    // Static fields are emited just in the implementation
                    case ObjCFileType.Implementation:
                        return true;
                    case ObjCFileType.PublicHeader:
                    case ObjCFileType.InternalOnlyHeader:
                    case ObjCFileType.InternalHeader:
                        return false;
                    default:
                        throw new NotSupportedException();
                }
            }
            else
            {
                switch (filetype)
                {
                    // Fields (public or private) are emited only in public or internal only headers
                    case ObjCFileType.PublicHeader:
                    case ObjCFileType.InternalOnlyHeader:
                        return true;
                    case ObjCFileType.Implementation:
                    case ObjCFileType.InternalHeader:
                        return false;
                    default:
                        throw new NotSupportedException();
                }
            }


        }

        bool ShouldWriteBaseProtocol(ObjCTypeReachability reachability, ObjCFileType filetype)
        {
            switch (reachability)
            {
                case ObjCTypeReachability.Public:
                case ObjCTypeReachability.External:
                    return filetype == ObjCFileType.PublicHeader || filetype == ObjCFileType.InternalOnlyHeader;
                case ObjCTypeReachability.Internal:
                    return filetype == ObjCFileType.InternalHeader || filetype == ObjCFileType.InternalOnlyHeader;
                default:
                    throw new NotSupportedException();
            }
        }

        protected virtual void WriteTypeParameters() { /* Do nothing */ }

        protected abstract void WriteTypeMembers();

        public override ObjWriterType Type => ObjWriterType.Type;

        public virtual int Arity => 0;

        public string TypeName => Item.GetName();

        public string ObjCTypeName => Item.GetObjCName(Context);

        public ICompilationContextProvider Provider => Context;

        public ConversionCSharpToObjC Conversion => Context.Conversion;
    }

    abstract class ObjCTypeWriter<TTypeDeclaration> : ObjCBaseTypeWriter<TTypeDeclaration>
        where TTypeDeclaration : TypeDeclarationSyntax
    {
        PartialDeclarationsTree _partialDeclarations;

        protected ObjCTypeWriter(TTypeDeclaration syntax, PartialDeclarationsTree partialDeclarations,
                ObjCCompilationContext context, ObjCFileType fileType)
            : base(syntax, context, fileType)
        {
            _partialDeclarations = partialDeclarations;
        }

        protected override void WriteTypeMembers()
        {
            if (_partialDeclarations.RootPartialDeclarations.Count == 0)
                WriteTypeMembers(Item.Members, _partialDeclarations);
            else
                WriteTypeMembers(GetPartialDeclarationMembers(), _partialDeclarations);
        }

        IEnumerable<MemberDeclarationSyntax> GetPartialDeclarationMembers()
        {
            foreach (var declaration in _partialDeclarations.RootPartialDeclarations)
            {
                foreach (var member in declaration.Members)
                {
                    switch (member.Kind())
                    {
                        case SyntaxKind.InterfaceDeclaration:
                        case SyntaxKind.ClassDeclaration:
                        case SyntaxKind.StructDeclaration:
                        {
                            if (_partialDeclarations.ChildrenPartialDeclarations.ContainsKey((TypeDeclarationSyntax)member))
                                yield return member;
                            break;
                        }
                        default:
                        {
                            yield return member;
                            break;
                        }
                    }
                }
            }
        }
    }
}
