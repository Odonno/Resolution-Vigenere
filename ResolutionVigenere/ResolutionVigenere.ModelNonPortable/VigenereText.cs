using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ResolutionVigenere.ModelNonPortable
{
    public class VigenereText : INotifyPropertyChanged
    {
        private string _cryptedText;
        public string CryptedText
        {
            get { return _cryptedText; }
            set { _cryptedText = value; RaisePropertyChanged("CryptedText"); }
        }

        private int _keyLength;
        public int KeyLength
        {
            get { return _keyLength; }
            set { _keyLength = value; RaisePropertyChanged("KeyLength"); }
        }

        private int _margeError;
        public int MargeError
        {
            get { return _margeError; }
            set { _margeError = value; RaisePropertyChanged("MargeError"); }
        }

        private string _selectedKey;
        public string SelectedKey
        {
            get { return _selectedKey; }
            set { _selectedKey = value; RaisePropertyChanged("SelectedKey"); }
        }

        private string _clearedText;
        public string ClearedText
        {
            get { return _clearedText; }
            set { _clearedText = value; RaisePropertyChanged("ClearedText"); }
        }

        private readonly ObservableCollection<string> _potentialKeys = new ObservableCollection<string>();
        public ObservableCollection<string> PotentialKeys { get { return _potentialKeys; }}


        #region PropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
