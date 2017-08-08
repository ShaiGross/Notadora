using NotaDAL.Context;
using NotaDAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotaConjugator
{
    public class Conjugator
    {
        #region Data Members

        private NotaContextAcces context;

        #endregion

        #region Ctor

        public Conjugator(NotaContextAcces context)
        {
            this.context = context;
        }

        #endregion

        public string Conjugate(Verb verb, Tense tense, Person person)
        {
            var appliedTenseConjugationRules = context.GetVerbsConjugationRules(verb, tense);
            return null;
        }
    }
}
