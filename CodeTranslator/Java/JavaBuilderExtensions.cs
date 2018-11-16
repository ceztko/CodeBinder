using CodeTranslator.Shared;
using CodeTranslator.Shared.CSharp;
using CodeTranslator.Util;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using CodeTranslator.Shared.Java;
using Microsoft.CodeAnalysis;

namespace CodeTranslator.Java
{
    static class JavaWriterExtension
    {
        public static void Append(this CodeBuilder builder,
            TypeParameterListSyntax typeParameterList,
            SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses,
            ICompilationContextProvider context)
        {
            var merged = mergeTypeConstraint(typeParameterList.Parameters, constraintClauses);
            using (builder.TypeParameterList())
            {
                bool first = true;
                foreach (var pair in merged)
                {
                    if (first)
                        first = true;
                    else
                        builder.AppendLine();

                    builder.Append(pair.Type.Identifier.Text);
                    if (pair.Constraints != null)
                    {
                        builder.Space();
                        writeTypeConstraints(builder, pair.Constraints, context);
                    }
                }
            }
        }

        static void writeTypeConstraints(CodeBuilder builder,
            TypeParameterConstraintClauseSyntax constraints,
            ICompilationContextProvider context)
        {
            bool first = true;
            foreach (var constraint in constraints.Constraints)
            {
                if (first)
                    first = false;
                else
                    builder.Space().Append("&").Space();

                builder.Append(constraint, context);
            }
        }

        private static (TypeParameterSyntax Type, TypeParameterConstraintClauseSyntax Constraints)[] mergeTypeConstraint(
            SeparatedSyntaxList<TypeParameterSyntax> typeParameters,
            SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses)
        {
            var ret = new (TypeParameterSyntax Type, TypeParameterConstraintClauseSyntax Constraint)[typeParameters.Count];
            for (int i = 0; i < typeParameters.Count; i++)
            {
                var type = typeParameters[i];
                var constraints = constraintClauses.FirstOrDefault((element) => element.Name.Identifier.Text == type.Identifier.Text);
                ret[i] = (type, constraints);
            }
            return ret;
        }

        public static CodeBuilder Append(this CodeBuilder builder, TypeParameterConstraintSyntax syntax, ICompilationContextProvider context)
        {
            switch (syntax.Kind())
            {
                case SyntaxKind.TypeConstraint:
                {
                    var typeContraints = syntax as TypeConstraintSyntax;
                    string javaTypeName = typeContraints.Type.GetJavaType(context, out var isInterface);

                    builder.Append(isInterface ? "implements" : "extends").Space().Append(javaTypeName);
                    break;
                }
                default:
                    throw new Exception("Unsupported type constraint");
            }

            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, FinallyClauseSyntax syntax, ICompilationContextProvider context)
        {
            return builder.Append(ContextWriter.NullWriter());
        }

        public static CodeBuilder Append(this CodeBuilder builder, CatchClauseSyntax syntax, ICompilationContextProvider context)
        {
            return builder.Append(ContextWriter.NullWriter());
        }

        public static CodeBuilder Append(this CodeBuilder builder, SwitchSectionSyntax syntax, ICompilationContextProvider context)
        {
            return builder.Append(ContextWriter.NullWriter());
        }

        public static CodeBuilder Append(this CodeBuilder builder, ElseClauseSyntax syntax, ICompilationContextProvider context)
        {
            return builder.Append(ContextWriter.NullWriter());
        }

        public static CodeBuilder Append(this CodeBuilder builder, VariableDeclarationSyntax syntax, ICompilationContextProvider context)
        {
            return builder.Append(ContextWriter.NullWriter());
        }

        public static CodeBuilder Append(this CodeBuilder builder, ConstructorDeclarationSyntax member, ICompilationContextProvider context)
        {
            return builder.Append(new ConstructorWriter(member, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, DestructorDeclarationSyntax member, ICompilationContextProvider context)
        {
            return builder.Append(new DestructorWriter(member, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, MethodDeclarationSyntax member, ICompilationContextProvider context)
        {
            return builder.Append(new MethodWriter(member, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, PropertyDeclarationSyntax member, ICompilationContextProvider context)
        {
            return builder.Append(new PropertyWriter(member, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, IndexerDeclarationSyntax member, ICompilationContextProvider context)
        {
            return builder.Append(new IndexerWriter(member, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, FieldDeclarationSyntax member, ICompilationContextProvider context)
        {
            return builder.Append(new FieldWriter(member, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, InterfaceDeclarationSyntax member, ICompilationContextProvider context)
        {
            return builder.Append(new InterfaceTypeWriter(member, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, ClassDeclarationSyntax member, ICompilationContextProvider context)
        {
            return builder.Append(new ClassTypeWriter(member, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, StructDeclarationSyntax member, ICompilationContextProvider context)
        {
            return builder.Append(new StructTypeWriter(member, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, EnumDeclarationSyntax member, ICompilationContextProvider context)
        {
            return builder.Append(new EnumTypeWriter(member, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, ArrayCreationExpressionSyntax expression, ICompilationContextProvider context)
        {
            return builder.Append(new ArrayCreationExpressionWriter(expression, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, OmittedArraySizeExpressionSyntax expression, ICompilationContextProvider context)
        {
            return builder.Append(new OmittedArraySizeExpressionWriter(expression, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, AssignmentExpressionSyntax expression, ICompilationContextProvider context)
        {
            return builder.Append(new AssignmentExpressionWriter(expression, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, BinaryExpressionSyntax expression, ICompilationContextProvider context)
        {
            return builder.Append(new BinaryExpressionWriter(expression, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, CastExpressionSyntax expression, ICompilationContextProvider context)
        {
            return builder.Append(new CastExpressionWriter(expression, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, ConditionalExpressionSyntax expression, ICompilationContextProvider context)
        {
            return builder.Append(new ConditionalExpressionWriter(expression, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, DeclarationExpressionSyntax expression, ICompilationContextProvider context)
        {
            return builder.Append(new DeclarationExpressionWriter(expression, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, ElementAccessExpressionSyntax expression, ICompilationContextProvider context)
        {
            return builder.Append(new ElementAccessExpressionWriter(expression, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, InitializerExpressionSyntax expression, ICompilationContextProvider context)
        {
            return builder.Append(new InitializerExpressionWriter(expression, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, BaseExpressionSyntax expression, ICompilationContextProvider context)
        {
            return builder.Append(new BaseExpressionWriter(expression, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, ThisExpressionSyntax expression, ICompilationContextProvider context)
        {
            return builder.Append(new ThisExpressionWriter(expression, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, InvocationExpressionSyntax expression, ICompilationContextProvider context)
        {
            return builder.Append(new InvocationExpressionWriter(expression, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, LiteralExpressionSyntax expression, ICompilationContextProvider context)
        {
            return builder.Append(new LiteralExpressionWriter(expression, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, MemberAccessExpressionSyntax expression, ICompilationContextProvider context)
        {
            return builder.Append(new MemberAccessExpressionWriter(expression, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, ObjectCreationExpressionSyntax expression, ICompilationContextProvider context)
        {
            return builder.Append(new ObjectCreationExpressionWriter(expression, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, ParenthesizedExpressionSyntax expression, ICompilationContextProvider context)
        {
            return builder.Append(new ParenthesizedExpressionWriter(expression, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, PostfixUnaryExpressionSyntax expression, ICompilationContextProvider context)
        {
            return builder.Append(new PostfixUnaryExpressionWriter(expression, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, PrefixUnaryExpressionSyntax expression, ICompilationContextProvider context)
        {
            return builder.Append(new PrefixUnaryExpressionWriter(expression, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, RefExpressionSyntax expression, ICompilationContextProvider context)
        {
            return builder.Append(new RefExpressionWriter(expression, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, ThrowExpressionSyntax expression, ICompilationContextProvider context)
        {
            return builder.Append(new ThrowExpressionWriter(expression, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, TypeOfExpressionSyntax expression, ICompilationContextProvider context)
        {
            return builder.Append(new TypeOfExpressionWriter(expression, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, ArrayTypeSyntax expression, ICompilationContextProvider context)
        {
            return builder.Append(new ArrayTypeWriter(expression, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, QualifiedNameSyntax expression, ICompilationContextProvider context)
        {
            return builder.Append(new QualifiedNameWriter(expression, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, GenericNameSyntax expression, ICompilationContextProvider context)
        {
            return builder.Append(new GenericNameWriter(expression, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, IdentifierNameSyntax expression, ICompilationContextProvider context)
        {
            return builder.Append(new IdentifierNameWriter(expression, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, NullableTypeSyntax expression, ICompilationContextProvider context)
        {
            return builder.Append(new NullableTypeWriter(expression, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, OmittedTypeArgumentSyntax expression, ICompilationContextProvider context)
        {
            return builder.Append(new OmittedTypeArgumentWriter(expression, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, PredefinedTypeSyntax expression, ICompilationContextProvider context)
        {
            return builder.Append(new PredefinedTypeWriter(expression, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, RefTypeSyntax expression, ICompilationContextProvider context)
        {
            return builder.Append(new RefTypeWriter(expression, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, BlockSyntax statement, ICompilationContextProvider context)
        {
            return builder.Append(new BlockStatementWriter(statement, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, BreakStatementSyntax statement, ICompilationContextProvider context)
        {
            return builder.Append(new BreakStatementWriter(statement, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, ForEachStatementSyntax statement, ICompilationContextProvider context)
        {
            return builder.Append(new ForEachStatementWriter(statement, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, ContinueStatementSyntax statement, ICompilationContextProvider context)
        {
            return builder.Append(new ContinueStatementWriter(statement, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, DoStatementSyntax statement, ICompilationContextProvider context)
        {
            return builder.Append(new DoStatementWriter(statement, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, EmptyStatementSyntax statement, ICompilationContextProvider context)
        {
            return builder.Append(new EmptyStatementWriter(statement, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, ExpressionStatementSyntax statement, ICompilationContextProvider context)
        {
            return builder.Append(new ExpressionStamentWriter(statement, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, ForStatementSyntax statement, ICompilationContextProvider context)
        {
            return builder.Append(new ForStatementWriter(statement, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, IfStatementSyntax statement, ICompilationContextProvider context)
        {
            return builder.Append(new IfStatementWriter(statement, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, LocalDeclarationStatementSyntax statement, ICompilationContextProvider context)
        {
            return builder.Append(new LocalDeclarationStatementWriter(statement, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, LockStatementSyntax statement, ICompilationContextProvider context)
        {
            return builder.Append(new LockStatementWriter(statement, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, ReturnStatementSyntax statement, ICompilationContextProvider context)
        {
            return builder.Append(new ReturnStatementWriter(statement, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, SwitchStatementSyntax statement, ICompilationContextProvider context)
        {
            return builder.Append(new SwitchStatementWriter(statement, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, ThrowStatementSyntax statement, ICompilationContextProvider context)
        {
            return builder.Append(new ThrowStatementWriter(statement, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, TryStatementSyntax statement, ICompilationContextProvider context)
        {
            return builder.Append(new TryStatementWriter(statement, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, UsingStatementSyntax statement, ICompilationContextProvider context)
        {
            return builder.Append(new UsingStatementWriter(statement, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, WhileStatementSyntax statement, ICompilationContextProvider context)
        {
            return builder.Append(new WhileStatementWriter(statement, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, YieldStatementSyntax statement, ICompilationContextProvider context)
        {
            return builder.Append(new YieldStatementWriter(statement, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, ExpressionSyntax syntax, ICompilationContextProvider context)
        {
            return builder.Append(syntax.GetWriter(context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, StatementSyntax syntax, ICompilationContextProvider context)
        {
            return builder.Append(syntax.GetWriter(context));
        }

        public static IEnumerable<ContextWriter> GetWriters(this MemberDeclarationSyntax member, ICompilationContextProvider context)
        {
            var kind = member.Kind();
            switch (kind)
            {
                case SyntaxKind.ConstructorDeclaration:
                    return new[] { new ConstructorWriter(member as ConstructorDeclarationSyntax, context) };
                case SyntaxKind.DestructorDeclaration:
                    return new[] { new DestructorWriter(member as DestructorDeclarationSyntax, context) };
                case SyntaxKind.MethodDeclaration:
                    return getMethodWriters(member as MethodDeclarationSyntax, context);
                case SyntaxKind.PropertyDeclaration:
                    return new[] { new PropertyWriter(member as PropertyDeclarationSyntax, context) };
                case SyntaxKind.IndexerDeclaration:
                    return new[] { new IndexerWriter(member as IndexerDeclarationSyntax, context) };
                case SyntaxKind.FieldDeclaration:
                    return new[] { new FieldWriter(member as FieldDeclarationSyntax, context) };
                case SyntaxKind.InterfaceDeclaration:
                    return new[] { new InterfaceTypeWriter(member as InterfaceDeclarationSyntax, context) };
                case SyntaxKind.ClassDeclaration:
                    return new[] { new ClassTypeWriter(member as ClassDeclarationSyntax, context) };
                case SyntaxKind.StructKeyword:
                    return new[] { new StructTypeWriter(member as StructDeclarationSyntax, context) };
                case SyntaxKind.EnumDeclaration:
                    return new[] { new EnumTypeWriter(member as EnumDeclarationSyntax, context) };
                default:
                    throw new Exception();
            }
        }

        // Reference: roslyn/src/Compilers/CSharp/Portable/Generated/Syntax.xml.Main.Generated.cs
        public static ContextWriter GetWriter(this ExpressionSyntax expression, ICompilationContextProvider context)
        {
            var kind = expression.Kind();
            switch (kind)
            {
                case SyntaxKind.ArrayCreationExpression:
                    return new ArrayCreationExpressionWriter(expression as ArrayCreationExpressionSyntax, context);
                case SyntaxKind.OmittedArraySizeExpression:
                    return new OmittedArraySizeExpressionWriter(expression as OmittedArraySizeExpressionSyntax, context);
                case SyntaxKind.AddAssignmentExpression:
                case SyntaxKind.AndAssignmentExpression:
                case SyntaxKind.DivideAssignmentExpression:
                case SyntaxKind.ExclusiveOrAssignmentExpression:
                case SyntaxKind.LeftShiftAssignmentExpression:
                case SyntaxKind.ModuloAssignmentExpression:
                case SyntaxKind.MultiplyAssignmentExpression:
                case SyntaxKind.OrAssignmentExpression:
                case SyntaxKind.RightShiftAssignmentExpression:
                case SyntaxKind.SimpleAssignmentExpression:
                case SyntaxKind.SubtractAssignmentExpression:
                    return new AssignmentExpressionWriter(expression as AssignmentExpressionSyntax, context);
                case SyntaxKind.AddExpression:
                case SyntaxKind.SubtractExpression:
                case SyntaxKind.MultiplyExpression:
                case SyntaxKind.DivideExpression:
                case SyntaxKind.ModuloExpression:
                case SyntaxKind.LeftShiftExpression:
                case SyntaxKind.RightShiftExpression:
                case SyntaxKind.LogicalOrExpression:
                case SyntaxKind.LogicalAndExpression:
                case SyntaxKind.BitwiseOrExpression:
                case SyntaxKind.BitwiseAndExpression:
                case SyntaxKind.ExclusiveOrExpression:
                case SyntaxKind.EqualsExpression:
                case SyntaxKind.NotEqualsExpression:
                case SyntaxKind.LessThanExpression:
                case SyntaxKind.LessThanOrEqualExpression:
                case SyntaxKind.GreaterThanExpression:
                case SyntaxKind.GreaterThanOrEqualExpression:
                case SyntaxKind.IsExpression:
                case SyntaxKind.AsExpression:
                case SyntaxKind.CoalesceExpression:
                    return new BinaryExpressionWriter(expression as BinaryExpressionSyntax, context);
                case SyntaxKind.CastExpression:
                    return new CastExpressionWriter(expression as CastExpressionSyntax, context);
                case SyntaxKind.ConditionalExpression:
                    return new ConditionalExpressionWriter(expression as ConditionalExpressionSyntax, context);
                case SyntaxKind.DeclarationExpression:
                    return new DeclarationExpressionWriter(expression as DeclarationExpressionSyntax, context);
                case SyntaxKind.ElementAccessExpression:
                    return new ElementAccessExpressionWriter(expression as ElementAccessExpressionSyntax, context);
                case SyntaxKind.ObjectInitializerExpression:
                case SyntaxKind.CollectionInitializerExpression:
                case SyntaxKind.ArrayInitializerExpression:
                case SyntaxKind.ComplexElementInitializerExpression:
                    return new InitializerExpressionWriter(expression as InitializerExpressionSyntax, context);
                case SyntaxKind.BaseExpression:
                    return new BaseExpressionWriter(expression as BaseExpressionSyntax, context);
                case SyntaxKind.ThisExpression:
                    return new ThisExpressionWriter(expression as ThisExpressionSyntax, context);
                case SyntaxKind.InvocationExpression:
                    return new InvocationExpressionWriter(expression as InvocationExpressionSyntax, context);
                case SyntaxKind.ArgListExpression:
                case SyntaxKind.NumericLiteralExpression:
                case SyntaxKind.StringLiteralExpression:
                case SyntaxKind.CharacterLiteralExpression:
                case SyntaxKind.TrueLiteralExpression:
                case SyntaxKind.FalseLiteralExpression:
                case SyntaxKind.NullLiteralExpression:
                case SyntaxKind.DefaultLiteralExpression:
                    return new LiteralExpressionWriter(expression as LiteralExpressionSyntax, context);
                case SyntaxKind.SimpleMemberAccessExpression:
                case SyntaxKind.PointerMemberAccessExpression:
                    return new MemberAccessExpressionWriter(expression as MemberAccessExpressionSyntax, context);
                case SyntaxKind.ObjectCreationExpression:
                    return new ObjectCreationExpressionWriter(expression as ObjectCreationExpressionSyntax, context);
                case SyntaxKind.ParenthesizedExpression:
                    return new ParenthesizedExpressionWriter(expression as ParenthesizedExpressionSyntax, context);
                case SyntaxKind.PostIncrementExpression:
                case SyntaxKind.PostDecrementExpression:
                    return new PostfixUnaryExpressionWriter(expression as PostfixUnaryExpressionSyntax, context);
                case SyntaxKind.UnaryPlusExpression:
                case SyntaxKind.UnaryMinusExpression:
                case SyntaxKind.BitwiseNotExpression:
                case SyntaxKind.LogicalNotExpression:
                case SyntaxKind.PreIncrementExpression:
                case SyntaxKind.PreDecrementExpression:
                case SyntaxKind.AddressOfExpression:
                case SyntaxKind.PointerIndirectionExpression:
                    return new PrefixUnaryExpressionWriter(expression as PrefixUnaryExpressionSyntax, context);
                case SyntaxKind.RefExpression:
                    return new RefExpressionWriter(expression as RefExpressionSyntax, context);
                case SyntaxKind.ThrowExpression:
                    return new ThrowExpressionWriter(expression as ThrowExpressionSyntax, context);
                case SyntaxKind.TypeOfExpression:
                    return new TypeOfExpressionWriter(expression as TypeOfExpressionSyntax, context);
                case SyntaxKind.ArrayType:
                    return new ArrayTypeWriter(expression as ArrayTypeSyntax, context);
                case SyntaxKind.QualifiedName:
                    return new QualifiedNameWriter(expression as QualifiedNameSyntax, context);
                case SyntaxKind.GenericName:
                    return new GenericNameWriter(expression as GenericNameSyntax, context);
                case SyntaxKind.IdentifierName:
                    return new IdentifierNameWriter(expression as IdentifierNameSyntax, context);
                case SyntaxKind.NullableType:
                    return new NullableTypeWriter(expression as NullableTypeSyntax, context);
                case SyntaxKind.OmittedTypeArgument:
                    return new OmittedTypeArgumentWriter(expression as OmittedTypeArgumentSyntax, context);
                case SyntaxKind.PredefinedType:
                    return new PredefinedTypeWriter(expression as PredefinedTypeSyntax, context);
                case SyntaxKind.RefTypeExpression:
                    return new RefTypeWriter(expression as RefTypeSyntax, context);
                default:
                    throw new Exception();
            }
        }

        // Reference: roslyn/src/Compilers/CSharp/Portable/Generated/Syntax.xml.Main.Generated.cs
        public static ContextWriter GetWriter(this StatementSyntax statement, ICompilationContextProvider context)
        {
            var kind = statement.Kind();
            switch (kind)
            {
                case SyntaxKind.Block:
                    return new BlockStatementWriter(statement as BlockSyntax, context);
                case SyntaxKind.BreakStatement:
                    return new BreakStatementWriter(statement as BreakStatementSyntax, context);
                case SyntaxKind.ForEachStatement:
                    return new ForEachStatementWriter(statement as ForEachStatementSyntax, context);
                case SyntaxKind.ContinueStatement:
                    return new ContinueStatementWriter(statement as ContinueStatementSyntax, context);
                case SyntaxKind.DoStatement:
                    return new DoStatementWriter(statement as DoStatementSyntax, context);
                case SyntaxKind.EmptyStatement:
                    return new EmptyStatementWriter(statement as EmptyStatementSyntax, context);
                case SyntaxKind.ExpressionStatement:
                    return new ExpressionStamentWriter(statement as ExpressionStatementSyntax, context);
                case SyntaxKind.ForStatement:
                    return new ForStatementWriter(statement as ForStatementSyntax, context);
                case SyntaxKind.IfStatement:
                    return new IfStatementWriter(statement as IfStatementSyntax, context);
                case SyntaxKind.LocalDeclarationStatement:
                    return new LocalDeclarationStatementWriter(statement as LocalDeclarationStatementSyntax, context);
                case SyntaxKind.LockStatement:
                    return new LockStatementWriter(statement as LockStatementSyntax, context);
                case SyntaxKind.ReturnStatement:
                    return new ReturnStatementWriter(statement as ReturnStatementSyntax, context);
                case SyntaxKind.SwitchStatement:
                    return new SwitchStatementWriter(statement as SwitchStatementSyntax, context);
                case SyntaxKind.ThrowStatement:
                    return new ThrowStatementWriter(statement as ThrowStatementSyntax, context);
                case SyntaxKind.TryStatement:
                    return new TryStatementWriter(statement as TryStatementSyntax, context);
                case SyntaxKind.UsingStatement:
                    return new UsingStatementWriter(statement as UsingStatementSyntax, context);
                case SyntaxKind.WhileStatement:
                    return new WhileStatementWriter(statement as WhileStatementSyntax, context);
                case SyntaxKind.YieldReturnStatement:
                    return new YieldStatementWriter(statement as YieldStatementSyntax, context);
                default:
                    throw new Exception();
            }
        }

        static IEnumerable<ContextWriter> getMethodWriters(MethodDeclarationSyntax method, ICompilationContextProvider context)
        {
            var signatures = method.GetMethodSignatures(context);
            if (signatures.Length == 0)
            {
                yield return new MethodWriter(method, context);
            }
            else
            {
                foreach (var signature in signatures)
                    yield return new SignatureMethodWriter(signature, method, context);
            }
        }

        public static CodeBuilder EndOfStatement(this CodeBuilder builder)
        {
            return builder.AppendLine(";");
        }

        public static CodeBuilder SemiColonSeparator(this CodeBuilder builder)
        {
            return builder.Append("; ");
        }

        public static CodeBuilder Colon(this CodeBuilder builder)
        {
            return builder.Append(":");
        }

        public static CodeBuilder SemiColon(this CodeBuilder builder)
        {
            return builder.Append(";");
        }

        public static CodeBuilder CommaSeparator(this CodeBuilder builder)
        {
            return builder.Append(", ");
        }

        public static CodeBuilder Space(this CodeBuilder builder)
        {
            return builder.Append(" ");
        }

        public static CodeBuilder Parenthesized(this CodeBuilder builder, Action parenthesized)
        {
            builder.Append("(");
            parenthesized();
            return builder.Append(")");
        }

        public static CodeBuilder Parenthesized(this CodeBuilder builder)
        {
            builder.Append("(");
            return builder.UsingChild(")");
        }

        public static CodeBuilder Block(this CodeBuilder builder, bool appendLine = true)
        {
            builder.AppendLine("{");
            return builder.Indent("}", appendLine);
        }

        public static CodeBuilder ParameterList(this CodeBuilder builder)
        {
            builder.AppendLine("(");
            return builder.Indent(")");
        }

        public static CodeBuilder TypeParameterList(this CodeBuilder builder)
        {
            builder.AppendLine("<");
            return builder.Indent(">");
        }
    }
}
