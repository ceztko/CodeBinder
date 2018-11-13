using CodeTranslator.Shared;
using CodeTranslator.Shared.CSharp;
using CodeTranslator.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.Java
{
    abstract class ExpressiontWriter<TExpression> : ContextWriter<TExpression>
        where TExpression : ExpressionSyntax
    {
        public ExpressiontWriter(TExpression syntax, ICompilationContextProvider context)
            : base(syntax, context) { }
    }

    class ArrayCreationExpressionWriter : ExpressiontWriter<ArrayCreationExpressionSyntax>
    {
        public ArrayCreationExpressionWriter(ArrayCreationExpressionSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append("NULL");
        }
    }

    class OmittedArraySizeExpressionWriter : ExpressiontWriter<OmittedArraySizeExpressionSyntax>
    {
        public OmittedArraySizeExpressionWriter(OmittedArraySizeExpressionSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append("NULL");
        }
    }

    class AssignmentExpressionWriter : ExpressiontWriter<AssignmentExpressionSyntax>
    {
        public AssignmentExpressionWriter(AssignmentExpressionSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            if (Context.Left.Kind() == SyntaxKind.IdentifierName)
            {
                var symbol = Context.Left.GetSymbolInfo(this);
                if (symbol.Symbol.Kind == SymbolKind.Property)
                {
                    var operatorKind = Context.OperatorToken.Kind();
                    switch (operatorKind)
                    {
                        case SyntaxKind.EqualsToken:
                            Builder.Append("set").Append((Context.Left as IdentifierNameSyntax).Identifier.Text)
                                .Append("(").Append(Context.Right.GetWriter(this)).Append(")");
                            break;
                        default:
                            break;
                    }
                    return;
                }
            }

            Builder.Append(Context.Left.GetWriter(this));
            Builder.Space().Append(Context.OperatorToken.Text).Space();
            Builder.Append(Context.Right.GetWriter(this));
        }
    }

    class BinaryExpressionWriter : ExpressiontWriter<BinaryExpressionSyntax>
    {
        public BinaryExpressionWriter(BinaryExpressionSyntax syntax, ICompilationContextProvider context) : base(syntax, context)
        {
        }

        protected override void Write()
        {
            Builder.Append("NULL");
        }
    }

    class CastExpressionWriter : ExpressiontWriter<CastExpressionSyntax>
    {
        public CastExpressionWriter(CastExpressionSyntax syntax, ICompilationContextProvider context) : base(syntax, context)
        {
        }

        protected override void Write()
        {
            Builder.Append("NULL");
        }
    }

    class ConditionalExpressionWriter : ExpressiontWriter<ConditionalExpressionSyntax>
    {
        public ConditionalExpressionWriter(ConditionalExpressionSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append("NULL");
        }
    }

    class DeclarationExpressionWriter : ExpressiontWriter<DeclarationExpressionSyntax>
    {
        public DeclarationExpressionWriter(DeclarationExpressionSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append("NULL");
        }
    }

    class DefaultExpressionWriter : ExpressiontWriter<DefaultExpressionSyntax>
    {
        public DefaultExpressionWriter(DefaultExpressionSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append("NULL");
        }
    }

    class ElementAccessExpressionWriter : ExpressiontWriter<ElementAccessExpressionSyntax>
    {
        public ElementAccessExpressionWriter(ElementAccessExpressionSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append("NULL");
        }
    }

    class InitializerExpressionWriter : ExpressiontWriter<InitializerExpressionSyntax>
    {
        public InitializerExpressionWriter(InitializerExpressionSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append("NULL");
        }
    }

    class BaseExpressionWriter : ExpressiontWriter<BaseExpressionSyntax>
    {
        public BaseExpressionWriter(BaseExpressionSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append("NULL");
        }
    }

    class ThisExpressionWriter : ExpressiontWriter<ThisExpressionSyntax>
    {
        public ThisExpressionWriter(ThisExpressionSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append("NULL");
        }
    }

    class InvocationExpressionWriter : ExpressiontWriter<InvocationExpressionSyntax>
    {
        public InvocationExpressionWriter(InvocationExpressionSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append("NULL");
        }
    }

    class LiteralExpressionWriter : ExpressiontWriter<LiteralExpressionSyntax>
    {
        public LiteralExpressionWriter(LiteralExpressionSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append("NULL");
        }
    }

    class MemberAccessExpressionWriter : ExpressiontWriter<MemberAccessExpressionSyntax>
    {
        public MemberAccessExpressionWriter(MemberAccessExpressionSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append("NULL");
        }
    }

    class ObjectCreationExpressionWriter : ExpressiontWriter<ObjectCreationExpressionSyntax>
    {
        public ObjectCreationExpressionWriter(ObjectCreationExpressionSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append("NULL");
        }
    }

    class ParenthesizedExpressionWriter : ExpressiontWriter<ParenthesizedExpressionSyntax>
    {
        public ParenthesizedExpressionWriter(ParenthesizedExpressionSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append("NULL");
        }
    }

    class PostfixUnaryExpressionWriter : ExpressiontWriter<PostfixUnaryExpressionSyntax>
    {
        public PostfixUnaryExpressionWriter(PostfixUnaryExpressionSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append("NULL");
        }
    }

    class PrefixUnaryExpressionWriter : ExpressiontWriter<PrefixUnaryExpressionSyntax>
    {
        public PrefixUnaryExpressionWriter(PrefixUnaryExpressionSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append("NULL");
        }
    }

    class RefExpressionWriter : ExpressiontWriter<RefExpressionSyntax>
    {
        public RefExpressionWriter(RefExpressionSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append("NULL");
        }
    }

    class ThrowExpressionWriter : ExpressiontWriter<ThrowExpressionSyntax>
    {
        public ThrowExpressionWriter(ThrowExpressionSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append("NULL");
        }
    }

    class TypeOfExpressionWriter : ExpressiontWriter<TypeOfExpressionSyntax>
    {
        public TypeOfExpressionWriter(TypeOfExpressionSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append("NULL");
        }
    }

    class ArrayTypeWriter : ExpressiontWriter<ArrayTypeSyntax>
    {
        public ArrayTypeWriter(ArrayTypeSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append("NULL");
        }
    }

    class QualifiedNameWriter : ExpressiontWriter<QualifiedNameSyntax>
    {
        public QualifiedNameWriter(QualifiedNameSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append("NULL");
        }
    }

    class GenericNameWriter : ExpressiontWriter<GenericNameSyntax>
    {
        public GenericNameWriter(GenericNameSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append("NULL");
        }
    }

    class IdenfitiferNameWriter : ExpressiontWriter<IdentifierNameSyntax>
    {
        public IdenfitiferNameWriter(IdentifierNameSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            var symbol = Context.GetSymbolInfo(this);
            Builder.Append(Context.Identifier.Text);
        }
    }

    class NullableTypeWriter : ExpressiontWriter<NullableTypeSyntax>
    {
        public NullableTypeWriter(NullableTypeSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append("NULL");
        }
    }

    class OmittedTypeArgumentWriter : ExpressiontWriter<OmittedTypeArgumentSyntax>
    {
        public OmittedTypeArgumentWriter(OmittedTypeArgumentSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append("NULL");
        }
    }

    class PredefinedTypeWriter : ExpressiontWriter<PredefinedTypeSyntax>
    {
        public PredefinedTypeWriter(PredefinedTypeSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append("NULL");
        }
    }

    class RefTypeWriter : ExpressiontWriter<RefTypeSyntax>
    {
        public RefTypeWriter(RefTypeSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append("NULL");
        }
    }
}
