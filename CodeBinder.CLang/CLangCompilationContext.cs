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
        Dictionary<string, CLangModuleContextParent> _modules;
        List<EnumDeclarationSyntax> _enums;
        List<ClassDeclarationSyntax> _types;
        List<DelegateDeclarationSyntax> _callbacks;

        protected CLangCompilationContext()
        {
            _modules = new Dictionary<string, CLangModuleContextParent>();
            _enums = new List<EnumDeclarationSyntax>();
            _types = new List<ClassDeclarationSyntax>();
            _callbacks = new List<DelegateDeclarationSyntax>();
        }

        public void AddModule(CompilationContext compilation, CLangModuleContextParent module)
        {
            _modules.Add(module.Name, module);
            AddType(module, null);
        }

        public void AddModuleChild(CompilationContext compilation, CLangModuleContextChild module, CLangModuleContextParent parent)
        {
            AddType(module, parent);
        }

        public bool TryGetModule(string moduleName, [NotNullWhen(true)]out CLangModuleContextParent? module)
        {
            return _modules.TryGetValue(moduleName, out module);
        }

        public void AddEnum(EnumDeclarationSyntax enm)
        {
            _enums.Add(enm);
        }

        public void AddCallback(DelegateDeclarationSyntax callback)
        {
            _callbacks.Add(callback);
        }

        public void AddType(ClassDeclarationSyntax type)
        {
            _types.Add(type);
        }

        protected override INodeVisitor CreateVisitor()
        {
            return new CLangNodeVisitor(this);
        }

        public IEnumerable<CLangModuleContextParent> Modules
        {
            get { return _modules.Values; }
        }

        public IEnumerable<EnumDeclarationSyntax> Enums
        {
            get { return _enums; }
        }

        public IEnumerable<ClassDeclarationSyntax> Types
        {
            get { return _types; }
        }

        public IEnumerable<DelegateDeclarationSyntax> Callbacks
        {
            get { return _callbacks; }
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
