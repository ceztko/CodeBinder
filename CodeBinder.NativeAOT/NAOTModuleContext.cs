// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.NativeAOT;

public abstract class NAOTModuleContext : TypeContext<NAOTModuleContext, NAOTCompilationContext>
{
    NAOTCompilationContext _Compilation;

    public override NAOTCompilationContext Compilation => _Compilation;

    protected NAOTModuleContext(NAOTCompilationContext context)
    {
        _Compilation = context;
    }

    public abstract IEnumerable<MethodDeclarationSyntax> Methods
    {
        get;
    }
}

public class NAOTModuleContextParent : NAOTModuleContext
{
    private string _Name;

    public NAOTModuleContextParent(string name, NAOTCompilationContext context)
        : base(context)
    {
        _Name = name;
    }

    protected override IEnumerable<TypeConversion<NAOTModuleContext>> GetConversions()
    {
        if (Compilation.Conversion.CreateTemplate)
            yield return new NAOTModuleConversion(this, true, Compilation.Conversion);
        else
            yield return new NAOTModuleConversion(this, false, Compilation.Conversion);
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

public class NAOTModuleContextChild : NAOTModuleContext
{
    private List<MethodDeclarationSyntax> _methods;

    public NAOTModuleContextChild(NAOTCompilationContext context)
        : base(context)
    {
        _methods = new List<MethodDeclarationSyntax>();
    }

    public void AddNativeMethod(MethodDeclarationSyntax method)
    {
        _methods.Add(method);
    }

    protected override IEnumerable<TypeConversion<NAOTModuleContext>> GetConversions()
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
