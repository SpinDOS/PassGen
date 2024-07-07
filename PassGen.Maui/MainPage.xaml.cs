using System.ComponentModel;
using System.Windows.Input;

namespace PassGen.Maui;

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
		var useSavedSalt = !string.IsNullOrEmpty(salt);
		if (useSavedSalt) 
			_saltGroup.HeightRequest = 0; // collapse group without animation
		_viewModel.Salt = _viewModel.SavedSalt = salt;
		_viewModel.UseSavedSalt = useSavedSalt;
	}

	private void OnModelPropertyChanged(object sender, PropertyChangedEventArgs eventArgs)
	{
		switch (eventArgs.PropertyName)
		{
		case nameof(_viewModel.UseSavedSalt):
			AnimateVerticalExpand(
				"SaltGroupExpandAnimation", _saltGroup, !_viewModel.UseSavedSalt, null
			);
			break;
		case nameof(_viewModel.GeneratedPassword):
			AnimateVerticalExpand(
				"GeneratedPasswordExpandAnimation", 
				_generatedPasswordGroup, 
				!string.IsNullOrEmpty(_viewModel.GeneratedPassword),
				CreateScrollToLastElementCallback(_mainScrollView, _lastElement)
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

	private static void AnimateVerticalExpand(string animationName, Grid wrapperGrid, bool targetStateIsExpanded, Action<double, bool> finished) {
		var actualHeight = wrapperGrid.Height;
		var desiredHeight = targetStateIsExpanded
			? wrapperGrid.Children.Single().Measure(double.PositiveInfinity, double.PositiveInfinity).Height
			: 0.0;
		wrapperGrid.CancelAnimations();
		wrapperGrid.Animate(animationName,
			CreateExpandAnimationCallback(wrapperGrid),
			actualHeight,
			desiredHeight,
			finished: finished
		);
	}

	private static Action<double> CreateExpandAnimationCallback(Grid grid)
	{
		var weakReferenceGrid = new WeakReference<Grid>(grid);
		return it =>
		{
			if (weakReferenceGrid.TryGetTarget(out var strongReferenceGrid))
				strongReferenceGrid.HeightRequest = it;
		};
	}

	private static Action<double, bool> CreateScrollToLastElementCallback(ScrollView scrollView, Element lastElement)
	{
		var weakReferenceScrollView = new WeakReference<ScrollView>(scrollView);
		var weakReferenceElem = new WeakReference<Element>(lastElement);
		return async (d, canceled) =>
		{
			if (!canceled && 
				weakReferenceScrollView.TryGetTarget(out var strongReferenceScrollView) &&
				weakReferenceElem.TryGetTarget(out var strongReferenceElem))
			{
				await strongReferenceScrollView.ScrollToAsync(strongReferenceElem, ScrollToPosition.MakeVisible, true);
			}
		};
	}
}
