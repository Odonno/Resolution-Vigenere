using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using ResolutionVigenere.ModelNonPortable;
using ResolutionVigenere.ViewModelNonPortable.ViewModel;

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
        #region Fields

        private List<OccurenceLetter> _occurenceList;

        #endregion


        #region Properties

        private readonly VigenereText _vigenereText = new VigenereText();
        public VigenereText VigenereText { get { return _vigenereText; } }

        public ICommand SearchKeysCommand { get; private set; }
        public ICommand DecryptCommand { get; private set; }

        #endregion


        #region Constructor

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
            DecryptCommand = new RelayCommand(Decrypt, CanDecrypt);
        }

        #endregion


        #region Methods

        public bool CanSearchKeys()
        {
            return VigenereText.KeyLength > 0 && !string.IsNullOrWhiteSpace(VigenereText.CryptedText) && VigenereText.CryptedText.Length > VigenereText.KeyLength;
        }
        public void SearchKeys()
        {
            // Step 1 : TODO : Get the probably length of the key

            // Step 2 : Get potential keys from the probably letters on each series
            // as many as then length of the key
            VigenereText.PotentialKeys.Clear();

            _occurenceList = new List<OccurenceLetter>(VigenereText.KeyLength);
            for (int i = 0; i < VigenereText.KeyLength; i++)
                _occurenceList.Add(new OccurenceLetter());

            // Step 2a : Get occurence list of each serie
            int j = 0;

            foreach (var serie in GetSeries())
            {
                foreach (char letter in serie)
                    _occurenceList[j].LettersOccurence[letter]++;

                j++;
            }

            // Step 2b : Get the potential keys from these lists
            var keys = GetKeys("");

            foreach (var key in keys)
                VigenereText.PotentialKeys.Add(key);
        }


        public bool CanDecrypt()
        {
            return !string.IsNullOrWhiteSpace(VigenereText.CryptedText) && !string.IsNullOrWhiteSpace(VigenereText.SelectedKey);
        }
        public void Decrypt()
        {
            // for each letter, we search the cleared letter from crypted AND key letter
            var clearedTextBuilder = new StringBuilder();
            int i = 0;

            foreach (char cryptedLetter in VigenereText.CryptedText)
            {
                int valueLetter = (cryptedLetter - VigenereText.SelectedKey[i]) % 26;
                if (valueLetter < 0)
                    valueLetter += 26;
                char clearedLetter = (char)(valueLetter + 'A');

                clearedTextBuilder.Append(clearedLetter);
                i = ++i % VigenereText.KeyLength;
            }

            VigenereText.ClearedText = clearedTextBuilder.ToString();
        }


        private IEnumerable<string> GetKeys(string startKey)
        {
            // Stop case : if the key length is reached
            if (startKey.Length == VigenereText.KeyLength)
                return new List<string> { startKey };

            var returnedKeys = new List<string>();

            // Get the max value of occurence
            var maxValue = _occurenceList[startKey.Length].LettersOccurence.Max(lo => lo.Value);

            // use "marge error" property to get more possibilities
            var letters = _occurenceList[startKey.Length].LettersOccurence.
                Where(pair => pair.Value >= maxValue - VigenereText.MargeError).
                Select(pair => pair.Key);

            // for each key letter, susbstract 4 (e => a)
            foreach (var l in letters)
            {
                int valueLetter = (l - 'A' - 4) % 26;
                if (valueLetter < 0)
                    valueLetter += 26;
                char letter = (char)(valueLetter + 'A');

                // Get all possible keys from the recursive function
                returnedKeys.AddRange(GetKeys(startKey + letter));
            }

            return returnedKeys;
        }

        private IEnumerable<string> GetSeries()
        {
            var text = VigenereText.CryptedText.ToUpper();
            var series = new List<string>(VigenereText.KeyLength);
            var seriesBuilder = new List<StringBuilder>(VigenereText.KeyLength);

            // use a regex to only care "A-Z"
            var myRegex = new Regex("[^A-Z]");
            text = myRegex.Replace(text, "");

            for (int i = 0; i < VigenereText.KeyLength; i++)
                seriesBuilder.Add(new StringBuilder());

            // Add letters of each serie
            for (int i = 0; i < text.Length; i++)
                seriesBuilder[i % VigenereText.KeyLength].Append(text[i].ToString());

            // Get the generated strings from series
            series.AddRange(seriesBuilder.Select(t => t.ToString()));
            return series;
        }

        #endregion
    }
}