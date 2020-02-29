using CodeBinder.Shared;
using CodeBinder.Shared.CSharp;
using CodeBinder.Util;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using System.Diagnostics;

namespace CodeBinder.Apple
{
    abstract class PropertyWriter<TProperty> : ObjCCodeWriter<TProperty>
            where TProperty : BasePropertyDeclarationSyntax
    {
        protected bool IsAutomatic { get; private set; }
        protected bool IsReadonly { get; private set; }

        public bool IsStatic { get; private set; }

        protected PropertyWriter(TProperty syntax, ObjCCompilationContext context, ObjCFileType fileType)
            : base(syntax, context, fileType)
        {
            IsAutomatic = syntax.IsAutomatic(Context);
            IsReadonly = syntax.IsReadOnly(Context);
            IsStatic = Item.IsStatic(Context);
        }

        public override ObjWriterType Type => ObjWriterType.Method;

        protected override void Write()
        {
            if (!IsStatic)
            {
                if (Item.HasAccessibility(Accessibility.Public, Context))
                {
                    if (FileType.IsPublicHeader())
                        WriteDeclaration();
                }
                else
                {
                    if (FileType.IsInternalKindHeader())
                        WriteDeclaration();
                }

                if (IsAutomatic && FileType.IsImplementation())
                    WriteSynthetization();
            }

            if (!IsAutomatic)
                WriteAccessors();
        }

        protected virtual void WriteSynthetization()
        {
            // Do nothing
        }

        protected abstract void WriteDeclaration();

        internal virtual void WriteAccessors()
        {
            foreach (var accessor in Item.AccessorList!.Accessors)
            {
                if (ShouldEmit(accessor))
                    WriteAccessor(accessor);
            }
        }

        private void WriteAccessor(AccessorDeclarationSyntax accessor)
        {
            switch (accessor.Keyword.Kind())
            {
                case SyntaxKind.GetKeyword:
                    WriteGetter(accessor);
                    break;
                case SyntaxKind.SetKeyword:
                    WriteSetter(accessor);
                    break;
                default:
                    throw new Exception();
            }
        }

        private void WriteSetter(AccessorDeclarationSyntax accessor)
        {
            Builder.Append(Modifier).Space().Append("(void)").Append(SetterName);
            WriteSetterParameters();
            Builder.Colon().Parenthesized().Append(ObjCType).Close().Append("value");

            if (FileType.IsHeader())
            {
                Builder.EndOfStatement();
            }
            else // Is implementation
            {
                if (accessor.Body == null)
                {
                    // Objective C doesn't have abstract properties
                    Debug.Assert(Item.IsAbstract(Context));
                    using (Builder.AppendLine().Block())
                    {
                        Builder.Append("@throw [NSException exceptionWithName:@\"Not implemented\" reason:nil userInfo:nil]").EndOfStatement();
                    }
                }
                else
                {
                    using (Builder.AppendLine().Block())
                    {
                        if (!Context.Conversion.SkipBody)
                            Builder.Append(accessor.Body, Context, true).AppendLine();
                    }
                }
            }
        }

        private void WriteGetter(AccessorDeclarationSyntax accessor)
        {
            Builder.Append(Modifier).Space().Parenthesized().Append(ObjCType).Close().Append(GetterName);
            WriteGetterParameters();

            if (FileType.IsHeader())
            {
                Builder.EndOfStatement();
            }
            else // Is implementation
            {
                if (accessor.Body == null)
                {
                    // Objective C doesn't have abstract properties
                    Debug.Assert(Item.IsAbstract(Context));
                    using (Builder.AppendLine().Block())
                    {
                        Builder.Append("@throw [NSException exceptionWithName:@\"Not implemented\" reason:nil userInfo:nil]").EndOfStatement();
                    }
                }
                else
                {
                    using (Builder.AppendLine().Block())
                    {
                        if (Context.Conversion.SkipBody)
                            Builder.Append(Item.Type.GetObjCDefaultReturnStatement(Context)).EndOfStatement();
                        else
                            Builder.Append(accessor.Body, Context, true).AppendLine();
                    }
                }
            }
        }

        protected virtual void WriteGetterParameters() { /* Do nothing */ }

        protected virtual void WriteSetterParameters()
        {
            // Do nothing
        }

        bool ShouldEmit(AccessorDeclarationSyntax accessor)
        {
            return ShouldEmitAccessor(accessor.GetAccessibility(Context), FileType);
        }

        // Valid for property accessors
        static bool ShouldEmitAccessor(Accessibility accessibility, ObjCFileType filetype)
        {
            switch (accessibility)
            {
                case Accessibility.Public:
                {
                    switch (filetype)
                    {
                        case ObjCFileType.PublicHeader:
                        case ObjCFileType.InternalOnlyHeader:
                        case ObjCFileType.Implementation:
                            return true;
                        case ObjCFileType.InternalHeader:
                            return false;
                        default:
                            throw new NotSupportedException();

                    }
                }
                case Accessibility.Protected:
                case Accessibility.Private:
                case Accessibility.ProtectedAndInternal:
                case Accessibility.Internal:
                {
                    switch (filetype)
                    {
                        case ObjCFileType.PublicHeader:
                            return false;
                        case ObjCFileType.InternalHeader:
                        case ObjCFileType.InternalOnlyHeader:
                        case ObjCFileType.Implementation:
                            return true;
                        default:
                            throw new NotSupportedException();
                    }
                }
                default:
                    throw new NotSupportedException();
            }
        }

        protected string Modifier
        {
            get
            {
                if (IsStatic)
                    return "+";
                else
                    return "-";
            }
        }

        public virtual string GetterName
        {
            get { return PropertyName; }
        }

        public virtual string SetterName
        {
            get { return "set" + PropertyNameCapitalized; }
        }

        public string ObjCType
        {
            get { return Item.Type.GetObjCType(ObjCTypeUsageKind.Declaration, Context); }
        }

        public abstract string PropertyName { get; }

        public abstract string PropertyNameCapitalized { get; }
    }

    class ObjCPropertyWriter : PropertyWriter<PropertyDeclarationSyntax>
    {
        public ObjCPropertyWriter(PropertyDeclarationSyntax syntax, ObjCCompilationContext context, ObjCFileType fileType)
            : base(syntax, context, fileType) { }

        public static string GetUnderlyingFieldName(PropertyDeclarationSyntax property, ObjCCompilationContext context)
        {
            return "__" + property.GetObjCName(context);
        }

        protected override void WriteSynthetization()
        {
            Builder.Append($"@synthesize {PropertyName} = {GetUnderlyingFieldName(Item, Context)}").EndOfStatement();
        }

        protected override void WriteDeclaration()
        {
            Builder.Append($"@property(nonatomic{(IsReadonly ? ",readonly" : "")})").Space().Append(Item.Type, ObjCTypeUsageKind.Declaration, Context).Space().Append(PropertyName).EndOfStatement();
        }

        public override string PropertyName
        {
            get { return Item.GetObjCName(Context); }
        }

        public override string PropertyNameCapitalized
        {
            get { return Item.GetObjCName(Context).ToObjCCaseCapitalized(); }
        }
    }

    class ObjCIndexerWriter : PropertyWriter<IndexerDeclarationSyntax>
    {
        public ObjCIndexerWriter(IndexerDeclarationSyntax syntax, ObjCCompilationContext context, ObjCFileType fileType)
            : base(syntax, context, fileType) { }

        public override string PropertyName => throw new NotImplementedException();

        public override string PropertyNameCapitalized => throw new NotImplementedException();

        public override string GetterName
        {
            get { return "get"; }
        }

        public override string SetterName
        {
            get { return "set"; }
        }

        protected override void WriteDeclaration()
        {
            // Do nothing. Indexers has no par in Objective-C
        }

        protected override void WriteSetterParameters()
        {
            foreach (var parameter in Item.ParameterList.Parameters)
            {
                Builder.Colon().Parenthesized()
                    .Append(parameter.Type!, ObjCTypeUsageKind.Declaration, Context).Close().Append(parameter.Identifier.Text);

                Builder.Space();
            }
        }
    }
}
