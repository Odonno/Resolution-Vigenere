using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using ResolutionVigenere.Model;
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
            VigenereText.PotentialKeys = new List<string>();

            var occurenceList = new List<OccurenceLetter>(VigenereText.KeyLength);
            for (int i = 0; i < VigenereText.KeyLength; i++)
                occurenceList.Add(new OccurenceLetter());

            // Get occurence list of each serie
            int j = 0;

            foreach (var serie in GetSeries())
            {
                foreach (char letter in serie)
                    occurenceList[j].LettersOccurence[letter]++;

                j++;
            }

            // Get the potential keys from these lists
            var keyBuilder = new StringBuilder();
            foreach (var occurenceLetter in occurenceList)
            {
                var maxValue = occurenceLetter.LettersOccurence.Max(lo => lo.Value);
                char letter = occurenceLetter.LettersOccurence.FirstOrDefault(pair => pair.Value == maxValue).Key;

                // for each key letter, susbstract 4 (e => a)
                int valueLetter = (letter - 'a' - 4) % 26;
                letter = (char)(valueLetter + 'a');

                keyBuilder.Append(letter);
            }

            VigenereText.PotentialKeys.Add(keyBuilder.ToString());
        }

        private IEnumerable<string> GetSeries()
        {
            var text = VigenereText.Text.ToUpper();
            var series = new List<string>(VigenereText.KeyLength);
            var seriesBuilder = new List<StringBuilder>(VigenereText.KeyLength);

            for (int i = 0; i < VigenereText.KeyLength; i++)
                seriesBuilder.Add(new StringBuilder());

            for (int i = 0; i < text.Length; i++)
                seriesBuilder[i%VigenereText.KeyLength].Append(text[i].ToString());

            series.AddRange(seriesBuilder.Select(t => t.ToString()));
            return series;
        }
    }
}