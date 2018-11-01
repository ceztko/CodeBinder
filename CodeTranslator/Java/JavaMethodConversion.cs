using CodeTranslator.Shared;
using CodeTranslator.Util;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CodeTranslator.Shared.CSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.Java
{
    abstract class BaseMethodWriter : BaseWriter
    {
        protected BaseMethodWriter(ISemanticModelProvider context)
            : base(context) { }

        protected override void Write()
        {
            Builder.Append(Method.GetJavaModifiersString());
            Builder.Append(" ");
            WriteReturnType();
            Builder.Append(MethodName);
            if (Method.ParameterList.Parameters.Count == 0)
            {
                Builder.Append("()");
            }
            else
            {
                using (Builder.BeginParameterList())
                {
                    WriteParameters(Method.ParameterList);
                }
            }
            using (Builder.Append(" ").BeginBlock())
            {

            }
        }

        private void WriteParameters(ParameterListSyntax list)
        {
            foreach (var parameter in list.Parameters)
                WriteType(parameter.Type);
        }

        protected void WriteType(TypeSyntax type)
        {

        }

        protected virtual void WriteReturnType() { }

        public BaseMethodDeclarationSyntax Method
        {
            get { return GetMethod(); }
        }

        public abstract string MethodName { get; }

        protected abstract BaseMethodDeclarationSyntax GetMethod();
    }

    abstract class MethodWriter<TMethod> : BaseMethodWriter
        where TMethod : BaseMethodDeclarationSyntax
    {
        public new TMethod Method { get; private set; }

        protected MethodWriter(TMethod method, ISemanticModelProvider context)
            : base(context)
        {
            Method = method;
        }

        protected override BaseMethodDeclarationSyntax GetMethod()
        {
            return Method;
        }
    }

    class MethodWriter : MethodWriter<MethodDeclarationSyntax>
    {
        public MethodWriter(MethodDeclarationSyntax method, ISemanticModelProvider context)
            : base(method, context) { }

        protected override void WriteReturnType()
        {
            WriteType(Method.ReturnType);
        }

        public override string MethodName
        {
            get { return Method.GetName(); }
        }
    }

    class ConstructorWriter : MethodWriter<ConstructorDeclarationSyntax>
    {
        public ConstructorWriter(ConstructorDeclarationSyntax method, ISemanticModelProvider context)
            : base(method, context) { }

        public override string MethodName
        {
            get { return (Method.Parent as BaseTypeDeclarationSyntax).GetName();}
        }
    }

    class DestructorWriter : MethodWriter<DestructorDeclarationSyntax>
    {
        public DestructorWriter(DestructorDeclarationSyntax method, ISemanticModelProvider context)
            : base(method, context) { }

        public override string MethodName
        {
            get { return "finalize"; }
        }
    }

    /*
    private void WriteTypeJava(StreamWriter writer, Type type)
    {
        if (type.IsEnum)
        {
            writer.Write("int");
            return;
        }

        if (type.IsByRef && !type.IsPrimitive)
        {
            writer.Write("long");
            return;
        }

        string typeName = type.Name;
        string typeSuffix = String.Empty;
        if (type.IsArray)
        {
            typeName = typeName.Substring(0, typeName.Length - 2);
            typeSuffix = "[]";
        }

        switch (typeName)
        {
            case "Void":
            {
                writer.Write("void");
                break;
            }
            case nameof(Object):
            {
                writer.Write("Object");
                break;
            }
            case nameof(IntPtr):
            {
                writer.Write("long");
                break;
            }
            case nameof(Boolean):
            {
                writer.Write("boolean");
                break;
            }
            case nameof(Char):
            {
                writer.Write("char");
                break;
            }
            case nameof(String):
            {
                writer.Write("String");
                break;
            }
            case nameof(Byte):
            {
                writer.Write("byte");
                break;
            }
            case nameof(SByte):
            {
                writer.Write("byte");
                break;
            }
            case nameof(Int16):
            {
                writer.Write("short");
                break;
            }
            case nameof(UInt16):
            {
                writer.Write("short");
                break;
            }
            case nameof(Int32):
            {
                writer.Write("int");
                break;
            }
            case nameof(UInt32):
            {
                writer.Write("int");
                break;
            }
            case nameof(Int64):
            {
                writer.Write("long");
                break;
            }
            case nameof(UInt64):
            {
                writer.Write("long");
                break;
            }
            case nameof(Single):
            {
                writer.Write("float");
                break;
            }
            case nameof(Double):
            {
                writer.Write("double");
                break;
            }
            default:
            {
                throw new Exception("Unsupported type " + type.Name);
            }
        }

        writer.Write(typeSuffix);
    }
    */
}
