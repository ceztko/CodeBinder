// Copyright(c) 2020 Francesco Pretto
// This file is subject to the MIT license
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Java.Shared
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
