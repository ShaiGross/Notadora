using NotaConjugator.Data;
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
        ConjugationPackage conjugationPackage;

        #endregion

        #region Ctor

        public Conjugator(NotaContextAcces context)
        {
            this.context = context;
        }

        #endregion

        #region Methods

        public string Conjugate(int tenseId, int verbId, int personId)
        {
            bool conjugaitonPackageSuccess = buildConjugationPackage(tenseId, verbId, personId);

            if (!conjugaitonPackageSuccess)
                return null;            

            return Conjugate();
        }

        private string Conjugate()
        {
            var conjugationRuleType = conjugationPackage.ConjugationRule.Type;
            var suffix = conjugationPackage.Instruction.Suffix;
            var conjugationString = conjugationPackage.ConjugationMatch.ConjugationString;

            switch (conjugationRuleType)
            {
                case ConjugationRuleType.Independent:
                    throw new NotImplementedException("seperate to conjugationClassificationType and conjugationType");
                case ConjugationRuleType.OffsetDepndent:
                    throw new NotImplementedException("add offset to db");
                case ConjugationRuleType.NewStemDependent:                                        
                case ConjugationRuleType.NewInfDependent:
                    return conjugationString + suffix;
                case ConjugationRuleType.SpecialConjugation:
                    return conjugationString;
                default:
                    throw new Exception("Unexpected enum");
            }
        }

        private bool buildConjugationPackage(int tenseId, int verbId, int personId)
        {
            var verb = context.GetItem<Verb>(verbId);

            if (verb == null)
                return false;

            var conjugationMatch = context.getConjugationMatch(tenseId, verbId, personId);

            if (conjugationMatch == null)
                return false;

            var conjugationRule = context.getConjugationMatchConjugationRule(conjugationMatch);

            if (conjugationRule == null)
                return false;

            var instruction = context.GetConjugationInstruction(verb, conjugationRule.Id, personId);

            if (instruction == null)
                return false;

            conjugationPackage = new ConjugationPackage
            {
                Verb = verb,
                ConjugationMatch = conjugationMatch,
                ConjugationRule = conjugationRule,
                Instruction = instruction
            };

            return true;
        }

        private string Conjugate(ConjugationMatch conjugationMatch)
        {
            return null;
            //var conjugationRule = context.GetConj
        }

        #endregion
    }
}
