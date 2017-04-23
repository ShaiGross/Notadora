﻿using NotaConjugator.Data;
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

        public Conjugation ConjugatePerson(int tenseId, int verbId, int personId)
        {
            bool conjugaitonPackageSuccess = buildConjugationPackage(tenseId, verbId, personId);

            if (!conjugaitonPackageSuccess)
                return null;

            var conjugationString = Conjugate();

            return new Conjugation
            {
                TenseId = tenseId,
                VerbId = verbId,
                PersonId = personId,
                conjugationString = conjugationString
            };
        }

        public List<Conjugation> ConjugateTense(int tenseId, int verbId)
        {            
            var tenseConjugaitons = new List<Conjugation>();
            var tensePersonIds = context.GetAllTensePersons(tenseId).Select(p => p.Id);

            foreach (var personId in tensePersonIds)
            {
                bool conjugaitonPackageSuccess = buildConjugationPackage(tenseId, verbId, personId);

                if (!conjugaitonPackageSuccess)
                    return null;

                var conjugationString = Conjugate();

                var conjugation = new Conjugation
                {
                    TenseId = tenseId,
                    VerbId = verbId,
                    PersonId = personId,
                    conjugationString = conjugationString
                };

                tenseConjugaitons.Add(conjugation);
            }

            return tenseConjugaitons;
        }

        public List<Conjugation> ConjugateVerb(int verbId)
        {
            var tenseConjugaitons = new List<Conjugation>();
            var tenseIds = context.GetItemList<Tense>().Select(t => t.Id);

            foreach (var tenseId in tenseIds)
            {
                var tensePersonIds = context.GetAllTensePersons(tenseId).Select(p => p.Id);

                foreach (var personId in tensePersonIds)
                {
                    bool conjugaitonPackageSuccess = buildConjugationPackage(tenseId, verbId, personId);

                    if (!conjugaitonPackageSuccess)
                        return null;

                    var conjugationString = Conjugate();

                    var conjugation = new Conjugation
                    {
                        TenseId = tenseId,
                        VerbId = verbId,
                        PersonId = personId,
                        conjugationString = conjugationString
                    };

                    tenseConjugaitons.Add(conjugation);
                }                
            }

            return tenseConjugaitons;
        }

        private string Conjugate()
        {
            var conjugationRuleType = conjugationPackage.ConjugationRule.Type;
            var suffix = conjugationPackage.Instruction.Suffix;
            var conjugationString = conjugationPackage.ConjugationMatch.ConjugationString ?? string.Empty;

            switch (conjugationRuleType)
            {
                case ConjugationRuleType.Independent:
                    var pattern = ConjugationUtils.getConjugationMatchPattern(conjugationPackage);
                    return pattern + suffix;
                case ConjugationRuleType.NewPatternDependent:
                    return conjugationString + suffix;
                case ConjugationRuleType.SpecialConjugation:
                    return conjugationString;
                default:
                    throw new Exception("Unexpeted ConjugationRule type");
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

        #endregion
    }
}
