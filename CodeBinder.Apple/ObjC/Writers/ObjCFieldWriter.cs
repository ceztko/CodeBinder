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
            Builder.Append(Item.Type, ObjCTypeUsageKind.Declaration, Context).Space().Append(ObjCPropertyWriter.GetUnderlyingFieldName(Item, Context)).EndOfStatement();
        }
    }

    class ObjCFieldWriter : ObjCFieldWriterBase<FieldDeclarationSyntax>
    {
        public ObjCFieldWriter(FieldDeclarationSyntax syntax, ObjCCompilationContext context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append(Item.Declaration, Context).EndOfStatement();
        }
    }
}
