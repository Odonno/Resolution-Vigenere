using System.Collections.Generic;

namespace ResolutionVigenere.Model
{
    public class OccurenceLetter
    {
        private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public Dictionary<char, int> LettersOccurence { get; private set; }

        public OccurenceLetter()
        {
            LettersOccurence = new Dictionary<char, int>();

            foreach (char letter in Alphabet)
                LettersOccurence.Add(letter, 0);
        }
    }
}
