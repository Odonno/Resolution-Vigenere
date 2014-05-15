using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ResolutionVigenere.ViewModel.ViewModel
{
    public class ViewModelBase : INotifyPropertyChanged
    {
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
