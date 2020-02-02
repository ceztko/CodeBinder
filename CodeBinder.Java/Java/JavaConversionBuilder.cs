using CodeBinder.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CodeBinder.Java
{
    abstract class JavaConversionBuilder : ConversionBuilder
    {
        public ConversionCSharpToJava Conversion { get; private set; }

        public JavaConversionBuilder(ConversionCSharpToJava conversion)
        {
            Conversion = conversion;
        }
    }
}
