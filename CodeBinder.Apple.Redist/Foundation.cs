// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.Apple;

public struct NSFastEnumerationState
{

}

// Fake (id) struct that allows setting like a buffer, see
// https://developer.apple.com/documentation/foundation/nsfastenumeration/1412867-countbyenumeratingwithstate?language=objc
public struct id
{
    public object this[int index]
    {
        set { }
    }
}

// TODO: convert also other numerical types
public struct NSInteger
{
    public static implicit operator long(NSInteger d) => 0;
    public static implicit operator NSInteger(long b) => new NSInteger();
    public static implicit operator int(NSInteger d) => 0;
    public static implicit operator NSInteger(int b) => new NSInteger();
    public static explicit operator NSInteger(uint i) => new NSInteger();
    public static explicit operator uint(NSInteger s) => 0;
    public static explicit operator NSInteger(ulong i) => new NSInteger();
    public static explicit operator ulong(NSInteger s) => 0;
}

public struct NSUInteger
{
    public static implicit operator ulong(NSUInteger d) => 0;
    public static implicit operator NSUInteger(ulong b) => new NSUInteger();
    public static implicit operator uint(NSUInteger d) => 0;
    public static implicit operator NSUInteger(uint b) => new NSUInteger();
    public static explicit operator NSUInteger(int i) => new NSUInteger();
    public static explicit operator int(NSUInteger s) => 0;
    public static explicit operator NSUInteger(long i) => new NSUInteger();
    public static explicit operator long(NSUInteger s) => 0;
}

public struct size_t
{
    public static implicit operator ulong(size_t d) => 0;
    public static implicit operator size_t(ulong b) => new size_t();
    public static implicit operator uint(size_t d) => 0;
    public static implicit operator size_t(uint b) => new size_t();
    public static explicit operator size_t(int i) => new size_t();
    public static explicit operator int(size_t s) => 0;
    public static explicit operator size_t(long i) => new size_t();
    public static explicit operator long(size_t s) => 0;
}
