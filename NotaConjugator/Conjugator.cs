using NotaDAL.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotaConjugator
{
    public class Conjugator
    {
        NotaContextAcces context;

        public string conjugate(int tenseId, int verbId, int personId)
        {
            return null;
        }

        public Conjugator(NotaContextAcces context)
        {
            this.context = context;
        }
    }
}
