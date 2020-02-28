using CodeBinder.Shared;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CodeBinder.Shared.CSharp;
using Microsoft.CodeAnalysis;
using System.Diagnostics;
using System;

namespace CodeBinder.Apple
{
    abstract class MethodWriter<TMethod> : ObjCCodeWriter<TMethod>
        where TMethod : BaseMethodDeclarationSyntax
    {
        public bool IsStatic { get; private set; }

        protected MethodWriter(TMethod method, ObjCCompilationContext context, ObjCFileType fileType)
            : base(method, context, fileType)
        {
            IsStatic = Item.IsStatic(Context);
        }

        protected override void Write()
        {
            WriteModifiers();
            WriteReturnType();
            Builder.Append(MethodName);
            WriteParameters();
            WriteMethodBody();
        }

        protected virtual void WriteParameters()
        {
            int parameterCount = ParameterCount;
            if (parameterCount == 1)
            {
                writeParameters(Item.ParameterList, parameterCount);
            }
            else if (parameterCount > 1)
            {
                using (Builder.Indent())
                {
                    using (Builder.Indent())
                    {
                        writeParameters(Item.ParameterList, parameterCount);
                        Builder.AppendLine();
                    }
                }
            }
        }

        protected virtual void WriteModifiers()
        {
            if (IsStatic)
                Builder.Append("+");
            else
                Builder.Append("-");

            Builder.Space();
        }

        protected void WriteType(TypeSyntax type, ObjCTypeUsageKind kind)
        {
            Builder.Append(type.GetObjCType(kind, Context));
        }

        void WriteMethodBody()
        {
            if (FileType.IsHeader())
            {
                Builder.EndOfStatement();
            }
            else
            {
                using (Builder.AppendLine().Block())
                {
                    if (Item.Body == null)
                    {
                        Debug.Assert(Item.IsAbstract(Context));
                        // Objective C doesn't have abstract properties
                        Builder.Append("@throw [NSException exceptionWithName:@\"Not implemented\" reason:nil userInfo:nil]").EndOfStatement();
                    }
                    else
                    {
                        WriteMethodBodyPrefixInternal();
                        if (DoWriteMethodBody && !Context.Conversion.SkipBody)
                            Builder.Append(Item.Body, Context, true).AppendLine();
                        WriteMethodBodyPostfixInternal();
                    }
                }
            }
        }

        private void writeParameters(ParameterListSyntax list, int parameterCount)
        {
            bool first = true;
            for (int i = 0; i < parameterCount; i++)
            {
                Builder.AppendLine(ref first);
                var parameter = list.Parameters[i];
                Builder.Colon();
                writeParameter(parameter);
            }
        }

        private void writeParameter(ParameterSyntax parameter)
        {
            bool isRef = parameter.IsRef() | parameter.IsOut();
            Builder.Parenthesized(() =>
                WriteType(parameter.Type!, isRef ? ObjCTypeUsageKind.DeclarationByRef : ObjCTypeUsageKind.Declaration)
            );
            Builder.Append(parameter.Identifier.Text);
        }

        public override ObjWriterType Type => ObjWriterType.Method;

        protected virtual void WriteMethodBodyPrefixInternal() { /* Do nothing */ }

        protected virtual void WriteMethodBodyPostfixInternal() { /* Do nothing */ }

        protected void WriteReturnType()
        {
            Builder.Parenthesized(writeReturnType);
        }

        protected virtual void writeReturnType()
        {
            Builder.Append("void");
        }

        public virtual bool DoWriteMethodBody
        {
            get { return true; }
        }

        public virtual int ParameterCount
        {
            get { return Item.ParameterList.Parameters.Count; }
        }

        public abstract string MethodName { get; }
    }

    class MethodWriter : MethodWriter<MethodDeclarationSyntax>
    {
        int _optionalIndex;

        public MethodWriter(MethodDeclarationSyntax method, int optionalIndex,
            ObjCCompilationContext context, ObjCFileType fileType)
            : base(method, context, fileType)
        {
            _optionalIndex = optionalIndex;
        }

        protected override void writeReturnType()
        {
            WriteType(Item.ReturnType, ObjCTypeUsageKind.Declaration);
        }

        protected override void WriteMethodBodyPrefixInternal()
        {
            if (_optionalIndex >= 0)
            {
                var typeSymbol = Item.ReturnType.GetTypeSymbol(Context);
                if (typeSymbol.SpecialType != SpecialType.System_Void)
                    Builder.Append("return").Space();

                using (Builder.MethodCall())
                {
                    Builder.Append("self").Space().Append(MethodName).Colon();
                    for (int i = 0; i < Item.ParameterList.Parameters.Count; i++)
                    {
                        var parameter = Item.ParameterList.Parameters[i];
                        if (i > 0)
                            Builder.Space().Colon();

                        if (i < _optionalIndex)
                            Builder.Append(parameter.Identifier.Text);
                        else
                            Builder.Append(parameter.Default!.Value, Context);
                    }
                }

                Builder.EndOfStatement();
            }
            else
            {
                if (Context.Conversion.SkipBody)
                    Builder.Append(Item.ReturnType.GetObjCDefaultReturnStatement(Context)).EndOfStatement();
            }
        }

        public bool IsParentInterface
        {
            get { return Item.Parent.Kind() == SyntaxKind.InterfaceDeclaration; }
        }

        public override string MethodName
        {
            get
            {
                // Try first look for replacements
                var methodSymbol = Item.GetDeclaredSymbol<IMethodSymbol>(Context);
                if (methodSymbol.HasObjCReplacement(out var replacement))
                    return replacement.Name;

                var methodName = methodSymbol.GetObjCName(Context);
                if (Context.Conversion.MethodsLowerCase)
                    return methodName.ToObjCCase();
                else
                    return methodName;

            }
        }

        public override bool DoWriteMethodBody
        {
            // Don't write true method body if it's just an optional parameter trampoline
            get { return _optionalIndex == -1; }
        }

        public override int ParameterCount
        {
            get
            {
                if (_optionalIndex == -1)
                    return base.ParameterCount;

                return _optionalIndex;
            }
        }
    }

    class ObjCConstructorWriter : MethodWriter<ConstructorDeclarationSyntax>
    {
        public ObjCConstructorWriter(ConstructorDeclarationSyntax method,
            ObjCCompilationContext context, ObjCFileType fileType)
            : base(method, context, fileType) { }

        protected override void WriteParameters()
        {
            if (!IsStatic)
                base.WriteParameters();
        }

        protected override void writeReturnType()
        {
            if (IsStatic)
                Builder.Append("void");
            else
                Builder.Append("id");
        }

        protected override void WriteMethodBodyPrefixInternal()
        {
            if (IsStatic)
            {
                var method = Item.GetDeclaredSymbol<IMethodSymbol>(Context);
                Builder.Append($"if (self != [{method.ContainingType.GetObjCName(Context)} class])").AppendLine()
                    .IndentChild().Append("return").EndOfStatement().Close();
            }
            else
            {
                Builder.Append("self = ");
                if (Item.Initializer == null)
                {
                    Builder.Append("[super init]").EndOfStatement();
                }
                else
                {
                    using (Builder.MethodCall())
                    {
                        string selfIdentifier;
                        switch (Item.Initializer.ThisOrBaseKeyword.Kind())
                        {
                            case SyntaxKind.ThisKeyword:
                                selfIdentifier = "self";
                                break;
                            case SyntaxKind.BaseKeyword:
                                selfIdentifier = "super";
                                break;
                            default:
                                throw new Exception();
                        }

                        Builder.Append(selfIdentifier).Space().Append("init").Append(Item.Initializer.ArgumentList, Context);
                    }
                    Builder.EndOfStatement();
                }
                Builder.Append("if (self == nil)").AppendLine()
                    .IndentChild().Append("return nil").EndOfStatement().Close();
            }
        }

        protected override void WriteMethodBodyPostfixInternal()
        {
            if (!IsStatic)
                Builder.Append("return self").EndOfStatement();
        }

        public override string MethodName
        {
            get
            {
                if (IsStatic)
                    return "initialize";
                else
                    return "init";
            }
        }
    }

    // NOTE: Not needed to call [super dealloc]
    // https://stackoverflow.com/a/25377583/213871
    class ObjCDestructorWriter : MethodWriter<DestructorDeclarationSyntax>
    {
        public ObjCDestructorWriter(DestructorDeclarationSyntax method,
            ObjCCompilationContext context, ObjCFileType fileType)
            : base(method, context, fileType) { }

        public override string MethodName
        {
            get { return "dealloc"; }
        }
    }
}
