// Copyright(c) 2020 Francesco Pretto
// This file is subject to the MIT license
using CodeBinder.Attributes;
using CodeBinder.Shared;
using CodeBinder.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace CodeBinder.CLang
{
    public abstract class CLangCompilationContext : CompilationContext<CLangModuleContext, ConversionCSharpToCLang>
    {
        Dictionary<string, CLangModuleContextParent> _Modules;
        List<EnumDeclarationSyntax> _Enums;
        List<ClassDeclarationSyntax> _OpaqueTypes;
        List<StructDeclarationSyntax> _StructTypes;
        List<DelegateDeclarationSyntax> _Callbacks;

        protected CLangCompilationContext()
        {
            _Modules = new Dictionary<string, CLangModuleContextParent>();
            _Enums = new List<EnumDeclarationSyntax>();
            _OpaqueTypes = new List<ClassDeclarationSyntax>();
            _StructTypes = new List<StructDeclarationSyntax>();
            _Callbacks = new List<DelegateDeclarationSyntax>();
        }

        public void AddModule(CompilationContext compilation, CLangModuleContextParent module)
        {
            _Modules.Add(module.Name, module);
            AddType(module, null);
        }

        public void AddModuleChild(CompilationContext compilation, CLangModuleContextChild module, CLangModuleContextParent parent)
        {
            AddType(module, parent);
        }

        public bool TryGetModule(string moduleName, [NotNullWhen(true)]out CLangModuleContextParent? module)
        {
            return _Modules.TryGetValue(moduleName, out module);
        }

        public void AddEnum(EnumDeclarationSyntax enm)
        {
            _Enums.Add(enm);
        }

        public void AddCallback(DelegateDeclarationSyntax callback)
        {
            _Callbacks.Add(callback);
        }

        public void AddType(ClassDeclarationSyntax type)
        {
            _OpaqueTypes.Add(type);
        }

        public void AddType(StructDeclarationSyntax type)
        {
            _StructTypes.Add(type);
        }

        protected override INodeVisitor CreateVisitor()
        {
            return new CLangNodeVisitor(this);
        }

        public IReadOnlyCollection<CLangModuleContextParent> Modules
        {
            get { return _Modules.Values; }
        }

        public IReadOnlyList<EnumDeclarationSyntax> Enums
        {
            get { return _Enums; }
        }

        public IReadOnlyList<ClassDeclarationSyntax> OpaqueTypes
        {
            get { return _OpaqueTypes; }
        }

        public IReadOnlyList<StructDeclarationSyntax> StructTypes
        {
            get { return _StructTypes; }
        }

        public IReadOnlyList<DelegateDeclarationSyntax> Callbacks
        {
            get { return _Callbacks; }
        }

        public override IEnumerable<IConversionWriter> DefaultConversions
        {
            get
            {
                yield return new CLangLibraryHeaderConversion(this);
                yield return new CLangLibDefsHeaderConversion(this);
                yield return new CLangTypesHeaderConversion(this);
                yield return new CLangMethodInitConversion(this);
            }
        }
    }

    class CLangCompilationContextImpl : CLangCompilationContext
    {
        public new ConversionCSharpToCLang Conversion { get; private set; }

        public CLangCompilationContextImpl(ConversionCSharpToCLang conversion)
        {
            Conversion = conversion;
        }

        protected override ConversionCSharpToCLang getLanguageConversion() => Conversion;
    }
}
