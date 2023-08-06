namespace SampleLibrary;

public abstract class AnnotationCollection
{
    [Requires(Policies.ReifiedGenerics)]
    public List<TAnnotation> GetAll<TAnnotation>()
        where TAnnotation : Annotation
    {
        var annotations = new List<TAnnotation>();
        getAllPrivate(annotations);
        return annotations;
    }

    [Requires(Policies.ReifiedGenerics)]
    [OverloadBinding("WithList", OverloadFeature.ParameterArity)]
    public void GetAll<TAnnotation>(List<TAnnotation> annotations, bool clear = true)
        where TAnnotation : Annotation
    {
        if (clear)
            annotations.Clear();

        getAllPrivate(annotations);
    }

    public List<Annotation> GetAll(AnnotationType type = AnnotationType.Any)
    {
        var ret = new List<Annotation>();
        getAllPrivate(ret, type);
        return ret;
    }

    [Requires(Policies.ReifiedGenerics)]
    void getAllPrivate<TAnnotation>(List<TAnnotation> annotations)
        where TAnnotation : Annotation
    {
        var targetType = Annotation.GetAnnotationType<TAnnotation>();
        getAllPrivate(annotations, targetType);
    }

    void getAllPrivate<TAnnotation>(List<TAnnotation> annots, AnnotationType targetType)
         where TAnnotation : Annotation
    {
        getAllInternal(annots, new PdfAnnotationPredicate(targetType));
    }

    protected abstract void getAllInternal<TAnnotation>(List<TAnnotation> annots, PdfAnnotationPredicate predicate)
        where TAnnotation : Annotation;
}

#region Support

public class PdfAnnotationPredicate
{
    AnnotationType _targetType;

    public PdfAnnotationPredicate(AnnotationType targetType)
    {
        _targetType = targetType;
    }

    public bool P(Annotation annot)
    {
        if (_targetType != AnnotationType.Any && annot.Type != _targetType)
            return false;

        return true;
    }
}

#endregion // Support
