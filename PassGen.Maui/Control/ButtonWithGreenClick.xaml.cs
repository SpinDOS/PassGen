using System.Windows.Input;

namespace PassGen.Maui;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class ButtonWithGreenClick : Grid
{
    public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(ButtonWithGreenClick));
    public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(ButtonWithGreenClick), propertyChanged: OnCommandPropertyChanged);

    public ButtonWithGreenClick() { InitializeComponent(); }

    public string Text
    {
        get => (string) GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public ICommand Command
    {
        get => (ICommand) GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    private static void OnCommandPropertyChanged(BindableObject obj, object oldValue, object newValue)
    {
        var buttonWithGreenClick = (ButtonWithGreenClick) obj;
        buttonWithGreenClick._btnClick.Command = newValue != null
            ? new CommandWrapper((ICommand)newValue, new WeakReference<VisualElement>(buttonWithGreenClick._btnGreen))
            : null;
    }

    private sealed class CommandWrapper : IAsyncCommand
    {
        private readonly ICommand _command;
        private readonly WeakReference<VisualElement> _btnGreen;
        public CommandWrapper(ICommand command, WeakReference<VisualElement> btnGreen)
        {
            _command = command ?? throw new ArgumentNullException(nameof(command));
            _btnGreen = btnGreen ?? throw new ArgumentNullException(nameof(btnGreen));
        }

        public event EventHandler CanExecuteChanged
        {
            add => _command.CanExecuteChanged += value;
            remove => _command.CanExecuteChanged -= value;
        }

        public bool CanExecute(object parameter) => _command.CanExecute(parameter);

        public async void Execute(object parameter) => await ExecuteAsync(parameter);
        public async Task ExecuteAsync(object parameter)
        {
            await _command.ExecuteAsyncOrSync(parameter);
            if (_btnGreen.TryGetTarget(out var btnGreen) && btnGreen != null)
                await AnimateGreenColor(btnGreen);
        }

        private static async Task AnimateGreenColor(VisualElement element)
        {
            element.CancelAnimations();
            element.Opacity = 1;
            await element.FadeTo(0, 2000, Easing.CubicInOut);
        }
    }
}
