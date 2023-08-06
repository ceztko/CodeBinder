namespace SampleLibrary;

[Module("Page")]
[DebuggerDisplay("Index = {Index}")]
public class Page : HandledObject<Page>
{
    PageAnnotationCollection _annotations;

    public Document Document { get; private set; }

    internal Page(Document doc, IntPtr page)
        : base(page, false)
    {
        _annotations = new PageAnnotationCollection(this);
        Document = doc;
    }

    #region Private Methods

    internal int getAnnotationCount()
    {
        return SLPageGetAnnotationCount(Handle);
    }

    internal void getAnnotations<TAnnotation>(List<TAnnotation> annotations, PdfAnnotationPredicate predicate)
        where TAnnotation : Annotation
    {
        var annotPtrs = new IntPtr[getAnnotationCount()];
        SLPageGetAnnotations(Handle, annotPtrs);
        for (int i = 0; i < annotPtrs.Length; i++)
        {
            var annotPtr = annotPtrs[i];
            var annot = Annotation.CreateAnnotation(annotPtr, this);
            if (predicate.P(annot))
                annotations.Add((TAnnotation)annot);
        }
    }

    #endregion // Private Methods

    #region Properties

    /// <summary>0-based index</summary>
    public int Index
    {
        get { return SLPageGetIndex(Handle); }
    }

    /// <summary>High level API to retrieve size of the page. Will return the rotated size
    /// in presence of rotated page</summary>
    public Rect Rect
    {
        get
        {
            var array = new double[4];
            SLPageGetRect(Handle, array);
            return Rect.Create(array);
        }
    }

    public PageAnnotationCollection Annotations
    {
        get { return _annotations; }
    }

    #endregion // Properties

    #region DllImport

    [DllImport("SampleLibrary", CallingConvention = CallingConvention.Cdecl), Order]
    static extern void SLPageGetAnnotations([SLPdfPage] HandleRef page, [SLAnnotation][Out] IntPtr[] annots);

    [DllImport("SampleLibrary", CallingConvention = CallingConvention.Cdecl), Order]
    static extern int SLPageGetAnnotationCount([SLPdfPage] HandleRef page);

    [DllImport("SampleLibrary", CallingConvention = CallingConvention.Cdecl), Order]
    static extern int SLPageGetIndex([SLPdfPage] HandleRef page);

    [DllImport("SampleLibrary", CallingConvention = CallingConvention.Cdecl), Order]
    static extern void SLPageGetRect([SLPdfPage] HandleRef page,
        [MarshalAs(UnmanagedType.LPArray, SizeConst = 4)][Out] double[] rect);

    #endregion // DllImport
}

#region Support

public class PageAnnotationCollection : AnnotationCollection
{
    private Page _page;

    internal PageAnnotationCollection(Page page)
    {
        _page = page;
    }

    public int Count
    {
        get { return _page.getAnnotationCount(); }
    }

    protected override void getAllInternal<TAnnotation>(List<TAnnotation> annots, PdfAnnotationPredicate predicate)
    {
        _page.getAnnotations(annots, predicate);
    }
}

#endregion // Support

[Conditional(ConditionString)]
class SLPdfPage : NativeTypeBinder { }
