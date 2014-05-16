using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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

        private bool _knowingKeyLength;
        public bool KnowingKeyLength
        {
            get { return _knowingKeyLength; }
            set { _knowingKeyLength = value; RaisePropertyChanged("KnowingKeyLength"); }
        }

        private string _selectedKey;
        public string SelectedKey
        {
            get { return _selectedKey; }
            set { _selectedKey = value; RaisePropertyChanged("SelectedKey"); }
        }

        private int _margeErrorDetectionKeyLength = 5;
        public int MargeErrorDetectionKeyLength
        {
            get { return _margeErrorDetectionKeyLength; }
            set
            {
                if (value < 0)
                    _margeErrorDetectionKeyLength = 0;
                else if (value > 100)
                    _margeErrorDetectionKeyLength = 100;
                else
                    _margeErrorDetectionKeyLength = value;

                RaisePropertyChanged("MargeErrorDetectionKey");
            }
        }

        private int _margeErrorDetectionKey;
        public int MargeErrorDetectionKey
        {
            get { return _margeErrorDetectionKey; }
            set
            {
                if (value < 0)
                    _margeErrorDetectionKey = 0;
                else if (value > 10)
                    _margeErrorDetectionKey = 10;
                else
                    _margeErrorDetectionKey = value;

                RaisePropertyChanged("MargeErrorDetectionKey");
            }
        }

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
            if (KnowingKeyLength && VigenereText.KeyLength <= 0)
                return false;

            return !string.IsNullOrWhiteSpace(VigenereText.CryptedText) && VigenereText.CryptedText.Length > VigenereText.KeyLength;
        }
        public void SearchKeys()
        {
            string cryptedText = VigenereText.CryptedText.ToUpper();

            // use a regex to only care "A-Z"
            var myRegex = new Regex("[^A-Z]");
            cryptedText = myRegex.Replace(cryptedText, "");

            if (!KnowingKeyLength)
            {
                // Step 1 : Get the probably length of the key (if we don't know it)

                // Get the "espace repetition" on the crypted text
                var espaceRepetitions = GetEspacesRepetition(cryptedText);

                // fixbug by taking the "most seen values" in "espaceRepetitions" variable
                int avgEspaceRepetitions = espaceRepetitions.Count * MargeErrorDetectionKeyLength / 100;
                espaceRepetitions = (from n in espaceRepetitions
                                     where espaceRepetitions.Count(e => e == n) >= avgEspaceRepetitions
                                     select n).Distinct().ToList();

                // Get the PGCD on all of these values - that's the KeyLength
                VigenereText.KeyLength = PGCD(espaceRepetitions);
            }

            // Step 2 : Get potential keys from the probably letters on each series
            // as many as then length of the key
            VigenereText.PotentialKeys.Clear();

            _occurenceList = new List<OccurenceLetter>(VigenereText.KeyLength);
            for (int i = 0; i < VigenereText.KeyLength; i++)
                _occurenceList.Add(new OccurenceLetter());

            // Step 2a : Get occurence list of each serie
            int j = 0;

            foreach (var serie in GetSeries(cryptedText))
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
            return !string.IsNullOrWhiteSpace(VigenereText.CryptedText) && !string.IsNullOrWhiteSpace(SelectedKey);
        }
        public void Decrypt()
        {
            // for each letter, we search the cleared letter from crypted AND key letter
            var clearedTextBuilder = new StringBuilder();
            int i = 0;

            foreach (char cryptedLetter in VigenereText.CryptedText)
            {
                int valueLetter = (cryptedLetter - SelectedKey[i]) % 26;
                if (valueLetter < 0)
                    valueLetter += 26;
                char clearedLetter = (char)(valueLetter + 'A');

                clearedTextBuilder.Append(clearedLetter);
                i = ++i % VigenereText.KeyLength;
            }

            VigenereText.ClearedText = clearedTextBuilder.ToString();
        }

        private IList<int> GetEspacesRepetition(string cryptedText)
        {
            var espaceRepetitions = new List<int>();

            for (int i = 20; i > 2; i--)
            {
                for (int j = 0; j < cryptedText.Length - i; j++)
                {
                    string possibleRepetition = cryptedText.Substring(j, i);

                    var repetitions = cryptedText.Split(new[] { possibleRepetition }, StringSplitOptions.RemoveEmptyEntries);
                    if (repetitions.Count() > 2)
                        espaceRepetitions.Add(repetitions[1].Length + i);
                }
            }

            return espaceRepetitions;
        }

        private int PGCD(IList<int> values)
        {
            // Case 1 : no values
            if (values == null || !values.Any())
                return 1;

            int numberValues = values.Count();

            // Case 2 : only one value
            if (numberValues == 1)
                return values.First();

            // Case 3 : two values or more (so PGCD of these values)
            int pgcd = PGCD(values[0], values[1]);

            for (int i = 2; i < numberValues; i++)
                pgcd = PGCD(pgcd, values[i]);

            return pgcd;
        }

        private int PGCD(int value1, int value2)
        {
            while (value1 != value2)
            {
                if (value1 > value2)
                    value1 -= value2;
                else
                    value2 -= value1;
            }

            return value1;
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
                Where(pair => pair.Value > 0 && pair.Value >= maxValue - MargeErrorDetectionKey).
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

        private IEnumerable<string> GetSeries(string cryptedText)
        {
            var series = new List<string>(VigenereText.KeyLength);
            var seriesBuilder = new List<StringBuilder>(VigenereText.KeyLength);

            for (int i = 0; i < VigenereText.KeyLength; i++)
                seriesBuilder.Add(new StringBuilder());

            // Add letters of each serie
            for (int i = 0; i < cryptedText.Length; i++)
                seriesBuilder[i % VigenereText.KeyLength].Append(cryptedText[i].ToString());

            // Get the generated strings from series
            series.AddRange(seriesBuilder.Select(t => t.ToString()));
            return series;
        }

        #endregion
    }
}