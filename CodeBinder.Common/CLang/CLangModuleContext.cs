using CodeBinder.Shared;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.CLang
{
    public abstract class CLangModuleContext : TypeContext<CLangModuleContext, CLangCompilationContext>
    {
        protected CLangModuleContext(CLangCompilationContext context)
            : base(context) { }

        public abstract string Name
        {
            get;
        }

        public abstract IEnumerable<MethodDeclarationSyntax> Methods
        {
            get;
        }
    }

    public class JNIModuleContextParent : CLangModuleContext
    {
        private string _Name;

        public JNIModuleContextParent(string name, CLangCompilationContext context)
            : base(context)
        {
            _Name = name;
        }

        protected override TypeConversion GetConversion()
        {
            var ret = new CLangModuleConversion(Compilation.Conversion);
            ret.TypeContext = this;
            return ret;
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

    public class JNIModuleContextChild : CLangModuleContext
    {
        private List<MethodDeclarationSyntax> _methods;

        public JNIModuleContextChild(CLangCompilationContext context)
            : base(context)
        {
            _methods = new List<MethodDeclarationSyntax>();
        }

        public void AddNativeMethod(MethodDeclarationSyntax method)
        {
            _methods.Add(method);
        }

        protected override TypeConversion GetConversion()
        {
            return null;
        }

        public override IEnumerable<MethodDeclarationSyntax> Methods
        {
            get { return _methods; }
        }

        public override string Name
        {
            get { return Parent.Name; }
        }
    }
}
