using System.ComponentModel;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.Input;

namespace PassGen.Maui;

public partial class MainPage : ContentPage
{
	private readonly MainPageViewModel _viewModel;

	public MainPage(MainPageViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
		BindingContext = _viewModel;
		if (_viewModel.UseSavedSalt)
			_saltGroup.HeightRequest = 0; // collapse group without animation
		_viewModel.PropertyChanged += ModelPropertyChangedEventHandler;
	}

	[RelayCommand(CanExecute=nameof(CanCopyToClipboard))]
	private async Task CopyToClipboard()
	{
		await Clipboard.SetTextAsync(_viewModel.GeneratedPassword);
		if (OperatingSystem.IsAndroid())
			await Toast.Make("Successfully copied generated password to clipboard").Show();
	}

	private bool CanCopyToClipboard() => !string.IsNullOrEmpty(_viewModel?.GeneratedPassword);

	private void ModelPropertyChangedEventHandler(object sender, PropertyChangedEventArgs eventArgs)
	{
		var viewModel = (MainPageViewModel)sender;
		switch (eventArgs.PropertyName)
		{
		case nameof(viewModel.UseSavedSalt):
			AnimateVerticalExpand(_saltGroup, "SaltGroupExpandAnimation", !viewModel.UseSavedSalt);
			break;
		case nameof(viewModel.GeneratedPassword):
			CopyToClipboardCommand.NotifyCanExecuteChanged();
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

	private static void AnimateVerticalExpand(Layout elementWrapper, string animationName, bool targetStateIsExpanded, Action<double, bool> finished = null)
	{
		var actualHeight = elementWrapper.Height;
		var desiredHeight = targetStateIsExpanded
			? elementWrapper.Children.Single().Measure(double.PositiveInfinity, double.PositiveInfinity).Height
			: 0.0;
		elementWrapper.AbortAnimation(animationName);
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
