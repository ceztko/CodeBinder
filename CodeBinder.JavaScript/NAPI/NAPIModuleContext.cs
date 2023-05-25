// SPDX-FileCopyrightText: (C) 2023 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

using CodeBinder.Attributes;

namespace CodeBinder.JavaScript.NAPI;

public abstract class NAPIModuleContext : TypeContext<NAPIModuleContext, NAPICompilationContext>
{
    public override NAPICompilationContext Compilation => _Compilation;

    public NAPICompilationContext _Compilation;

    protected NAPIModuleContext(NAPICompilationContext context)
    {
        _Compilation = context;
    }

    public abstract IEnumerable<MethodDeclarationSyntax> Methods
    {
        get;
    }

    public abstract IEnumerable<ImportAttribute> Includes
    {
        get;
    }
}

public class JNIModuleContextParent : NAPIModuleContext
{
    string _Name;
    List<ImportAttribute> _includes;

    public JNIModuleContextParent(string name, NAPICompilationContext context)
        : base(context)
    {
        _Name = name;
        _includes = new List<ImportAttribute>();
    }

    public void AddInclude(ImportAttribute include)
    {
        _includes.Add(include);
    }

    protected override IEnumerable<TypeConversion<NAPIModuleContext>> GetConversions()
    {
        yield return new NAPIModuleConversion(this, ConversionType.Header, Compilation.Conversion);
        yield return new NAPIModuleConversion(this, ConversionType.Implementation, Compilation.Conversion);
    }

    public override IEnumerable<MethodDeclarationSyntax> Methods
    {
        get
        {
            foreach (var child in Children)
            {
                foreach (var method in child.Methods)
                    yield return method;
            }
        }
    }

    public override string Name
    {
        get { return _Name; }
    }

    public override IEnumerable<ImportAttribute> Includes => _includes;
}

public class JNIModuleContextChild : NAPIModuleContext
{
    List<MethodDeclarationSyntax> _methods;

    public JNIModuleContextChild(NAPICompilationContext context)
        : base(context)
    {
        _methods = new List<MethodDeclarationSyntax>();
    }

    public void AddNativeMethod(MethodDeclarationSyntax method)
    {
        _methods.Add(method);
    }

    protected override IEnumerable<TypeConversion<NAPIModuleContext>> GetConversions()
    {
        throw new NotImplementedException();
    }

    public override IEnumerable<MethodDeclarationSyntax> Methods
    {
        get { return _methods; }
    }

    public override string Name
    {
        get { return Parent!.Name; }
    }

    public override IEnumerable<ImportAttribute> Includes
    {
        get { yield break; }
    }
}
