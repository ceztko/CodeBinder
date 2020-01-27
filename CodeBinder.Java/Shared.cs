using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Shared.Java
{
    public enum ParenthesisDirection
    {
        Left,
        Right
    }

    public enum ParenthesisType
    {
        Round,
        Square,
        Angle,
        Brace,
    }

    public enum JavaInteropType
    {
        Boolean,
        Character,
        Byte,
        Short,
        Integer,
        Long,
        Float,
        Double,
        String,
    }

    [Flags]
    enum JavaTypeFlags
    {
        None = 0,
        NativeMethod = 1,
        IsByRef = 2,
    }
}
