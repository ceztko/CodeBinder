using CodeTranslator.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.JNI
{
    public class JNIModuleContext : TypeContext<JNIModuleContext, JNISyntaxTreeContext>
    {
        public JNIModuleContext(JNISyntaxTreeContext context)
            : base(context)
        {

        }

        protected override TypeConversion GetConversion()
        {
            throw new NotImplementedException();
        }
    }
}
