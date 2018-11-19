using CodeTranslator.Shared;
using CodeTranslator.Util;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CodeTranslator.Shared.CSharp;
using System;
using System.Collections.Generic;
using System.Text;
using CodeTranslator.Shared.Java;
using Microsoft.CodeAnalysis;

namespace CodeTranslator.Java
{
    abstract class MethodWriter<TMethod> : CodeWriter<TMethod>
        where TMethod : BaseMethodDeclarationSyntax
    {
        protected MethodWriter(TMethod method, ICompilationContextProvider context)
            : base(method, context) { }

        protected override void Write()
        {
            WriteModifiers();
            if (Arity != 0)
                WriteTypeParameters();

            WriteReturnType();
            Builder.Append(MethodName);
            WriteParameters();
            writeMethodBody();
        }

        protected virtual void WriteParameters()
        {
            if (Context.ParameterList.Parameters.Count == 0)
            {
                Builder.Append("()");
            }
            else if (Context.ParameterList.Parameters.Count == 1)
            {
                using (Builder.ParameterList())
                {
                    writeParameters(Context.ParameterList);
                }
            }
            else
            {
                using (Builder.Indent())
                {
                    using (Builder.ParameterList(true))
                    {
                        writeParameters(Context.ParameterList);
                        Builder.AppendLine();
                    }
                }
            }
        }

        protected virtual void WriteModifiers()
        {
            string modifiers = Context.GetJavaModifiersString();
            if (!string.IsNullOrEmpty(modifiers))
                Builder.Append(modifiers).Space();
        }

        protected void WriteType(TypeSyntax type, JavaTypeFlags flags)
        {
            Builder.Append(type.GetJavaType(flags, this));
        }

        void writeMethodBody()
        {
            if (Context.Body == null || !WriteMethodBody)
            {
                Builder.EndOfStatement();
            }
            else
            {
                using (Builder.AppendLine().Block())
                {
                    WriteMethodBodyInternal();
                    if (!CSToJavaConversion.SkipBody)
                        Builder.Append(Context.Body, this, true).AppendLine();
                }
            }
        }

        private void writeParameters(ParameterListSyntax list)
        {
            bool first = true;
            foreach (var parameter in list.Parameters)
            {
                if (first)
                    first = false;
                else
                    Builder.AppendLine(",");

                writeParameter(parameter);
            }
        }

        private void writeParameter(ParameterSyntax parameter)
        {
            var flags = IsNative ? JavaTypeFlags.NativeMethod : JavaTypeFlags.None;
            bool isRef = parameter.IsRef() | parameter.IsOut();
            if (isRef)
                flags |= JavaTypeFlags.IsByRef;

            WriteType(parameter.Type, flags);
            Builder.Space().Append(parameter.Identifier.Text);
        }

        protected virtual void WriteTypeParameters() { /* Do nothing */ }

        protected virtual void WriteMethodBodyInternal() { /* Do nothing */ }

        protected virtual void WriteReturnType() { /* Do nothing */ }

        public virtual bool WriteMethodBody
        {
            get { return true; }
        }

        public virtual int Arity
        {
            get { return 0; }
        }

        public abstract string MethodName { get; }

        public abstract bool IsNative { get; }
    }

    class MethodWriter : MethodWriter<MethodDeclarationSyntax>
    {
        static Dictionary<string, Dictionary<string, string>> _replacements;

        static MethodWriter()
        {
            _replacements = new Dictionary<string, Dictionary<string, string>>()
            {
                // java.lang.Object
                { "System.Object", new Dictionary<string, string>() {
                    { "GetHashCode", "hashCode" },
                    { "Equals", "hashCode" },
                    { "Clone", "clone" },
                    { "ToString", "toString" },
                } },
                // java.lang.AutoCloseable
                { "System.IDisposable", new Dictionary<string, string>() { { "Dispose", "close" } } },
                // java.lang.Iterable<T>
                { "System.Collections.Generic.IEnumerable<out T>", new Dictionary<string, string>() { { "GetEnumerator", "iterator" } } },
            };
        }

        public MethodWriter(MethodDeclarationSyntax method, ICompilationContextProvider context)
            : base(method, context) { }

        protected override void WriteModifiers()
        {
            if (IsParentInterface)
                return;

            base.WriteModifiers();
        }

        protected override void WriteTypeParameters()
        {
            Builder.Append(Context.GetTypeParameters(), this).Space();
        }

        protected override void WriteReturnType()
        {
            WriteType(Context.ReturnType, IsNative ? JavaTypeFlags.NativeMethod : JavaTypeFlags.None);
            Builder.Space();
        }

        protected override void WriteMethodBodyInternal()
        {
            if (CSToJavaConversion.SkipBody)
                Builder.Append(Context.ReturnType.GetJavaDefaultReturnStatement(this)).EndOfStatement();
        }

        public bool IsParentInterface
        {
            get { return Context.Parent.Kind() == SyntaxKind.InterfaceDeclaration; }
        }

        public override string MethodName
        {
            get
            {
                // Try first interface replacements
                var methodSymbol = (IMethodSymbol)Context.GetDeclaredSymbol(this);
                var containingType = methodSymbol.ContainingType;
                foreach (var iface in containingType.AllInterfaces)
                {
                    string ifaceName = iface.GetFullName();
                    if (_replacements.TryGetValue(ifaceName, out var replacements))
                    {
                        foreach (var member in iface.GetMembers())
                        {
                            if (member.Kind != SymbolKind.Method)
                                continue;

                            if (replacements.TryGetValue(methodSymbol.Name, out var replacement))
                            {
                                if (containingType.FindImplementationForInterfaceMember(member) == methodSymbol)
                                    return replacement;
                            }
                        }
                    }
                }

                if (methodSymbol.OverriddenMethod != null)
                {
                    var overridenMethodContaningType = methodSymbol.OverriddenMethod.ContainingType.GetFullName();
                    if (_replacements.TryGetValue(overridenMethodContaningType, out var replacements))
                    {
                        if (replacements.TryGetValue(methodSymbol.Name, out var replacement))
                            return replacement;
                    }
                }

                var methodName = Context.GetName();
                if (IsNative)
                    return methodName;
                else
                    return methodName.ToJavaCase();
            }
        }

        public override bool IsNative
        {
            get { return Context.IsNative(this); }
        }

        public override int Arity
        {
            get { return Context.Arity; }
        }
    }

    class SignatureMethodWriter : MethodWriter<MethodDeclarationSyntax>
    {
        MethodSignatureInfo _signature;

        public SignatureMethodWriter(MethodSignatureInfo signature, MethodDeclarationSyntax method, ICompilationContextProvider context)
            : base(method, context)
        {
            _signature = signature;
        }

        protected override void WriteModifiers()
        {
            Builder.Append(_signature.GetJavaModifiersString()).Space();
        }

        protected override void WriteReturnType()
        {
            Builder.Append(_signature.ReturnType.GetJavaType(JavaTypeFlags.NativeMethod)).Space();
        }

        protected override void WriteMethodBodyInternal()
        {
            Builder.EndOfStatement();
        }

        protected override void WriteParameters()
        {
            for (int i = 0; i < _signature.Parameters.Length; i++)
                WriteParameter(ref _signature.Parameters[i]);
        }

        private void WriteParameter(ref MethodParameterInfo parameter)
        {
            Builder.CommaSeparator().Append(parameter.GetJavaType(JavaTypeFlags.NativeMethod)).Space().Append(parameter.Name);
        }

        public override string MethodName
        {
            get { return _signature.MethodName; }
        }

        public override bool WriteMethodBody
        {
            get { return false; }
        }

        public override bool IsNative
        {
            get { return true; } // TODO: Check if the method is really native?
        }
    }

    class ConstructorWriter : MethodWriter<ConstructorDeclarationSyntax>
    {
        bool _isStatic;

        public ConstructorWriter(ConstructorDeclarationSyntax method, ICompilationContextProvider context)
            : base(method, context)
        {
            _isStatic = Context.Modifiers.Any(SyntaxKind.StaticKeyword);
        }

        protected override void WriteModifiers()
        {
            if (!_isStatic)
                base.WriteModifiers();
        }

        protected override void WriteParameters()
        {
            if (!_isStatic)
                base.WriteParameters();
        }

        protected override void WriteMethodBodyInternal()
        {
            if (Context.Initializer != null)
                Builder.Append(Context.Initializer, this).EndOfStatement();
        }

        public override string MethodName
        {
            get
            {
                if (_isStatic)
                    return "static";
                else
                    return (Context.Parent as BaseTypeDeclarationSyntax).GetName();
            }
        }

        public override bool IsNative
        {
            get { return false; }
        }
    }

    class DestructorWriter : MethodWriter<DestructorDeclarationSyntax>
    {
        public DestructorWriter(DestructorDeclarationSyntax method, ICompilationContextProvider context)
            : base(method, context) { }

        protected override void WriteMethodBodyInternal()
        {
            base.WriteMethodBodyInternal();
            Builder.Append("super.finalize()").EndOfStatement();
        }

        protected override void WriteModifiers()
        {
            Builder.Append("protected").Space();
        }

        protected override void WriteReturnType()
        {
            Builder.Append("void").Space();
        }

        public override string MethodName
        {
            get { return "finalize"; }
        }

        public override bool IsNative
        {
            get { return false; }
        }
    }
}
