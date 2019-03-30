using CodeBinder.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CodeBinder.Java
{
    abstract class JavaConversionBuilder : ConversionBuilder
    {
        public CSToJavaConversion Conversion { get; private set; }

        public JavaConversionBuilder(CSToJavaConversion conversion)
        {
            Conversion = conversion;
        }
    }
}
