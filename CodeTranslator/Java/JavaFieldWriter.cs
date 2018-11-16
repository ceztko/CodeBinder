using CodeTranslator.Shared;
using CodeTranslator.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.Java
{
    public class FieldWriter : CodeWriter<FieldDeclarationSyntax>
    {
        public FieldWriter(FieldDeclarationSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            string modifiers = Context.GetJavaModifiersString();
            if (!string.IsNullOrEmpty(modifiers))
                Builder.Append(modifiers).Space();

            Builder.Append(Context.Declaration.Type.GetJavaType(this)).Space();
            WriteVariableDeclaration(Context.Declaration.Variables);
            Builder.EndOfStatement();
        }

        private void WriteVariableDeclaration(SeparatedSyntaxList<VariableDeclaratorSyntax> variables)
        {
            foreach (var variable in variables)
                Builder.Append(variable.Identifier.Text);
        }
    }
}
