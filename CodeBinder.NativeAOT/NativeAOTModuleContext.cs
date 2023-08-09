// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.NativeAOT;

public abstract class NativeAOTModuleContext : TypeContext<NativeAOTModuleContext, NativeAOTCompilationContext>
{
    NativeAOTCompilationContext _Compilation;

    public override NativeAOTCompilationContext Compilation => _Compilation;

    protected NativeAOTModuleContext(NativeAOTCompilationContext context)
    {
        _Compilation = context;
    }

    public abstract IEnumerable<MethodDeclarationSyntax> Methods
    {
        get;
    }
}

public class CLangModuleContextParent : NativeAOTModuleContext
{
    private string _Name;

    public CLangModuleContextParent(string name, NativeAOTCompilationContext context)
        : base(context)
    {
        _Name = name;
    }

    protected override IEnumerable<TypeConversion<NativeAOTModuleContext>> GetConversions()
    {
        yield return new NativeAOTModuleConversion(this, Compilation.Conversion);
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
}

public class CLangModuleContextChild : NativeAOTModuleContext
{
    private List<MethodDeclarationSyntax> _methods;

    public CLangModuleContextChild(NativeAOTCompilationContext context)
        : base(context)
    {
        _methods = new List<MethodDeclarationSyntax>();
    }

    public void AddNativeMethod(MethodDeclarationSyntax method)
    {
        _methods.Add(method);
    }

    protected override IEnumerable<TypeConversion<NativeAOTModuleContext>> GetConversions()
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
}
