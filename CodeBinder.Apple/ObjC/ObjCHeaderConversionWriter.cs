using CodeBinder.Shared;
using CodeBinder.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CodeBinder.Apple
{
    abstract class ObjCHeaderConversionWriter : ConversionWriter<ObjCCompilationContext>
    {
        public ObjCHeaderConversionWriter(ObjCCompilationContext compilation)
            : base(compilation)
        {
        }

        protected void BeginHeaderGuard(CodeBuilder builder)
        {
            builder.AppendLine($"#ifndef {HeaderGuard}");
            builder.AppendLine($"#define {HeaderGuard}");
        }

        protected void EndHeaderGuard(CodeBuilder builder)
        {
            builder.AppendLine($"#endif // {HeaderGuard}");
        }

        private string HeaderGuard
        {
            get
            {
                var stem = HeaderGuardStem;
                if (stem.Length == 0)
                    throw new Exception("Stem is empty");

                return $"{Context.ObjCLibraryName.ToUpper()}_{HeaderGuardStem}_HEADER";
            }
        }

        protected virtual string HeaderGuardStem => throw new NotImplementedException();
    }
}
