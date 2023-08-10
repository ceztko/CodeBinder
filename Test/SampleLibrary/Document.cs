namespace SampleLibrary;

[Module("Document")]
public class Document : HandledObject<Document>
{
    PageCollection _pages;
    Metadata _metadata;

    static Document()
    {
        Common.InitLibrary();
    }

    [OverloadBinding("New", OverloadFeature.ParameterArity)]
    public Document()
        : this(SLCreateDocument())
    {
    }

    private Document(IntPtr ptr)
        : base(ptr, true)
    {
        _pages = new PageCollection(this);
        _metadata = new Metadata(this);
    }

    #region Methods

    protected override void FreeHandle(IntPtr handle)
    {
        SLFreeDocument(handle);
    }

    [Requires(Features.PassByRef)]
    [OverloadBinding("WithFile", OverloadFeature.TypeMatch)]
    public static bool IsPdfDocument(string filename, out DocVersion version)
    {
        return SLIsPdfDocument(filename, out version);
    }

    [Requires(Features.PassByRef)]
    [OverloadBinding("WithArray", OverloadFeature.TypeMatch)]
    public static bool IsPdfDocument(byte[] bytes, out DocVersion version)
    {
        return SLIsPdfDocumentBuffer(bytes, bytes.Length, out version);
    }

    [OverloadBinding("WithFile", OverloadFeature.TypeMatch)]
    public static Document Create(string filename, string? password = null)
    {
        var ptr = SLPdfLoadFile(filename, password);
        var doc = new Document(ptr);
        return doc;
    }

    [OverloadBinding("WithArray", OverloadFeature.TypeMatch)]
    public static Document Create(byte[] bytes, string? password = null)
    {
        var ptr = SLPdfLoadBuffer(bytes, 0, bytes.Length, password);
        return new Document(ptr);
    }

    #endregion // Methods

    #region Properties

    public PageCollection Pages
    {
        get { return _pages; }
    }

    public Metadata Metadata
    {
        get { return _metadata; }
    }

    #endregion // Properties

    #region DllImport

    [DllImport("SampleLibrary", CallingConvention = CallingConvention.Cdecl), Order]
    static extern void SLFreeDocument([SLDocument] IntPtr doc);

    [DllImport("SampleLibrary", CallingConvention = CallingConvention.Cdecl), Order]
    [return: SLDocument]
    static extern IntPtr SLCreateDocument();

    [DllImport("SampleLibrary", CallingConvention = CallingConvention.Cdecl), Order]
    [return: SLDocument]
    static extern IntPtr SLPdfLoadBuffer([In] byte[] buffer, int offset, int size, cbstring password);

    [DllImport("SampleLibrary", CallingConvention = CallingConvention.Cdecl), Order]
    [return: SLDocument]
    static extern IntPtr SLPdfLoadFile(cbstring filename, cbstring password);

    [DllImport("SampleLibrary", CallingConvention = CallingConvention.Cdecl), Order]
    [return: MarshalAs(UnmanagedType.I1)]
    static extern cbbool SLIsPdfDocument(cbstring filename, out DocVersion version);

    [DllImport("SampleLibrary", CallingConvention = CallingConvention.Cdecl), Order]
    [return: MarshalAs(UnmanagedType.I1)]
    static extern cbbool SLIsPdfDocumentBuffer([In] byte[] buffer, int size, out DocVersion version);

    #endregion // DllImport
}

#region Support

[Module("Document")]
[DebuggerDisplay("Count = {Count}")]
public partial class PageCollection : IEnumerable<Page>
{
    Document _doc;

    internal PageCollection(Document doc)
    {
        _doc = doc;
    }

    public Page NewPage(Rect? size = null)
    {
        var page = SLDocNewPage(_doc.Handle, size == null ? null : size.Value.ToArray());
        return new Page(_doc, page);;
    }

    public Page NewPageAt(int atIndex, Rect? size = null)
    {
        var page = SLDocNewPageAt(_doc.Handle, atIndex, size == null ? null : size.Value.ToArray());
        return new Page(_doc, page);
    }

    public List<Page> ToList()
    {
        int pageCount = SLDocGetPageCount(_doc.Handle);
        var pages = new IntPtr[pageCount];
        SLDocGetPages(_doc.Handle, pages);
        var ret = new List<Page>(pageCount);
        for (int i = 0; i < pages.Length; i++)
        {
            var page = pages[i];
            ret.Add(new Page(_doc, page));
        }

        return ret;
    }

    [Requires(Features.Iterators, Features.Generators)]
    public IEnumerator<Page> GetEnumerator()
    {
        int count = Count;
        for (int i = 0; i < count; i++)
            yield return this[i];
    }

    [Requires(Features.DotNet)]
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <param name="index">0-based index</param>
    public Page this[int index]
    {
        get
        {
            var page = SLDocGetPage(_doc.Handle, index);
            return new Page(_doc, page);
        }
    }

    public int Count
    {
        get { return SLDocGetPageCount(_doc.Handle); }
    }

    [DllImport("SampleLibrary", CallingConvention = CallingConvention.Cdecl), Order]
    static extern int SLDocGetPageCount([SLDocument] HandleRef doc);

    [DllImport("SampleLibrary", CallingConvention = CallingConvention.Cdecl), Order]
    static extern void SLDocGetPages([SLDocument] HandleRef doc, [SLPdfPage][Out] IntPtr[] pages);

    [DllImport("SampleLibrary", CallingConvention = CallingConvention.Cdecl), Order]
    [return: SLPdfPage]
    static extern IntPtr SLDocGetPage([SLDocument] HandleRef doc, int index);

    [DllImport("SampleLibrary", CallingConvention = CallingConvention.Cdecl), Order]
    static extern void SLDocAppendPage([SLDocument] HandleRef doc, [SLDocument] HandleRef srcdoc, int pageIndex);

    [DllImport("SampleLibrary", CallingConvention = CallingConvention.Cdecl), Order]
    [return: SLPdfPage]
    static extern IntPtr SLDocNewPageAt([SLDocument] HandleRef doc, int atIndex,
        [MarshalAs(UnmanagedType.LPArray, SizeConst = 4)] double[]? rect);

    [DllImport("SampleLibrary", CallingConvention = CallingConvention.Cdecl), Order]
    [return: SLPdfPage]
    static extern IntPtr SLDocNewPage([SLDocument] HandleRef doc,
        [MarshalAs(UnmanagedType.LPArray, SizeConst = 4)] double[]? rect);
}

[Module("Document")]
public class Metadata
{
    Document _doc;

    internal Metadata(Document doc)
    {
        _doc = doc;
    }

    public string? Title
    {
        get { return SLDocGetTitle(_doc.Handle); }
    }

    public DocVersion DocVersion
    {
        get { return SLDocGetVersion(_doc.Handle); }
    }

    [DllImport("SampleLibrary", CallingConvention = CallingConvention.Cdecl), Order]
    static extern DocVersion SLDocGetVersion([SLDocument] HandleRef doc);

    [DllImport("SampleLibrary", CallingConvention = CallingConvention.Cdecl), Order]
    static extern cbstring SLDocGetTitle([SLDocument] HandleRef doc);
}

#endregion // Support

[NativeBinding("SLDocVersion"), NativeStem("DOC_VERSION"), NativeSubstitution(@"v(\d_\d)", "$1")]
public enum DocVersion
{
    Unknown,
    v1_0 = 10,
    v2_0 = 20,
}

[Conditional(ConditionString)]
class SLDocument : NativeTypeBinder { }
