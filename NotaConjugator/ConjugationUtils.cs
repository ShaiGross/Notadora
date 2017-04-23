using NotaConjugator.Data;
using NotaDAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotaConjugator
{
    public static class ConjugationUtils
    {
        public static string getConjugationMatchPattern(Verb verb, ConjugationRule conjugationRule)
        {
            var stemLength = verb.Stem.Length;
            var patternIndex = conjugationRule.PatternIndex.Value;
            return verb.Infinative.Substring(0, stemLength + patternIndex);
        }

        public static string getConjugationMatchPattern(ConjugationPackage conjugationPackage)
        {
            return getConjugationMatchPattern(conjugationPackage.Verb, conjugationPackage.ConjugationRule);
        }
    }
}
