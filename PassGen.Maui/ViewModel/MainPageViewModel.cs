using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace PassGen.Maui;

public class MainPageViewModel : INotifyPropertyChanged
{
    private string _targetSite;
    private bool _useSavedSalt;
    private string _savedSalt;
    private string _salt;
    private string _generatedPassword;

    private readonly WeakEventManager _weakEventManager = new WeakEventManager();
    private readonly ISaltStorage _saltStorage;
    private readonly AsyncCommand _saveSaltCommand;
    private readonly AsyncCommand _clearSavedSaltCommand;
    private readonly Command _invertUseSavedSaltCommand;
    private readonly Command _generatePasswordCommand;

    public MainPageViewModel(ISaltStorage saltStorage, IPasswordGenerator passwordGenerator)
    {
        _saltStorage = saltStorage ?? throw new ArgumentNullException(nameof(saltStorage));
        passwordGenerator = passwordGenerator ?? throw new ArgumentNullException(nameof(passwordGenerator));

        _saveSaltCommand = new AsyncCommand(
            execute: async() =>
            {
                await saltStorage.SetSalt(Salt);
                SavedSalt = Salt;
                UseSavedSalt = true;
                Salt = null;
            },
            canExecute: () => HasSalt);

        _clearSavedSaltCommand = new AsyncCommand(
            execute: async() =>
            {
                await saltStorage.ClearSalt();
                SavedSalt = null;
                UseSavedSalt = false;
            },
            canExecute: () => HasSavedSalt);

        _invertUseSavedSaltCommand = new Command(
            execute: () => UseSavedSalt = !UseSavedSalt,
            canExecute: () => HasSavedSalt);

        _generatePasswordCommand = new Command(
            execute: () => GeneratedPassword = passwordGenerator.GeneratePassword(TargetSite, SaltToUse),
            canExecute: () => !string.IsNullOrEmpty(TargetSite) && !string.IsNullOrEmpty(SaltToUse));
    }

    public async Task LoadDataAsync() => SavedSalt = await _saltStorage.GetSalt();

    public ICommand SaveSaltCommand => _saveSaltCommand;
    public ICommand ClearSavedSaltCommand => _clearSavedSaltCommand;
    public ICommand InvertUseSavedSaltCommand => _invertUseSavedSaltCommand;
    public ICommand GeneratePasswordCommand => _generatePasswordCommand;

    public string TargetSite
    {
        get => _targetSite;
        set
        {
            if (_targetSite == value)
                return;

            var isChangedHasValue = string.IsNullOrEmpty(_targetSite) != string.IsNullOrEmpty(value);

            _targetSite = value;
            OnPropertyChanged(nameof(TargetSite));

            GeneratedPassword = null;

            if (isChangedHasValue)
                _generatePasswordCommand.ChangeCanExecute();
        }
    }

    public bool UseSavedSalt
    {
        get => _useSavedSalt;
        set
        {
            if (_useSavedSalt == value)
                return;

            _useSavedSalt = value;
            OnPropertyChanged(nameof(UseSavedSalt));

            Salt = null;
            GeneratedPassword = null;

            _generatePasswordCommand.ChangeCanExecute();
        }
    }

    public string Salt
    {
        get => _salt;
        set
        {
            if (_salt == value)
                return;

            var isChangedHasValue = string.IsNullOrEmpty(_salt) != string.IsNullOrEmpty(value);

            _salt = value;
            OnPropertyChanged(nameof(Salt));

            if (isChangedHasValue)
                OnPropertyChanged(nameof(HasSalt));

            GeneratedPassword = null;

            if (isChangedHasValue)
            {
                _saveSaltCommand.ChangeCanExecute();
                _generatePasswordCommand.ChangeCanExecute();
            }
        }
    }

    public bool HasSalt => !string.IsNullOrEmpty(Salt);

    public string GeneratedPassword
    {
        get => _generatedPassword;
        set
        {
            if (_generatedPassword == value)
                return;

            _generatedPassword = value;
            OnPropertyChanged(nameof(GeneratedPassword));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged
    {
        add => _weakEventManager.AddEventHandler(value, nameof(PropertyChanged));
        remove => _weakEventManager.RemoveEventHandler(value, nameof(PropertyChanged));
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
        _weakEventManager.HandleEvent(this, new PropertyChangedEventArgs(propertyName), nameof(PropertyChanged));

    private string SavedSalt
    {
        get => _savedSalt;
        set
        {
            if (_savedSalt == value)
                return;

            var isChangedHasValue = string.IsNullOrEmpty(_savedSalt) != string.IsNullOrEmpty(value);

            _savedSalt = value;
            // SavedSalt is private property, no need to call OnPropertyChanged

            if (isChangedHasValue)
                OnPropertyChanged(nameof(HasSavedSalt));

            UseSavedSalt = !string.IsNullOrEmpty(value);
            GeneratedPassword = null;

            if (isChangedHasValue)
            {
                _clearSavedSaltCommand.ChangeCanExecute();
                _invertUseSavedSaltCommand.ChangeCanExecute();
                _generatePasswordCommand.ChangeCanExecute();
            }
        }
    }

    public bool HasSavedSalt => !string.IsNullOrEmpty(SavedSalt);

    private string SaltToUse => UseSavedSalt ? SavedSalt : Salt;
}