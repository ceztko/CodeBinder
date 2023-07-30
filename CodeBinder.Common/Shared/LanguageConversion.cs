// SPDX-FileCopyrightText: (C) 2018 Francesco Pretto<ceztko@gmail.com>
// SPDX-FileCopyrightText: (C) 2017-2018 ICSharpCode
// SPDX-License-Identifier: MIT
using CodeBinder.Attributes;

namespace CodeBinder.Shared;

/// <summary>
/// Basic language conversion
/// </summary>
/// <remarks>Inherit this class to provide custom contexts</remarks>
public abstract class LanguageConversion<TCompilationContext, TTypeContext, TVisitor> : LanguageConversion
    where TCompilationContext : CompilationContext<TTypeContext, TVisitor>
    where TTypeContext : TypeContext<TTypeContext>
    where TVisitor : NodeVisitor
{
    protected LanguageConversion() { }

    internal protected abstract override TCompilationContext CreateCompilationContext();

    internal protected abstract override ValidationContext<TVisitor>? CreateValidationContext();
}

/// <summary>
/// Validation context that's used to validate a compilation
/// </summary>
/// <remarks>This class is for infrastructure only</remarks>
public abstract class LanguageConversion
{
    NamespaceMappingTree _NamespaceMapping;

    internal LanguageConversion()
    {
        _NamespaceMapping = new NamespaceMappingTree();
    }

    internal protected abstract NodeVisitor CreateVisitor();

    internal protected abstract ValidationContext? CreateValidationContext();

    /// <summary>
    /// Add here policies supported by this language conversion
    /// </summary>
    public virtual IReadOnlyCollection<string> SupportedPolicies
    {
        get { return new string[] { }; }
    }

    public virtual string GetMethodBaseName(IMethodSymbol symbol)
    {
        string baseName;
        if (symbol.ExplicitInterfaceImplementations.Length == 0)
        {
            baseName = symbol.Name;
        }
        else
        {
            // Get name of explicitly interface implemented method
            baseName = symbol.ExplicitInterfaceImplementations[0].Name;
        }

        HandleMethodCasing(symbol, ref baseName);
        return baseName;
    }

    /// <summary>
    /// Supported overload features
    /// </summary>
    public virtual OverloadFeature? OverloadFeatures => null;

    /// <summary>
    /// True for native conversions (eg. CLang)
    /// </summary>
    public virtual bool IsNative => false;

    /// <summary>
    /// Default discard native syntax (eg. [DllImport] methods or [UnmanagedFunctionPointer] delegates)
    /// </summary>
    public virtual bool DiscardNative => false;

    /// <summary>
    /// False if namespace mapping is needed for this conversion
    /// </summary>
    public virtual bool NeedNamespaceMapping => true;

    public virtual bool UseUTF8Bom => true;

    public bool CheckMethodOverloads
    {
        get { return OverloadFeatures == null ? false : OverloadFeatures != OverloadFeature.FullSupport; }
    }

    /// <summary>
    /// Method name casing
    /// </summary>
    public virtual MethodCasing MethodCasing => MethodCasing.Undefinied;

    /// <summary>
    /// Namespace mapping store
    /// </summary>
    public NamespaceMappingTree NamespaceMapping
    {
        get
        {
            if (!NeedNamespaceMapping)
                throw new Exception("Namespace mapping not supported for this conversion");

            return _NamespaceMapping;
        }
    }

    public virtual IEnumerable<IConversionWriter> DefaultConversions
    {
        get { yield break; }
    }

    public virtual bool TryParseExtraArgs(List<string> args)
    {
        return false;
    }

    /// <summary>Conditional compilation symbols</summary>
    public virtual IReadOnlyList<string> PreprocessorDefinitions
    {
        get { return new string[0]; }
    }

    internal IEnumerable<ConversionDelegate> DefaultConversionDelegates
    {
        get
        {
            foreach (var conversion in DefaultConversions)
                yield return new ConversionDelegate(conversion);
        }
    }

    internal protected abstract CompilationContext CreateCompilationContext();

    internal void HandleMethodCasing(IMethodSymbol symbol, ref string methodName)
    {
        if (symbol.IsNative())
            return;

        switch (MethodCasing)
        {
            case MethodCasing.LowerCamelCase:
                methodName = methodNameToLowerCase(methodName);
                break;
        }
    }

    static string methodNameToLowerCase(string methodName)
    {
        if (char.IsLower(methodName, 0))
            return methodName;

        return char.ToLowerInvariant(methodName[0]) + methodName.Substring(1);
    }
}

public enum MethodCasing
{
    Undefinied = 0,
    LowerCamelCase
}
