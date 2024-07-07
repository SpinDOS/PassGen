using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace PassGen.Maui;

public class MainPageViewModel : INotifyPropertyChanged
{
    private string _targetSite;
    private string _savedSalt;
    private bool _useSavedSalt;
    private string _salt;
    private string _generatedPassword;

    private readonly ISaltStorage saltStorage_;
    private readonly Command _saveSaltCommand;
    private readonly Command _clearSaltCommand;
    private readonly Command _invertUseSavedSaltCommand;
    private readonly Command _generatePasswordCommand;

    public MainPageViewModel(ISaltStorage saltStorage, IPasswordGenerator passwordGenerator)
    {
        saltStorage_ = saltStorage;
        _saveSaltCommand = new Command(
            execute: async() =>
            {
                var salt = Salt;
                await saltStorage.SetSalt(salt);
                SavedSalt = salt;
            }, 
            canExecute: () => !string.IsNullOrEmpty(Salt));
        
        _clearSaltCommand = new Command(
            execute: async() =>
            {
                await saltStorage.ClearSalt();
                Salt = SavedSalt = null;
            },
            canExecute: () => !string.IsNullOrEmpty(SavedSalt));

        _invertUseSavedSaltCommand = new Command(
            execute: () => UseSavedSalt = !UseSavedSalt,
            canExecute: () => !string.IsNullOrEmpty(SavedSalt)
        );
        
        _generatePasswordCommand = new Command(
            execute: () => GeneratedPassword = passwordGenerator.GeneratePassword(TargetSite, Salt),
            canExecute: () => !string.IsNullOrEmpty(TargetSite) && !string.IsNullOrEmpty(Salt));
    }

    public async Task LoadDataAsync() {
        Salt = SavedSalt = await saltStorage_.GetSalt();
        UseSavedSalt = !string.IsNullOrEmpty(Salt);
    }

    public ICommand SaveSaltCommand => _saveSaltCommand;
    public ICommand ClearSaltCommand => _clearSaltCommand;
    public ICommand InvertUseSavedSaltCommand => _invertUseSavedSaltCommand;
    public ICommand GeneratePasswordCommand => _generatePasswordCommand;

    public string TargetSite
    {
        get => _targetSite;
        set
        {
            if (_targetSite == value)
                return;

            var oldValue = _targetSite;
            _targetSite = value;
            OnPropertyChanged(nameof(TargetSite));
            
            GeneratedPassword = null;
            if (string.IsNullOrEmpty(oldValue) != string.IsNullOrEmpty(value))
                _generatePasswordCommand.ChangeCanExecute();
        }
    }

    public string SavedSalt
    {
        get => _savedSalt;
        set
        {
            if (_savedSalt == value)
                return;
            
            var oldValue = _savedSalt;
            _savedSalt = value;
            OnPropertyChanged(nameof(SavedSalt));
            
            if (string.IsNullOrEmpty(value))
                UseSavedSalt = false;
                
            if (string.IsNullOrEmpty(oldValue) != string.IsNullOrEmpty(value))
            {
                _clearSaltCommand.ChangeCanExecute();
                _invertUseSavedSaltCommand.ChangeCanExecute();
            }
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
        }
    }

    public string Salt
    {
        get => _salt;
        set
        {
            if (_salt == value)
                return;

            var oldValue = _salt;
            _salt = value;
            OnPropertyChanged(nameof(Salt));
            
            GeneratedPassword = null;
            if (string.IsNullOrEmpty(oldValue) != string.IsNullOrEmpty(value))
            {
                _saveSaltCommand.ChangeCanExecute();
                _generatePasswordCommand.ChangeCanExecute();
            }
        }
    }

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

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}