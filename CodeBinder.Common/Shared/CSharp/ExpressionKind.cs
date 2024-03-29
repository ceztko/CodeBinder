﻿// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.Shared.CSharp;

/// <summary>
/// This merge espressions kinds, for example all assignment operations are merged in ExpressionKind.Assignment
/// </summary>
public enum ExpressionKind
{
    Unknown,
    AnonymousMethod,
    ParenthesizedLambda,
    SimpleLambda,
    AnonymousObjectCreation,
    ArrayCreation,
    Assignment,
    Await,
    Binary,
    Cast,
    Checked,
    ConditionalAccess,
    Conditional,
    Declaration,
    Default,
    ElementAccess,
    ElementBinding,
    ImplicitArrayCreation,
    ImplicitElementAccess,
    ImplicitStackAllocArrayCreation,
    Initializer,
    Base,
    This,
    InterpolatedString,
    Invocation,
    IsPattern,
    Literal,
    MakeRef,
    MemberAccess,
    MemberBinding,
    ObjectCreation,
    OmittedArraySize,
    Parenthesized,
    PostfixUnary,
    PrefixUnary,
    Query,
    Ref,
    RefType,
    RefValue,
    SizeOf,
    StackAllocArrayCreation,
    Throw,
    Tuple,
    TypeOf,
    Type,
}
