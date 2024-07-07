namespace PassGen.Maui;

public class StackPanelWithLayoutScale : StackLayout
{
    public static readonly BindableProperty LayoutScaleXProperty = BindableProperty.Create(
        nameof(LayoutScaleX),
        typeof(double),
        typeof(StackPanelWithLayoutScale),
        (object) 1.0,
        propertyChanged: OnLayoutScaleChanged); 
    
    public static readonly BindableProperty LayoutScaleYProperty = BindableProperty.Create(
        nameof(LayoutScaleY),
        typeof(double),
        typeof(StackPanelWithLayoutScale),
        (object) 1.0,
        propertyChanged: OnLayoutScaleChanged); 
    
    protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
    {
        var result = base.OnMeasure(widthConstraint, heightConstraint);
        return new SizeRequest()
        {
            Minimum = new Size()
            {
                Width = result.Minimum.Width * LayoutScaleX,
                Height = result.Minimum.Height * LayoutScaleY,
            },
            Request = new Size()
            {
                Width = result.Request.Width * LayoutScaleX,
                Height = result.Request.Height * LayoutScaleY,
            },
        };
    }

    // protected override void LayoutChildren(double x, double y, double width, double height)
    // {
    //     base.LayoutChildren(x, y, width * LayoutScaleX, height * LayoutScaleY);
    // }

    public double LayoutScaleX
    {
        get => (double) base.GetValue(LayoutScaleXProperty);
        set => base.SetValue(LayoutScaleXProperty, value);
    }

    public double LayoutScaleY
    {
        get => (double) base.GetValue(LayoutScaleYProperty);
        set => base.SetValue(LayoutScaleYProperty, value);
    }
    
    private static void OnLayoutScaleChanged(BindableObject bindable, object oldValue, object newValue) => 
        ((StackPanelWithLayoutScale)bindable).InvalidateMeasure();
}
