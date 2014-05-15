using System;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using ResolutionVigenere.Model;
using ResolutionVigenere.ViewModel.ViewModel;

namespace ResolutionVigenere.View.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly VigenereText _vigenereText = new VigenereText();
        public VigenereText VigenereText { get { return _vigenereText; } }

        public ICommand SearchKeysCommand { get; private set; }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}

            SearchKeysCommand = new RelayCommand(SearchKeys, CanSearchKeys);
        }

        public bool CanSearchKeys()
        {
            return VigenereText.KeyLength > 0 && !string.IsNullOrWhiteSpace(VigenereText.Text);
        }
        public void SearchKeys()
        {
            throw new NotImplementedException();
        }
    }
}