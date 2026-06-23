// SPDX-FileCopyrightText: (C) 2018 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT
using CodeBinder.Attributes;
using CodeBinder.Java.Shared;

namespace CodeBinder.Java;

[ConversionLanguageName("Java")]
[ConfigurationSwitch("android", "Output is compatible with Android SDK")]
[ConfigurationSwitch("jdk8", "Output is compatible with JDK8")]
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

        if (args.Count == 1 && args[0].Key == "jdk8")
        {
            JavaPlatform = JavaPlatform.JDK8;
            return true;
        }

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
                case JavaPlatform.JDK8:
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

    public override IEnumerable<IConversionWriter> DefaultConversions
    {
        get
        {
            if (JavaPlatform == JavaPlatform.JDK8)
                yield return new JavaVerbatimConversionWriter(nameof(JavaClasses.BinderUtils), JavaClasses.BinderUtilsJDK8);
            else
                yield return new JavaVerbatimConversionWriter(nameof(JavaClasses.BinderUtils), JavaClasses.BinderUtils);

            yield return new JavaVerbatimConversionWriter(nameof(JavaClasses.HandleRef), JavaClasses.HandleRef);
            yield return new JavaVerbatimConversionWriter(nameof(JavaClasses.NativeHandle), JavaClasses.NativeHandle);
            yield return new JavaVerbatimConversionWriter(nameof(JavaClasses.FinalizableObject), JavaClasses.FinalizableObject);
            yield return new JavaVerbatimConversionWriter(nameof(JavaClasses.HandledObjectBase), JavaClasses.HandledObjectBase);
            yield return new JavaVerbatimConversionWriter(nameof(JavaClasses.HandledObject), JavaClasses.HandledObject);
            yield return new JavaVerbatimConversionWriter(nameof(JavaClasses.HandledObjectFinalizer), JavaClasses.HandledObjectFinalizer);
            yield return new JavaVerbatimConversionWriter(nameof(JavaClasses.IObjectFinalizer), JavaClasses.IObjectFinalizer);
            yield return new JavaInteropBoxWriter(JavaInteropType.Boolean);
            yield return new JavaInteropBoxWriter(JavaInteropType.Byte);
            yield return new JavaInteropBoxWriter(JavaInteropType.Short);
            yield return new JavaInteropBoxWriter(JavaInteropType.Integer);
            yield return new JavaInteropBoxWriter(JavaInteropType.Long);
            yield return new JavaInteropBoxWriter(JavaInteropType.Float);
            yield return new JavaInteropBoxWriter(JavaInteropType.Double);
            yield return new JavaInteropBoxWriter(JavaInteropType.String);
            for (int i = 0; i < 10; i++)
            {
                yield return new JavaDelegateWriter(true, i);
                yield return new JavaDelegateWriter(false, i);
            }
        }
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
    /// <summary>
    /// Legacy JDK8 platform
    /// </summary>
    JDK8,
}
