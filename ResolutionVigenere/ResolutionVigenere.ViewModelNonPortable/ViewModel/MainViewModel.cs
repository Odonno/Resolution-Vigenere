﻿using System;
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
        private List<OccurenceLetter> _occurenceList;

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

            _occurenceList = new List<OccurenceLetter>(VigenereText.KeyLength);
            for (int i = 0; i < VigenereText.KeyLength; i++)
                _occurenceList.Add(new OccurenceLetter());

            // Get occurence list of each serie
            int j = 0;

            foreach (var serie in GetSeries())
            {
                foreach (char letter in serie)
                    _occurenceList[j].LettersOccurence[letter]++;

                j++;
            }

            // Get the potential keys from these lists
            var keys = GetKeys("");

            foreach (var key in keys)
                VigenereText.PotentialKeys.Add(key);

            int y = 0;
        }

        private IEnumerable<string> GetKeys(string startKey)
        {
            if (startKey.Length == VigenereText.KeyLength)
                return new List<string> { startKey };

            var returnedKeys = new List<string>();
            var maxValue = _occurenceList[startKey.Length].LettersOccurence.Max(lo => lo.Value);
            var letters = _occurenceList[startKey.Length].LettersOccurence.Where(pair => pair.Value == maxValue).Select(pair => pair.Key);

            // TODO : use "marge error" property

            // for each key letter, susbstract 4 (e => a)
            foreach (var l in letters)
            {
                int valueLetter = (l - 'A' - 4) % 26;
                if (valueLetter < 0)
                    valueLetter += 26;
                char letter = (char)(valueLetter + 'A');

                returnedKeys.AddRange(GetKeys(startKey + letter));
            }

            return returnedKeys;
        }

        private IEnumerable<string> GetSeries()
        {
            // TODO : use a regex to only care "A-Z"
            var text = VigenereText.Text.ToUpper().Replace(" ", "");
            var series = new List<string>(VigenereText.KeyLength);
            var seriesBuilder = new List<StringBuilder>(VigenereText.KeyLength);

            for (int i = 0; i < VigenereText.KeyLength; i++)
                seriesBuilder.Add(new StringBuilder());

            for (int i = 0; i < text.Length; i++)
                seriesBuilder[i % VigenereText.KeyLength].Append(text[i].ToString());

            series.AddRange(seriesBuilder.Select(t => t.ToString()));
            return series;
        }
    }
}