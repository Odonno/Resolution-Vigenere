using System.Collections.Generic;

namespace ResolutionVigenere.ModelNonPortable
{
    public class OccurenceLetter
    {
        #region Fields

        private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        #endregion


        #region Properties

        public Dictionary<char, int> LettersOccurence { get; private set; }

        #endregion


        #region Constructor

        public OccurenceLetter()
        {
            LettersOccurence = new Dictionary<char, int>();

            foreach (char letter in Alphabet)
                LettersOccurence.Add(letter, 0);
        }

        #endregion
    }
}
