using System.Collections.Generic;
using System.ComponentModel;

namespace ResolutionVigenere.Model
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

        private IList<string> _potentialKeys;
        public IList<string> PotentialKeys
        {
            get { return _potentialKeys; }
            set { _potentialKeys = value; RaisePropertyChanged("PotentialKeys"); }
        }

        private int _margeError;
        public int MargeError
        {
            get { return _margeError; }
            set { _margeError = value; RaisePropertyChanged("MargeError"); }
        }


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
