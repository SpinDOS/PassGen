using System;
using System.ComponentModel;
using System.Windows.Input;
using PassGen.Xamarin.Control;
using PassGen.Xamarin.Service;
using PassGen.Xamarin.ViewModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PassGen.Xamarin
{
    public partial class MainPage : ContentPage
    {
        private readonly ISaltStorage _saltStorage;
        private readonly MainPageViewModel _viewModel;
        
        public MainPage(ISaltStorage saltStorage, IPasswordGenerator passwordGenerator, IToastNotifier toastNotifier)
        {
            _saltStorage = saltStorage;
            _viewModel = new MainPageViewModel(saltStorage, passwordGenerator);
            _viewModel.PropertyChanged += OnModelPropertyChanged;
            ChangeUseSavedSaltCommand = CreateChangeUseSavedSaltCommand(_viewModel);
            CopyToClipboardCommand = CreateCopyToClipboardCommand(_viewModel, toastNotifier);
            
            InitializeComponent();
            this.BindingContext = _viewModel;
            this.Appearing += OnAppearing;
        }

        public ICommand ChangeUseSavedSaltCommand { get; }

        public ICommand CopyToClipboardCommand { get; }
        
        private async void OnAppearing(object sender, EventArgs args)
        {
            var salt = await _saltStorage.GetSalt();
            _viewModel.Salt = _viewModel.SavedSalt = salt;
            _viewModel.UseSavedSalt = !string.IsNullOrEmpty(salt);
        }

        private void OnModelPropertyChanged(object sender, PropertyChangedEventArgs eventArgs)
        {
            switch (eventArgs.PropertyName)
            {
            case nameof(_viewModel.UseSavedSalt):
                _saltGroup.Animate(
                    "SaltGroupSlidingAnimation",
                    CreateLayoutScaleYAnimationCallback(_saltGroup),
                    _saltGroup.LayoutScaleY,
                    _viewModel.UseSavedSalt? 0.0 : 1.0);
                break;
            case nameof(_viewModel.GeneratedPassword):
                _generatedPasswordGroup.Animate("GeneratedPasswordAppearingAnimation",
                    CreateLayoutScaleYAnimationCallback(_generatedPasswordGroup),
                    _generatedPasswordGroup.LayoutScaleY,
                    string.IsNullOrEmpty(_viewModel.GeneratedPassword) ? 0.0 : 1.0,
                    finished: CreateScrollToLastElementCallback(_mainScrollView, _lastElement)
                );
                break;
            default:
                break;
            }
        }

        private static ICommand CreateChangeUseSavedSaltCommand(MainPageViewModel viewModel)
        {
            var command = new Command(
                () => viewModel.UseSavedSalt = !viewModel.UseSavedSalt,
                () => !string.IsNullOrEmpty(viewModel.SavedSalt));
            viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(viewModel.SavedSalt))
                    command.ChangeCanExecute();
            };
            return command;
        }

        private static ICommand CreateCopyToClipboardCommand(MainPageViewModel viewModel, IToastNotifier toastNotifier)
        {
            var command = new Command(
                async () =>
                {
                    await Clipboard.SetTextAsync(viewModel.GeneratedPassword);
                    toastNotifier.ShowToast("Successfully copied generated password to clipboard");
                },
                () => !string.IsNullOrEmpty(viewModel.GeneratedPassword));
            viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(viewModel.GeneratedPassword))
                    command.ChangeCanExecute();
            };
            return command;
        }

        private static Action<double> CreateLayoutScaleYAnimationCallback(StackPanelWithLayoutScale panel)
        {
            var weakReferencePanel = new WeakReference<StackPanelWithLayoutScale>(panel);
            return it =>
            {
                if (weakReferencePanel.TryGetTarget(out var strongReferencePanel))
                    strongReferencePanel.LayoutScaleY = it;
            };
        }

        private static Action<double, bool> CreateScrollToLastElementCallback(ScrollView scrollView, Element lastElement)
        {
            var weakReferenceScrollView = new WeakReference<ScrollView>(scrollView);
            var weakReferenceElem = new WeakReference<Element>(lastElement);
            return async (d, cancelled) =>
            {
                if (weakReferenceScrollView.TryGetTarget(out var strongReferenceScrollView) &&
                    weakReferenceElem.TryGetTarget(out var strongReferenceElem))
                {
                    await strongReferenceScrollView.ScrollToAsync(strongReferenceElem, ScrollToPosition.MakeVisible, true);
                }
            };
        }
    }
}