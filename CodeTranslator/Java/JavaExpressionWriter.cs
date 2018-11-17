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
    abstract class ExpressiontWriter<TExpression> : CodeWriter<TExpression>
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
                                .Append("(").Append(Context.Right, this).Append(")");
                            break;
                        default:
                            break;
                    }
                    return;
                }
            }

            Builder.Append(Context.Left, this).Space().Append(Context.GetJavaOperator()).Space().Append(Context.Right, this);
        }
    }

    class BinaryExpressionWriter : ExpressiontWriter<BinaryExpressionSyntax>
    {
        public BinaryExpressionWriter(BinaryExpressionSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            if (Context.Kind() == SyntaxKind.AsExpression)
            {
                Builder.Append("NULL");
                return;
            }

            Builder.Append(Context.Left, this).Space().Append(Context.GetJavaOperator()).Space().Append(Context.Right, this);
        }
    }

    class CastExpressionWriter : ExpressiontWriter<CastExpressionSyntax>
    {
        public CastExpressionWriter(CastExpressionSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Parenthesized().Append(Context.Type, this).Close().Append(Context.Expression, this);
        }
    }

    class ConditionalExpressionWriter : ExpressiontWriter<ConditionalExpressionSyntax>
    {
        public ConditionalExpressionWriter(ConditionalExpressionSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append(Context.Condition, this).Space().QuestionMark().Space()
                .Append(Context.WhenTrue, this).Space().Colon().Space()
                .Append(Context.WhenFalse, this);
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
            Builder.Append("super");
        }
    }

    class ThisExpressionWriter : ExpressiontWriter<ThisExpressionSyntax>
    {
        public ThisExpressionWriter(ThisExpressionSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append("this");
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
            Builder.Parenthesized().Append(Context.Expression, this);
        }
    }

    class PostfixUnaryExpressionWriter : ExpressiontWriter<PostfixUnaryExpressionSyntax>
    {
        public PostfixUnaryExpressionWriter(PostfixUnaryExpressionSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append(Context.Operand, this).Append(Context.GetJavaOperator());
        }
    }

    class PrefixUnaryExpressionWriter : ExpressiontWriter<PrefixUnaryExpressionSyntax>
    {
        public PrefixUnaryExpressionWriter(PrefixUnaryExpressionSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append(Context.GetJavaOperator()).Append(Context.Operand, this);
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
            Builder.Append(Context.Type, this).Append(".class");
        }
    }

    class ArrayTypeWriter : ExpressiontWriter<ArrayTypeSyntax>
    {
        public ArrayTypeWriter(ArrayTypeSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append(Context.ElementType, this).Append("[]");
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

    class IdentifierNameWriter : ExpressiontWriter<IdentifierNameSyntax>
    {
        public IdentifierNameWriter(IdentifierNameSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            // TODO: identificare properties
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
            if (Context.ElementType.Kind() == SyntaxKind.PredefinedType)
            {
                var prededefined = Context.ElementType as PredefinedTypeSyntax;
                Builder.Append(prededefined.GetJavaType());
                return;
            }

            Builder.Append(Context.ElementType, this);
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

    // Types with keyword: object, string, void, bool, char, byte, int, etc.
    class PredefinedTypeWriter : ExpressiontWriter<PredefinedTypeSyntax>
    {
        public PredefinedTypeWriter(PredefinedTypeSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append(Context.GetJavaType());
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
