using NotaDAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotaConjugator.Data
{
    public class ConjugationPackage
    {
        #region Properties

        public Verb Verb { get; set; }

        public ConjugationRule ConjugationRule { get; set; }
        public ConjugationMatch ConjugationMatch { get; set; }
        public ConjugationRulesInstruction Instruction { get; set; }

        #endregion
    }
}
