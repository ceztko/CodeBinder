// Copyright(c) 2020 Francesco Pretto
// This file is subject to the MIT license
using CodeBinder.Shared;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.JNI
{
    public abstract class JNIModuleContext : TypeContext<JNIModuleContext, JNICompilationContext>
    {
        public JNICompilationContext Context { get; private set; }

        protected JNIModuleContext(JNICompilationContext context)
        {
            Context = context;
        }

        public abstract IEnumerable<MethodDeclarationSyntax> Methods
        {
            get;
        }

        protected override JNICompilationContext getCompilationContext() => Context;
    }

    public class JNIModuleContextParent : JNIModuleContext
    {
        private string _Name;

        public JNIModuleContextParent(string name, JNICompilationContext context)
            : base(context)
        {
            _Name = name;
        }

        protected override IEnumerable<TypeConversion<JNIModuleContext>> getConversions()
        {
            yield return new JNIModuleConversion(this, ConversionType.Header, Compilation.Conversion);
            yield return new JNIModuleConversion(this, ConversionType.Implementation, Compilation.Conversion);
        }

        public override IEnumerable<MethodDeclarationSyntax> Methods
        {
            get
            {
                foreach (var child in Children)
                {
                    foreach (var method in child.Methods)
                        yield return method;
                }
            }
        }

        public override string Name
        {
            get { return _Name; }
        }
    }

    public class JNIModuleContextChild : JNIModuleContext
    {
        private List<MethodDeclarationSyntax> _methods;

        public JNIModuleContextChild(JNICompilationContext context)
            : base(context)
        {
            _methods = new List<MethodDeclarationSyntax>();
        }

        public void AddNativeMethod(MethodDeclarationSyntax method)
        {
            _methods.Add(method);
        }

        protected override IEnumerable<TypeConversion<JNIModuleContext>> getConversions()
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<MethodDeclarationSyntax> Methods
        {
            get { return _methods; }
        }

        public override string Name
        {
            get { return Parent!.Name; }
        }
    }
}
