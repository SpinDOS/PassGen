using System.ComponentModel;
using System.Windows.Input;
using CommunityToolkit.Maui.Alerts;

namespace PassGen.Maui;

public partial class MainPage : ContentPage
{	
	private readonly MainPageViewModel viewModel_;
	private readonly AsyncCommand copyToClipboardCommand_;
	
	public MainPage(MainPageViewModel viewModel)
	{
		viewModel_ = viewModel;
		copyToClipboardCommand_ = CreateCopyToClipboardCommand(viewModel);
		BindingContext = viewModel;
		InitializeComponent();
	}

	public ICommand CopyToClipboardCommand => copyToClipboardCommand_;

	private async void OnAppearing(object sender, EventArgs e) {
		await viewModel_.LoadDataAsync();
		if (viewModel_.UseSavedSalt)
			_saltGroup.HeightRequest = 0; // collapse group without animation
		viewModel_.PropertyChanged += OnModelPropertyChanged;
	}

	private void OnModelPropertyChanged(object sender, PropertyChangedEventArgs eventArgs)
	{
		var viewModel = (MainPageViewModel)sender;
		switch (eventArgs.PropertyName)
		{
		case nameof(viewModel.UseSavedSalt):
			AnimateVerticalExpand(_saltGroup, "SaltGroupExpandAnimation", !viewModel.UseSavedSalt);
			break;
		case nameof(viewModel.GeneratedPassword):
			copyToClipboardCommand_.ChangeCanExecute();
			AnimateVerticalExpand(
				_generatedPasswordGroup, 
				"GeneratedPasswordExpandAnimation", 
				!string.IsNullOrEmpty(viewModel.GeneratedPassword),
				CreateScrollToLastElementCallback(_mainScrollView, _lastElement)
			);
			break;
		default:
			break;
		}
	}

	private static AsyncCommand CreateCopyToClipboardCommand(MainPageViewModel viewModel)
	{
		return new AsyncCommand(
			execute: async () =>
			{
				await Clipboard.SetTextAsync(viewModel.GeneratedPassword);
				if (OperatingSystem.IsAndroid())
					await Toast.Make("Successfully copied generated password to clipboard").Show();
			},
			canExecute: () => !string.IsNullOrEmpty(viewModel.GeneratedPassword));
	}

	private static void AnimateVerticalExpand(Layout elementWrapper, string animationName, bool targetStateIsExpanded, Action<double, bool> finished = null) {
		var actualHeight = elementWrapper.Height;
		var desiredHeight = targetStateIsExpanded
			? elementWrapper.Children.Single().Measure(double.PositiveInfinity, double.PositiveInfinity).Height
			: 0.0;
		elementWrapper.CancelAnimations();
		elementWrapper.Animate(
			animationName,
			CreateExpandAnimationCallback(elementWrapper),
			actualHeight,
			desiredHeight,
			finished: finished
		);
	}

	private static Action<double> CreateExpandAnimationCallback(VisualElement element)
	{
		var weakReferenceElement = new WeakReference<VisualElement>(element);
		return it =>
		{
			if (weakReferenceElement.TryGetTarget(out var strongReferenceElement))
				strongReferenceElement.HeightRequest = it;
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
