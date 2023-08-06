namespace SampleLibrary;

[DebuggerDisplay("X = {X}, Y = {Y}, Width = {Width}, Height = {Height}")]
public struct Rect
{
    public double X;
    public double Y;
    public double Width;
    public double Height;

    [OverloadBinding("CreateWithWidthHeight", OverloadFeature.ParameterArity)]
    public Rect(double x, double y, double width, double height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    public static Rect Create(double[] array)
    {
        return new Rect(array[0], array[1], array[2], array[3]);
    }

    public double[] ToArray()
    {
        return new double[] { Left, Bottom, Width, Height };
    }

    public double Left
    {
        get { return X; }
        set { X = value; }
    }

    public double Bottom
    {
        get { return Y; }
        set { Y = value; }
    }

    public double Right
    {
        get { return Left + Width; }
    }

    public double Top
    {
        get { return Bottom + Height; }
    }
}
