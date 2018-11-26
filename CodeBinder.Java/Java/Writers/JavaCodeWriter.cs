using CodeBinder.Shared;
using CodeBinder.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Java
{
    abstract class JavaCodeWriter<TItem> : CodeWriter<TItem, JavaCodeWriterContext>
    {
        protected JavaCodeWriter(TItem item, JavaCodeWriterContext context)
            : base(item, context, context.Provider) { }
    }

    public class JavaCodeWriterContext : ICompilationContextProvider
    {
        public ICompilationContextProvider Provider { get; private set; }
        public CSToJavaConversion Conversion { get; private set; }

        public JavaCodeWriterContext(ICompilationContextProvider provider, CSToJavaConversion conversion)
        {
            Provider = provider;
            Conversion = conversion;
        }

        public CompilationContext Compilation
        {
            get { return Provider.Compilation; }
        }
    }
}
