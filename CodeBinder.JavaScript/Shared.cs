// SPDX-FileCopyrightText: (C) 2023 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.JavaScript;

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

public enum JavaScriptInteropType
{
    Boolean,
    Number,
    BigInt,
    String,
}

[Flags]
enum TypeScriptTypeFlags
{
    None = 0,
    IsByRef = 1,
}
