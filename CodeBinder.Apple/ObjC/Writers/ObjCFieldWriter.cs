using System;
using CodeBinder.Shared.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeBinder.Apple
{
    abstract class ObjCFieldWriterBase<TItem> : ObjCCodeWriterBase<TItem>
    {
        public ObjCFieldWriterBase(TItem syntax, ObjCCompilationContext context)
            : base(syntax, context) { }

        public override ObjWriterType Type => ObjWriterType.Field;
    }

    class ObjCUnderlyingFieldWriter : ObjCFieldWriterBase<PropertyDeclarationSyntax>
    {
        public ObjCUnderlyingFieldWriter(PropertyDeclarationSyntax item, ObjCCompilationContext context)
            : base(item, context) { }

        protected override void Write()
        {
            Builder.Append("@private").Space().Append(Item.Type, ObjCTypeUsageKind.Declaration, Context).Space().Append(ObjCPropertyWriter.GetUnderlyingFieldName(Item, Context)).EndOfStatement();
        }
    }

    class ObjCFieldWriter : ObjCFieldWriterBase<FieldDeclarationSyntax>
    {
        public bool IsStatic { get; private set; }

        public ObjCFieldWriter(FieldDeclarationSyntax syntax, ObjCCompilationContext context)
            : base(syntax, context)
        {
            IsStatic = syntax.IsStatic(context);
        }

        protected override void Write()
        {
            var symbol = Item.GetDeclaredSymbol<IFieldSymbol>(Context);
            if (IsStatic)
            {
                Builder.Append("static");
                if (symbol.DeclaredAccessibility == Accessibility.Public)
                    throw new Exception("Unsupported public static field");
            }
            else
                Builder.Append(Item.GetObjCModifierString(Context));

            Builder.Space();
            if (symbol.IsConst)
                Builder.Append("const").Space();

            Builder.Append(Item.Declaration, Context).EndOfStatement();
        }

        public override ObjWriterType Type => IsStatic ? ObjWriterType.StaticField : ObjWriterType.Field;
    }
}
