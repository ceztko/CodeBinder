using CodeTranslator.Shared;
using CodeTranslator.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.Java
{
    public class FieldWriter : SyntaxWriter<FieldDeclarationSyntax>
    {
        public FieldWriter(FieldDeclarationSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append(Syntax.GetJavaModifiersString()).Space();
            Builder.Append(Syntax.Declaration.Type.GetJavaType(this)).Space();
            WriteVariableDeclaration(Syntax.Declaration.Variables);
            Builder.EndOfLine();
        }

        private void WriteVariableDeclaration(SeparatedSyntaxList<VariableDeclaratorSyntax> variables)
        {
            foreach (var variable in variables)
                Builder.Append(variable.Identifier.Text);
        }
    }
}
