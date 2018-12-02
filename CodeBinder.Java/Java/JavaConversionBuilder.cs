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

        string _Basepath;

        public JavaConversionBuilder(CSToJavaConversion conversion)
        {
            Conversion = conversion;
            _Basepath = string.IsNullOrEmpty(conversion.BaseNamespace) ? null : conversion.BaseNamespace.Replace('.', Path.DirectorySeparatorChar);
        }

        public override string BasePath
        {
            get { return _Basepath; }
        }
    }
}
