// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace CodeBinder
{
#pragma warning disable IDE1006 // Naming Styles
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct cbstring
#pragma warning restore IDE1006 // Naming Styles
    {
        const uint OwnsDataFlags32 = 1u << 31;
        const ulong OwnsDataFlags64 = 1ul << 63;

        IntPtr m_data;
        UIntPtr m_length;

        public static implicit operator string?(cbstring cbstr)
        {
            if (cbstr.m_data == IntPtr.Zero)
                return null;

            bool ownsdata;
            int length;
            // First bit of length tells if receiver owns string
            if (sizeof(UIntPtr) == 8)
            {
                ulong l = cbstr.m_length.ToUInt64();
                ownsdata = (l & OwnsDataFlags64) != 0;
                length = (int)(l & ~OwnsDataFlags64);
            }
            else
            {
                uint l = cbstr.m_length.ToUInt32();
                ownsdata = (l & OwnsDataFlags32) != 0;
                length = (int)(l & ~OwnsDataFlags32);
            }

            var ret = MarshalNativeUtf8ToManagedString(cbstr.m_data, length);
            if (ownsdata)
                Marshal.FreeHGlobal(cbstr.m_data);

            return ret;
        }

        public static implicit operator cbstring(string? str)
        {
            if (str == null)
                return new cbstring();

            var ret = new cbstring();
            int length;
            MarshalManagedToNativeUtf8String(str, out ret.m_data, out length);
            // Managed code never keep string ownership, so we delegate to native
            if (sizeof(UIntPtr) == 8)
                ret.m_length = new UIntPtr(OwnsDataFlags64 | (uint)length);
            else
                ret.m_length = new UIntPtr(OwnsDataFlags32 | (uint)length);
            return ret;
        }

        static void MarshalManagedToNativeUtf8String(string input, out IntPtr data, out int length)
        {
            fixed (char* pInput = input)
            {
                int utf8Len = Encoding.UTF8.GetByteCount(pInput, input.Length);
                // Allocate also the space of the termination character
                var pResult = (byte*)Marshal.AllocHGlobal(utf8Len + 1);
                Encoding.UTF8.GetBytes(pInput, input.Length, pResult, utf8Len);
                pResult[utf8Len] = 0; // Null character
                data = (IntPtr)pResult;
                length = utf8Len;
            }
        }

        internal static string? MarshalNativeUtf8ToManagedString(IntPtr cbstr, int length)
        {
            sbyte* input = (sbyte*)cbstr;
            // The following string constructor is supported in .NET framework 4.5.2
            return new string(input, 0, length, Encoding.UTF8);
        }
    }
}
