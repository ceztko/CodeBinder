// SPDX-FileCopyrightText: (C) 2018 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT
using CodeBinder.Attributes;

namespace CodeBinder.Java;

[ConversionLanguageName("Java")]
[ConfigurationSwitch("android", "Output is compatible with Android SDK")]
[ConfigurationSwitch("jdk8", "Deprecated, it does nothing: the JDK8 support now lives in the codebinder-redist multi-release jar")]
public class ConversionCSharpToJava : CSharpLanguageConversion
{
    internal const string CodeBinderNamespace = "CodeBinder";

    internal const string SourcePreamble = "/* This file was generated. DO NOT EDIT! */";

    public ConversionCSharpToJava()
    {
    }

    public bool SkipBody { get; set; }

    public JavaPlatform JavaPlatform { get; set; } = JavaPlatform.JDK;

    public override MethodCasing MethodCasing => MethodCasing.LowerCamelCase;

    public override IReadOnlyCollection<string> SupportedPolicies => [ Features.GarbageCollection, Features.InstanceFinalizers ];

    public override bool TryParseExtraArgs(List<KeyValuePair<string, string?>> args)
    {
        // Try parse --android switch
        if (args.Count == 1 && args[0].Key == "android")
        {
            JavaPlatform = JavaPlatform.Android;
            return true;
        }

        // Kept for retrocompatibility with existing codegen scripts. The JDK8
        // and JDK9+ variants of "CodeBinder.BinderUtils" are both shipped by the
        // codebinder-redist multi-release jar, so there's nothing to switch here
        if (args.Count == 1 && args[0].Key == "jdk8")
            return true;

        return false;
    }

    protected override IEnumerable<IConversionWriter> GetContextConversions(CSharpCompilationContext context)
    {
        var namespaces = new HashSet<string>();
        foreach (var type in context.Types)
            namespaces.Add(type.Node.GetContainingNamespaceName(context));

        var javaNamespaces = new List<string>();
        foreach (var ns in namespaces)
            javaNamespaces.Add(NamespaceMapping.GetMappedNamespace(ns, NamespaceNormalization.LowerCase));

        foreach (var ns in javaNamespaces)
        {
            var splittedNs = ns.Split('.');
            var currNs = new List<string>();
            for (int i = 0; i < splittedNs.Length; i++)
            {
                currNs.Add(splittedNs[i]);
                yield return new JavaDummyNamespaceConversion(currNs.ToArray());
            }
        }
    }

    public override IEnumerable<TypeConversion<CSharpClassTypeContext>> GetConversions(CSharpClassTypeContext cls)
    {
        yield return new JavaClassConversion(cls, this);
    }

    public override IEnumerable<TypeConversion<CSharpInterfaceTypeContext>> GetConversions(CSharpInterfaceTypeContext iface)
    {
        yield return new JavaInterfaceConversion(iface, this);
    }

    public override IEnumerable<TypeConversion<CSharpStructTypeContext>> GetConversions(CSharpStructTypeContext str)
    {
        yield return new JavaStructConversion(str, this);
    }

    public override IEnumerable<TypeConversion<CSharpEnumTypeContext>> GetConversions(CSharpEnumTypeContext enm)
    {
        yield return new JavaEnumConversion(enm, this);
    }

    public override IEnumerable<TypeConversion<CSharpDelegateTypeContext>> GetConversions(CSharpDelegateTypeContext dlg)
    {
        yield break;
    }

    protected override CSharpValidationContext? CreateValidationContext()
    {
        return new JavaValidationContext(this);
    }

    public override IReadOnlyList<string> PreprocessorDefinitions
    {
        get
        {
            switch (JavaPlatform)
            {
                case JavaPlatform.JDK:
                    return ["JAVA", "JVM", "JVM_JDK", "JNI_JDK"];
                case JavaPlatform.Android:
                    return ["JAVA", "JVM", "JVM_ANDROID", "JNI_ANDROID"];
                default:
                    throw new NotSupportedException();
            }
        }

    }

    public override bool UseUTF8Bom
    {
        get { return false; }
    }
}

public enum JavaPlatform
{
    /// <summary>
    /// Default JDK platform (JDK11)
    /// </summary>
    JDK = 0,
    /// <summary>
    /// Android SDK
    /// </summary>
    Android,
}
