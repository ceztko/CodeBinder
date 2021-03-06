﻿// Copyright(c) 2020 Francesco Pretto
// This file is subject to the MIT license
using CodeBinder.Shared;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.CLang
{
    public abstract class CLangModuleContext : TypeContext<CLangModuleContext, CLangCompilationContext>
    {
        public new CLangCompilationContext Compilation { get; private set; }

        protected CLangModuleContext(CLangCompilationContext context)
        {
            Compilation = context;
        }

        public abstract IEnumerable<MethodDeclarationSyntax> Methods
        {
            get;
        }

        protected sealed override CLangCompilationContext getCompilationContext() => Compilation;
    }

    public class CLangModuleContextParent : CLangModuleContext
    {
        private string _Name;

        public CLangModuleContextParent(string name, CLangCompilationContext context)
            : base(context)
        {
            _Name = name;
        }

        protected override IEnumerable<TypeConversion<CLangModuleContext>> getConversions()
        {
            yield return new CLangModuleConversion(this, ModuleConversionType.CHeader, Compilation.Conversion);
            if (!Compilation.Conversion.PublicInterfaceOnly)
                yield return new CLangModuleConversion(this, ModuleConversionType.CppTrampoline, Compilation.Conversion);
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

    public class CLangModuleContextChild : CLangModuleContext
    {
        private List<MethodDeclarationSyntax> _methods;

        public CLangModuleContextChild(CLangCompilationContext context)
            : base(context)
        {
            _methods = new List<MethodDeclarationSyntax>();
        }

        public void AddNativeMethod(MethodDeclarationSyntax method)
        {
            _methods.Add(method);
        }

        protected override IEnumerable<TypeConversion<CLangModuleContext>> getConversions()
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
