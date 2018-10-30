using CodeTranslator.Shared;
using CodeTranslator.Util;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CodeTranslator.Shared.CSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.Java
{
    abstract class BaseMethodWriter
    {
        public ISemanticModelProvider Context { get; private set; }

        protected BaseMethodWriter(ISemanticModelProvider context)
        {
            Context = context;
        }

        public void Write(IndentStringBuilder builder)
        {
            builder.Append(Method.GetJavaModifiersString());
            builder.Append(" ");
            WriteReturnType(builder);
            WriteMethodName(builder);
            builder.AppendLine("(");
            using (builder = builder.Indent())
            {
                WriteParameters(Method.ParameterList);
                builder.AppendLine(")");
            }
            builder.Append("{");
            builder.Append("}");
        }

        protected abstract void WriteMethodName(IndentStringBuilder builder);

        private void WriteParameters(ParameterListSyntax list)
        {
            foreach (var parameter in list.Parameters)
                WriteType(parameter.Type);
        }

        protected void WriteType(TypeSyntax type)
        {

        }

        protected virtual void WriteReturnType(IndentStringBuilder builder) { }

        public BaseMethodDeclarationSyntax Method
        {
            get { return GetMethod(); }
        }

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

        protected override void WriteReturnType(IndentStringBuilder builder)
        {
            WriteType(Method.ReturnType);
        }

        protected override void WriteMethodName(IndentStringBuilder builder)
        {
            builder.Append(Method.GetName());
        }
    }

    class ConstructorWriter : MethodWriter<ConstructorDeclarationSyntax>
    {
        public ConstructorWriter(ConstructorDeclarationSyntax method, ISemanticModelProvider context)
            : base(method, context) { }

        protected override void WriteMethodName(IndentStringBuilder builder)
        {

        }
    }

    class DestructorWriter : MethodWriter<DestructorDeclarationSyntax>
    {
        public DestructorWriter(DestructorDeclarationSyntax method, ISemanticModelProvider context)
            : base(method, context) { }

        protected override void WriteMethodName(IndentStringBuilder builder)
        {

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
