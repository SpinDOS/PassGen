namespace PassGen.Maui;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class ShowHidePasswordEntry : Grid
{
    public static readonly BindableProperty HiddenProperty = BindableProperty.Create(nameof(Hidden), typeof(bool), typeof(ShowHidePasswordEntry), true);
    public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(ShowHidePasswordEntry));
    public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(ShowHidePasswordEntry));

    public ShowHidePasswordEntry() { InitializeComponent(); }

    public bool Hidden
    {
        get => (bool) GetValue(HiddenProperty);
        set => SetValue(HiddenProperty, value);
    }

    public string Text
    {
        get => (string) GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
    
    public string Placeholder
    {
        get => (string) GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    private void ShowHideButtonClickedEventHandler(object sender, EventArgs args) => Hidden = !Hidden;
}
