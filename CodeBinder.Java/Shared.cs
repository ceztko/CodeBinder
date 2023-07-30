// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.Java.Shared;

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
    ByRef = 2,
    TypeArgument = 4,
}
