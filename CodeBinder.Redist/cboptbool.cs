namespace CodeBinder;

// TODO: Other opt types

/// <summary>
/// A structure that can be used to marshal a bool? in DllImport methods
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct cboptbool
{
    byte HasValue;

    byte Value;

    public cboptbool(bool? value)
    {
        if (value is bool actualValue)
        {
            HasValue = 1;
            Value = actualValue ? (byte)1 : (byte)0;
        }
        else
        {
            HasValue = 0;
            Value = 0;
        }
    }

    public static implicit operator bool?(cboptbool val)
    {
        if (val.HasValue == 1)
            return val.Value == 1;
        else
            return default;
    }

    public static implicit operator cboptbool(bool? val)
    {
        return new cboptbool(val);
    }
}
