using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Shared.CSharp
{
    /// <summary>
    /// This merge statements kinds, for example all goto statements are merged in StatementKind.Goto
    /// </summary>
    public enum StatementKind
    {
        Unknown,
        Block,
        Break,
        Checked,
        ForEach,
        ForEachVariable,
        Continue,
        Do,
        Empty,
        Expression,
        Fixed,
        For,
        Goto,
        If,
        Labeled,
        LocalDeclaration,
        LocalFunction,
        Lock,
        Return,
        Switch,
        Throw,
        Try,
        Unsafe,
        Using,
        While,
        Yield,
    }
}
