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

        NotaContextAcces context;

        #endregion

        #region Ctor

        public Conjugator(NotaContextAcces contxt)
        {
            this.context = context;
        }

        #endregion

        #region Methods

        public string Conjugate(int tenseId, int verbId, int personId)
        {
            var conjugationMatch = context.getConjugationMatch(tenseId, verbId, personId);

            return Conjugate(conjugationMatch);
        }

        private string Conjugate(ConjugationMatch conjugationMatch)
        {
            return null;
            //var conjugationRule = context.GetConj
        }

        #endregion
    }
}
