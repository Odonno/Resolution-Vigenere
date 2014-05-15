using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ResolutionVigenere.ModelNonPortable
{
    public class VigenereText : INotifyPropertyChanged
    {
        private string _text;
        public string Text
        {
            get { return _text; }
            set { _text = value; RaisePropertyChanged("Text"); }
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
