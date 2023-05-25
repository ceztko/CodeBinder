// Copyright(c) 2020 Francesco Pretto
// This file is subject to the MIT license
using CodeBinder.Attributes;
using CodeBinder.CLang;
using CodeBinder.Shared;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.JNI
{
    public abstract class JNIModuleContext : TypeContext<JNIModuleContext, JNICompilationContext>
    {
        JNICompilationContext _Compilation;

        public override JNICompilationContext Compilation => _Compilation;

        protected JNIModuleContext(JNICompilationContext context)
        {
            _Compilation = context;
        }

        public abstract IEnumerable<MethodDeclarationSyntax> Methods
        {
            get;
        }

        public abstract IEnumerable<ImportAttribute> Includes
        {
            get;
        }
    }

    public class JNIModuleContextParent : JNIModuleContext
    {
        string _Name;
        List<ImportAttribute> _includes;

        public JNIModuleContextParent(string name, JNICompilationContext context)
            : base(context)
        {
            _Name = name;
            _includes = new List<ImportAttribute>();
        }

        public void AddInclude(ImportAttribute include)
        {
            _includes.Add(include);
        }

        protected override IEnumerable<TypeConversion<JNIModuleContext>> GetConversions()
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

        public override IEnumerable<ImportAttribute> Includes => _includes;
    }

    public class JNIModuleContextChild : JNIModuleContext
    {
        List<MethodDeclarationSyntax> _methods;

        public JNIModuleContextChild(JNICompilationContext context)
            : base(context)
        {
            _methods = new List<MethodDeclarationSyntax>();
        }

        public void AddNativeMethod(MethodDeclarationSyntax method)
        {
            _methods.Add(method);
        }

        protected override IEnumerable<TypeConversion<JNIModuleContext>> GetConversions()
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

        public override IEnumerable<ImportAttribute> Includes
        {
            get { yield break; }
        }
    }
}
