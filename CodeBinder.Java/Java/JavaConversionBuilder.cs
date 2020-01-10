using CodeBinder.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CodeBinder.Java
{
    abstract class JavaConversionBuilder : ConversionBuilderBase
    {
        public ConversionCSharpToJava Conversion { get; private set; }

        public JavaConversionBuilder(ConversionCSharpToJava conversion)
        {
            Conversion = conversion;
        }
    }
}
