using CodeBinder.Shared;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.JNI
{
    public abstract class JNIModuleContext : TypeContext<JNIModuleContext, JNISyntaxTreeContext>
    {
        public CSToJNIConversion LanguageConversion { get; internal set; }

        protected JNIModuleContext(JNISyntaxTreeContext context)
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

    public class JNIModuleContextParent : JNIModuleContext
    {
        private string _Name;

        public JNIModuleContextParent(string name)
            : base(null)
        {
            _Name = name;
        }

        protected override TypeConversion GetConversion()
        {
            var ret = new JNIModuleConversion(LanguageConversion);
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

    public class JNIModuleContextChild : JNIModuleContext
    {
        private List<MethodDeclarationSyntax> _methods;

        public JNIModuleContextChild(JNISyntaxTreeContext context)
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
