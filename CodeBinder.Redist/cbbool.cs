namespace CodeBinder;

/// <summary>
/// Blittable boolean that always occupies 1 byte
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct cbbool
{
    byte _value;

    public cbbool(bool value)
    {
        _value = value ? (byte)1 : (byte)0;
    }

    public static implicit operator bool(cbbool val)
    {
        return val._value == 1;
    }

    public static implicit operator cbbool(bool val)
    {
        return new cbbool(val);
    }
}
