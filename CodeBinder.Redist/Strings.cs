using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace CodeBinder
{
    /// <summary>
    /// Attribute to pass interop string parameter
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct PString
    {
        public static implicit operator string(PString pstr)
        {
            return pstr.String;
        }

        public static implicit operator PString(string str)
        {
            return new PString(str);
        }

        public PString(string str)
        {
            String = str;
            Lenght = new IntPtr(str.Length);
        }

        public string String;
        public IntPtr Lenght;
    }

    /// <summary>
    /// Attribute to return interop string
    /// </summary>
    /// <remarks>We need a separate struct for return values
    /// because PString is not blittable. We do manual manual marshalling</remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct RString
    {
        public string TakeString()
        {
            if (String == IntPtr.Zero)
                return null;

            var ret = Marshal.PtrToStringUni(String, Lenght.ToInt32());
            Marshal.FreeCoTaskMem(String);
            String = IntPtr.Zero;
            return ret;
        }

        public IntPtr String;

        // Length is IntPtr so size is variable depdending on address size
        public IntPtr Lenght;
    }
}
