using System;
using System.Collections.Generic;
using System.Text;
using CodeTranslator.Util;

namespace CodeTranslator.Shared
{
    public class NullTypeConversion<TTypeContext> : TypeConversion<TTypeContext>
        where TTypeContext : TypeContext
    {
        public override IEnumerable<IConversionBuilder> Builders
        {
            get { yield break; }
        }

        public override string FileName => throw new NotImplementedException();

        public override void Write(CodeBuilder builder)
        {
            throw new NotImplementedException();
        }
    }
}
