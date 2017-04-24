using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotaConjugator.Data
{
    public class ConjugationIndex
    {
        public int TenseId { get; set; }
        public int VerbId { get; set; }
        public int PersonId { get; set; }

        public string conjugationString { get; set; }
    }
}
