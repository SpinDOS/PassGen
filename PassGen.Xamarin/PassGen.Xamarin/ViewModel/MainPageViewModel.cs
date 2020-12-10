using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using PassGen.Xamarin.Service;

namespace PassGen.Xamarin.ViewModel
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private string _targetSite;
        private string _savedSalt;
        private bool _useSavedSalt;
        private string _salt;
        private string _generatedPassword;

        private readonly global::Xamarin.Forms.Command _saveSaltCommand;
        private readonly global::Xamarin.Forms.Command _clearSaltCommand;
        private readonly global::Xamarin.Forms.Command _generatePasswordCommand;
        
        [Obsolete("Should be used only in generated code by XAML", true)]
        public MainPageViewModel() { }

        public MainPageViewModel(ISaltStorage saltStorage, IPasswordGenerator passwordGenerator)
        {
            _saveSaltCommand = new global::Xamarin.Forms.Command(
                execute: async() =>
                {
                    var salt = Salt;
                    await saltStorage.SetSalt(salt);
                    SavedSalt = salt;
                }, 
                canExecute: () => !string.IsNullOrEmpty(Salt));
            
            _clearSaltCommand = new global::Xamarin.Forms.Command(
                execute: async() =>
                {
                    await saltStorage.ClearSalt();
                    Salt = SavedSalt = null;
                },
                canExecute: () => !string.IsNullOrEmpty(SavedSalt));
            
            _generatePasswordCommand = new global::Xamarin.Forms.Command(
                execute: () => GeneratedPassword = passwordGenerator.GeneratePassword(TargetSite, Salt),
                canExecute: () => !string.IsNullOrEmpty(TargetSite) && !string.IsNullOrEmpty(Salt));
        }

        public ICommand SaveSaltCommand => _saveSaltCommand;
        public ICommand ClearSaltCommand => _clearSaltCommand;
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
                    _clearSaltCommand.ChangeCanExecute();
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
}