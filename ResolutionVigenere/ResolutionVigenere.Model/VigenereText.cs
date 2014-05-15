using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResolutionVigenere.Model
{
    public class VigenereText
    {
        public string Text { get; set; }
        public int KeyLength { get; set; }
        public char[] Key { get; set; }
    }
}
