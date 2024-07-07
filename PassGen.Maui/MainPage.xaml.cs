using System.ComponentModel;
using CommunityToolkit.Maui.Alerts;

namespace PassGen.Maui;

public partial class MainPage : ContentPage
{	
	public MainPage(MainPageViewModel viewModel)
	{
		viewModel_ = viewModel;
		BindingContext = viewModel;
		CopyToClipboardCommand = CreateCopyToClipboardCommand(viewModel);
		InitializeComponent();
		viewModel.PropertyChanged += OnModelPropertyChanged;
		if (viewModel.UseSavedSalt) 
			_saltGroup.HeightRequest = 0; // collapse group without animation
	}

	private readonly MainPageViewModel viewModel_;

	public Command CopyToClipboardCommand { get; }

	private async void OnAppearing(object sender, EventArgs e) {
		await viewModel_.LoadDataAsync();
	}

	private void OnModelPropertyChanged(object sender, PropertyChangedEventArgs eventArgs)
	{
		var viewModel = (MainPageViewModel)sender;
		switch (eventArgs.PropertyName)
		{
		case nameof(viewModel.UseSavedSalt):
			AnimateVerticalExpand(
				"SaltGroupExpandAnimation", _saltGroup, !viewModel.UseSavedSalt, null
			);
			break;
		case nameof(viewModel.GeneratedPassword):
			CopyToClipboardCommand.ChangeCanExecute();
			AnimateVerticalExpand(
				"GeneratedPasswordExpandAnimation", 
				_generatedPasswordGroup, 
				!string.IsNullOrEmpty(viewModel.GeneratedPassword),
				CreateScrollToLastElementCallback(_mainScrollView, _lastElement)
			);
			break;
		default:
			break;
		}
	}

	private Command CreateCopyToClipboardCommand(MainPageViewModel viewModel)
	{
		return new Command(
			execute: async () =>
			{
				await Clipboard.SetTextAsync(viewModel.GeneratedPassword);
				if (OperatingSystem.IsAndroid())
					await Toast.Make("Successfully copied generated password to clipboard").Show();
			},
			canExecute: () => !string.IsNullOrEmpty(viewModel.GeneratedPassword));
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
