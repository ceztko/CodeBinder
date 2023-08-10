namespace SampleLibrary;

[Module("Annotation")]
[DebuggerDisplay("Title = {Title}, Page = {Page.Index}, Type = {Type}")]
public class Annotation : HandledObject<Annotation>
{
    AnnotationType _type;
    Page _page;

    protected Annotation(AnnotationType type, Page page, IntPtr annot)
        : base(annot, false)
    {
        _type = type;
        _page = page;
    }

    #region Methods

    internal static Annotation CreateAnnotation(IntPtr annot, Page page)
    {
        var type = SLAnnotGetType(annot);
        return CreateAnnotation(annot, type, page);
    }

    [OverloadBinding("WithType", OverloadFeature.ParameterArity)]
    internal static Annotation CreateAnnotation(IntPtr annot, AnnotationType type, Page page)
    {
        switch (type)
        {
            case AnnotationType.Link:
                return new AnnotationLink(page, annot);
            case AnnotationType.Popup:
                return new AnnotationPopup(page, annot);
            default:
                throw new Exception("Unknown annotation type");
        }
    }

    [Requires(Features.ReifiedGenerics)]
    internal static AnnotationType GetAnnotationType<TAnnotation>()
    where TAnnotation : Annotation
    {
        switch (typeof(TAnnotation).Name)
        {
            case nameof(AnnotationLink):
                return AnnotationType.Link;
            case nameof(AnnotationPopup):
                return AnnotationType.Popup;
            default:
                return AnnotationType.Any;
        }
    }

    #endregion // Methods

    #region Properties

    public Rect Rect
    {
        get
        {
            var array = new double[4];
            SLAnnotGetRect(Handle, array);
            return Rect.Create(array);
        }
    }

    public Page Page
    {
        get { return _page; }
    }

    public AnnotationType Type
    {
        get { return _type; }
    }

    #endregion // Properties

    #region DllImport

    [DllImport("SampleLibrary", CallingConvention = CallingConvention.Cdecl), Order]
    static extern AnnotationType SLAnnotGetType([SLAnnotation]IntPtr annot);

    [DllImport("SampleLibrary", CallingConvention = CallingConvention.Cdecl), Order]
    static extern void SLAnnotGetRect([SLAnnotation] HandleRef annot,
    [MarshalAs(UnmanagedType.LPArray, SizeConst = 4)][Out] double[] rect);

    #endregion // DllImport
}

[NativeBinding("SLAnnotationType"), NativeStem("ANNOTATION_TYPE")]
public enum AnnotationType
{
    Unknown = 0,
    Link,
    Popup,
    Any = -1,
}

[Module("Annotation")]
public class AnnotationLink : Annotation
{
    internal AnnotationLink(Page page, IntPtr annot)
        : base(AnnotationType.Link, page, annot) { }
}

[Module("Annotation")]
public class AnnotationPopup : Annotation
{
    internal AnnotationPopup(Page page, IntPtr annot)
        : base(AnnotationType.Popup, page, annot) { }
}

[Conditional(ConditionString)]
class SLAnnotation : NativeTypeBinder { }
