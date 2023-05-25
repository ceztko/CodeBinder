// Copyright(c) 2020 Francesco Pretto
// This file is subject to the MIT license
using CodeBinder.Attributes;
using CodeBinder.Shared;
using CodeBinder.Shared.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace CodeBinder.Apple
{
    public class ObjCCompilationContext : CSharpCompilationContext<ConversionCSharpToObjC>
    {
        public string CLangLibraryHeaderName => $"{LibraryName}.h";

        public string ObjCLibraryHeaderName => $"{ObjCLibraryName}.h";

        public string ObjCLibraryName => $"OC{LibraryName}";

        public ObjCCompilationContext(ConversionCSharpToObjC conversion)
            : base(conversion)

        {
        }

        protected override CSharpClassTypeContext CreateContext(ClassDeclarationSyntax cls)
        {
            return new ObjCClassContext(cls, this);
        }

        protected override CSharpInterfaceTypeContext CreateContext(InterfaceDeclarationSyntax iface)
        {
            return new ObjCInterfaceContext(iface, this);
        }

        protected override CSharpStructTypeContext CreateContext(StructDeclarationSyntax str)
        {
            return new ObjCStructContext(str, this);
        }

        public override IEnumerable<IConversionWriter> DefaultConversions
        {
            get
            {
                yield return new ObjCLibDefsHeaderConversion(this);
                yield return new ObjCTypesHeaderConversion(this, true);
                yield return new ObjCTypesHeaderConversion(this, false);
                yield return new ObjCLibraryHeaderConversion(this, true);
                yield return new ObjCLibraryHeaderConversion(this, false);
            }
        }

        protected override INodeVisitor CreateVisitor()
        {
            return new ObjCNodeVisitor(this);
        }
    }
}
