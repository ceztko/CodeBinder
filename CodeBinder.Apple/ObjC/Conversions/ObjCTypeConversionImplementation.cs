﻿using CodeBinder.Shared;
using CodeBinder.Shared.CSharp;
using CodeBinder.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Apple
{
    partial class ObjCTypeConversion<TTypeContext>
    {
        public abstract class Implementation : ObjCTypeConversion<TTypeContext>
        {
            public ObjImplementationType ImplementationType { get; private set; }

            public Implementation(TTypeContext context, ConversionCSharpToObjC conversion, ObjImplementationType implementationType)
                : base(context, conversion)
            {
                ImplementationType = implementationType;
            }

            protected sealed override string GetFileName() => ImplementationsFilename;

            protected override string? GetBasePath() =>
                ImplementationType == ObjImplementationType.PublicType ? null : ConversionCSharpToObjC.InternalBasePath;

            protected override IEnumerable<string> Imports
            {
                get
                {
                    yield return $"<{Compilation.CLangLibraryHeaderName}>";
                    if (ImplementationType == ObjImplementationType.PublicType)
                        yield return $"\"{ConversionCSharpToObjC.InternalBasePath}/{Compilation.ObjCLibraryHeaderName}\"";
                    else
                        yield return $"\"{Compilation.ObjCLibraryHeaderName}\"";
                }
            }
        }
    }

    class ObjCClassConversionImplementation : ObjCTypeConversion<ObjCClassContext>.Implementation
    {
        public ObjCClassConversionImplementation(ObjCClassContext context,
                ConversionCSharpToObjC conversion, ObjImplementationType implementationType)
            : base(context, conversion, implementationType) { }

        protected override CodeWriter GetTypeWriter()
        {
            return CodeWriter.NullWriter("/* NULL */");
            return new ObjCClassWriter(Context.Node, Context.ComputePartialDeclarationsTree(), Context.Compilation, ObjCFileType.Implementation);
        }
    }

    class ObjCStructConversionImplementation : ObjCTypeConversion<ObjCStructContext>.Implementation
    {
        public ObjCStructConversionImplementation(ObjCStructContext context,
                ConversionCSharpToObjC conversion, ObjImplementationType implementationType)
            : base(context, conversion, implementationType) { }

        protected override CodeWriter GetTypeWriter()
        {
            return CodeWriter.NullWriter("/* NULL */");
            return new ObjCStructWriter(Context.Node, Context.ComputePartialDeclarationsTree(), Context.Compilation, ObjCFileType.Implementation);
        }
    }
}