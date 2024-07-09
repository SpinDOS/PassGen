using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace PassGen.Maui;

public partial class MainPageViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(GeneratePasswordCommand))]
    private string targetSite;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(GeneratePasswordCommand))]
    private bool useSavedSalt;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ClearSavedSaltCommand), nameof(InvertUseSavedSaltCommand), nameof(GeneratePasswordCommand))]
    private string savedSalt;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSalt))]
    [NotifyCanExecuteChangedFor(nameof(SaveSaltCommand), nameof(GeneratePasswordCommand))]
    private string salt;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CopyToClipboardCommand))]
    private string _generatedPassword;

    private readonly ISaltStorage _saltStorage;
    private readonly IPasswordGenerator _passwordGenerator;

    public MainPageViewModel(ISaltStorage saltStorage, IPasswordGenerator passwordGenerator)
    {
        _saltStorage = saltStorage ?? throw new ArgumentNullException(nameof(saltStorage));
        _passwordGenerator = passwordGenerator ?? throw new ArgumentNullException(nameof(passwordGenerator));
    }

    public async Task LoadDataAsync() => SavedSalt = await _saltStorage.GetSalt();

    [RelayCommand(CanExecute=nameof(HasSalt))]
    private async Task SaveSalt()
    {
        await _saltStorage.SetSalt(Salt);
        SavedSalt = Salt;
        Salt = null;
    }

    [RelayCommand(CanExecute=nameof(HasSavedSalt))]
    private async Task ClearSavedSalt()
    {
        await _saltStorage.ClearSalt();
        SavedSalt = null;
    }

    [RelayCommand(CanExecute=nameof(HasSavedSalt))]
    private void InvertUseSavedSalt() => UseSavedSalt = !UseSavedSalt;

    [RelayCommand(CanExecute=nameof(CanGeneratePassword))]
    private void GeneratePassword() => GeneratedPassword = _passwordGenerator.GeneratePassword(TargetSite, SaltToUse);
    private bool CanGeneratePassword() => !string.IsNullOrEmpty(TargetSite) && !string.IsNullOrEmpty(SaltToUse);

	[RelayCommand(CanExecute=nameof(CanCopyToClipboard))]
	private async Task CopyToClipboard()
	{
		await Clipboard.SetTextAsync(GeneratedPassword);
		if (OperatingSystem.IsAndroid())
			await Toast.Make("Successfully copied generated password to clipboard").Show();
	}

	private bool CanCopyToClipboard() => !string.IsNullOrEmpty(GeneratedPassword);

    partial void OnTargetSiteChanged(string oldValue, string newValue) => GeneratedPassword = null;

    partial void OnUseSavedSaltChanged(bool oldValue, bool newValue)
    {
        Salt = null;
        GeneratedPassword = null;
    }

    partial void OnSavedSaltChanged(string oldValue, string newValue)
    {
        OnPropertyChanged(nameof(HasSavedSalt)); // maui incorrectly handles update of HasSavedSalt after UseSavedSalt
        UseSavedSalt = !string.IsNullOrEmpty(newValue);
        GeneratedPassword = null;
    }

    partial void OnSaltChanged(string oldValue, string newValue) => GeneratedPassword = null;

    public bool HasSavedSalt => !string.IsNullOrEmpty(SavedSalt);
    public bool HasSalt => !string.IsNullOrEmpty(Salt);

    private string SaltToUse => UseSavedSalt ? SavedSalt : Salt;
}