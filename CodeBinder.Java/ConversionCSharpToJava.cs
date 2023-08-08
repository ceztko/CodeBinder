// SPDX-FileCopyrightText: (C) 2018 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT
using CodeBinder.Attributes;
using CodeBinder.Java.Shared;

namespace CodeBinder.Java;

[ConversionLanguageName("Java")]
[ConfigurationSwitch("android", "Output is compatible with android sdk (Java)")]
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

    public override IReadOnlyCollection<string> SupportedPolicies => new string[] { Features.GarbageCollection, Features.InstanceFinalizers };

    public override bool TryParseExtraArgs(List<string> args)
    {
        // Try parse --commonjs switch
        if (args.Count == 1 && args[0] == "android")
        {
            JavaPlatform = JavaPlatform.Android;
            return true;
        }

        return false;
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
                    return new string[] { "JAVA", "JVM", "JVM_JDK", "JNI_JDK" };
                case JavaPlatform.Android:
                    return new string[] { "JAVA", "JVM", "JVM_ANDROID", "JNI_ANDROID" };
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
    JDK = 0,
    Android
}
