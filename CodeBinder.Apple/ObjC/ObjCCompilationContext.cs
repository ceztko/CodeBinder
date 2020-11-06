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
using System.Text;

namespace CodeBinder.Apple
{
    public class ObjCCompilationContext : CSharpCompilationContext
    {
        Dictionary<string, EnumDeclarationSyntax> _enums;
        Dictionary<string, BaseTypeDeclarationSyntax> _classes;
        Dictionary<string, InterfaceDeclarationSyntax> _interfaces;
        List<DelegateDeclarationSyntax> _callbacks;
        Dictionary<ISymbol, string> _bindedMethodNames;

        public new ConversionCSharpToObjC Conversion { get; private set; }

        public string CLangLibraryHeaderName => $"{LibraryName}.h";

        public string ObjCLibraryHeaderName => $"{ObjCLibraryName}.h";

        public string ObjCLibraryName => $"OC{LibraryName}";

        public ObjCCompilationContext(ConversionCSharpToObjC conversion)
        {
            Conversion = conversion;
            _enums = new Dictionary<string, EnumDeclarationSyntax>();
            _classes = new Dictionary<string, BaseTypeDeclarationSyntax>();
            _interfaces = new Dictionary<string, InterfaceDeclarationSyntax>();
            _callbacks = new List<DelegateDeclarationSyntax>();
            _bindedMethodNames = new Dictionary<ISymbol, string>(SymbolEqualityComparer.Default);
        }

        public override CSharpClassTypeContext CreateContext(ClassDeclarationSyntax cls)
        {
            return new ObjCClassContext(cls, this);
        }

        public override CSharpInterfaceTypeContext CreateContext(InterfaceDeclarationSyntax iface)
        {
            return new ObjCInterfaceContext(iface, this);
        }

        public override CSharpStructTypeContext CreateContext(StructDeclarationSyntax str)
        {
            return new ObjCStructContext(str, this);
        }

        public void AddInterface(InterfaceDeclarationSyntax iface)
        {
            _interfaces.TryAdd(iface.GetFullName(this), iface);
        }

        public void AddClass(BaseTypeDeclarationSyntax classType)
        {
            _classes.TryAdd(classType.GetFullName(this), classType);
        }

        public void AddEnum(EnumDeclarationSyntax enm)
        {
            _enums.TryAdd(enm.GetFullName(this), enm);
        }

        public bool IsCompilationDefinedType(ITypeSymbol symbol)
        {
            string fullName = symbol.GetFullName();
            if (_classes.ContainsKey(fullName))
                return true;

            if (_interfaces.ContainsKey(fullName))
                return true;

            if (_enums.ContainsKey(fullName))
                return true;

            return false;
        }

        public void AddCallback(DelegateDeclarationSyntax callback)
        {
            _callbacks.Add(callback);
        }

        public void AddMethodBinding(IMethodSymbol symbol, string bindedName)
        {
            if (Conversion.MethodsLowerCase)
                bindedName = bindedName.ToObjCCase();

            _bindedMethodNames.Add(symbol, bindedName);
        }

        public bool TryGetBindedName(IMethodSymbol symbol, [NotNullWhen(true)]out string? name)
        {
            if (_bindedMethodNames.TryGetValue(symbol, out name))
                return true;

            return false;
        }

        public IEnumerable<EnumDeclarationSyntax> Enums
        {
            get { return _enums.Values; }
        }

        public IEnumerable<BaseTypeDeclarationSyntax> Classes
        {
            get { return _classes.Values; }
        }

        public IEnumerable<InterfaceDeclarationSyntax> Interfaces
        {
            get { return _interfaces.Values; }
        }

        public IEnumerable<DelegateDeclarationSyntax> Callbacks
        {
            get { return _callbacks; }
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

        protected override CSharpLanguageConversion getLanguageConversion() => Conversion;

        protected override INodeVisitor CreateVisitor()
        {
            return new ObjCNodeVisitor(this);
        }
    }
}
