using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ResolutionVigenere.Model
{
    public class VigenereText : INotifyPropertyChanged
    {
        private string _text;
        public string Text { get { return _text; } set { _text = value; RaisePropertyChanged(); } }

        private int _keyLength;
        public int KeyLength { get { return _keyLength; } set { _keyLength = value; RaisePropertyChanged(); } }

        private ObservableCollection<string> _potentialKeys = new ObservableCollection<string>();
        public ObservableCollection<string> PotentialKeys
        {
            get { return _potentialKeys; }
            set { _potentialKeys = value; }
        }


        #region PropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
