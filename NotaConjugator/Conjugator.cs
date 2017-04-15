using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotaDAL;
using NotaDAL.Models;

namespace NotaConjugator
{
    public class Conjugator
    {
        #region Data Members

        private NotaContext context;
        private List<int> ignorePersonsIds = new List<int>();

        #endregion

        #region Ctors

        public Conjugator(NotaContext dataContext)
        {
            context = dataContext;
        }

        #endregion

        #region Methods

        public string ClassifyVerbConjugators(Verb verb, Dictionary<Tense, List<string>> tensesConjugations)
        {

            sortTensesConjugations(ref tensesConjugations);


            foreach (var tenseConjugations in tensesConjugations)
            {
                ignorePersonsIds.Clear();
                var tense = tenseConjugations.Key;
                var regularConjugationRule = context.GetTenseRegularConjugationRule(tense);
                var IrregularConjugationRules = context.GetTenseIrregularConjugationRules(tense);
                string conjData = null;

                if (IsVerbRegular(verb, tense, tenseConjugations.Value, ref conjData))
                {
                    context.AddVerbConjugationRule(verb,
                                                   regularConjugationRule,
                                                   conjData);
                    continue;
                }
                else
                {
                    ClassifyVerbIrregularConjugationRules(verb,
                                                          tense,
                                                          IrregularConjugationRules,
                                                          tenseConjugations.Value);
                }
            }

            return null;
        }

        private void sortTensesConjugations(ref Dictionary<Tense, List<string>> tensesConjugations)
        {
            var sortedTenses = tensesConjugations.Keys.ToList();
            sortedTenses.Sort();
            var conjugations = tensesConjugations.Values.ToList();

            var tempTensesConjugations = new Dictionary<Tense, List<string>>(tensesConjugations.Count);

            for (int index = 0; index < tensesConjugations.Count; index++)
            {
                tempTensesConjugations.Add(sortedTenses[index], conjugations[index]);
            }

            tensesConjugations = tempTensesConjugations;
        }

        private bool IsVerbRegular(Verb verb, 
                                   Tense tense,
                                   List<string> tenseConjugations,
                                   ref string conjData)
        {
            return IsConjugatorAppliedToVerb(verb,
                                             tenseConjugations,
                                             tense.RegularConjugationRule,
                                             ref conjData);
        }

        private void ClassifyVerbIrregularConjugationRules(Verb verb,
                                                           Tense tense,
                                                           List<ConjugationRule> conjugationRules, // TODO: Remove This and get from tense
                                                           List<string> conjugations)
        {
            ignorePersonsIds = new List<int>();
            conjugationRules.Sort();
            var conjugationRulesCounter = 0;
            string conjData = null;


            foreach (var conjugationRule in conjugationRules)
            {
                if (IsConjugatorAppliedToVerb(verb,
                                              conjugations,
                                              conjugationRule,
                                              ref conjData))
                {
                    context.AddVerbConjugationRule(verb, conjugationRule, conjData);
                    if (ignorePersonsIds.Count == tense.PersonsCount ||
                        IsVerbRegular(verb, tense, conjugations, ref conjData))
                    {
                        break;
                    }
                }

                conjugationRulesCounter++;
            }
        }

        private bool IsConjugatorAppliedToVerb(Verb verb,
                                               List<string> conjugations,
                                               ConjugationRule conjugationRule,
                                               ref string conjData)
        {
            List<ConjugationRulesInstruction> instructions = null;
            try
            {
                instructions = context.VerbConjugationInstructions.Where(vci => (vci.ConjugationRuleId == conjugationRule.Id) &&
                                                                                (vci.VerbType == verb.Type)).ToList();
            }
            catch { }

            if (instructions == null || !instructions.Any())
                return false;

            var conjugationRulePersons = context.GetConjugationRulePersons(conjugationRule);
            var persons = conjugationRulePersons.Where(p => !ignorePersonsIds.Contains(p.Id)).ToList();

            return IsConjugationRuleAppliedToConjugations(verb,
                                                          conjugations,
                                                          instructions,
                                                          conjugationRule,
                                                          persons,
                                                          ref conjData);
        }

        private bool IsConjugationRuleAppliedToConjugations(Verb verb,
                                                            List<string> conjugations,
                                                            List<ConjugationRulesInstruction> instructions,
                                                            ConjugationRule conjugationRule,
                                                            List<Person> persons,
                                                            ref string conjData)
        {
            var conjugationRuleType = conjugationRule.Type;
            string verbPattern = null;
            string newPattern = null;
            List<int> affectedPersonsIds = new List<int>();

            var isSpecialconjugationRule = (conjugationRuleType == ConjugationRuleType.SpecialConjugation);

            foreach (var person in persons)
            {
                var personIndex = person.GetIndex();
                var conjugation = conjugations[personIndex];
                var instruction = instructions.FirstOrDefault(i => i.PersonId == person.Id);
                var suffix = instruction.Suffix;
                var suffixMatch = conjugation.EndsWith(suffix);

                if (!suffixMatch)
                    return false;
                else if (!isSpecialconjugationRule)
                {
                    newPattern = conjugation.Remove(conjugation.LastIndexOf(suffix));

                    if (string.IsNullOrEmpty(verbPattern))
                        verbPattern = newPattern;
                    else if (verbPattern != newPattern)
                        return false;
                }

                affectedPersonsIds.Add(person.Id);
            }            

            if (isSpecialconjugationRule ||
                                   IsPatternValid(verb,
                                   verbPattern,
                                   conjugationRuleType))
            {
                conjData = verbPattern;
                ignorePersonsIds.AddRange(affectedPersonsIds);
                return true;
            }

            return false;
        }

        private bool IsPatternValid(Verb verb, string pattern, ConjugationRuleType type)
        {
            switch (type)
            {
                case ConjugationRuleType.Independent:
                    return (pattern == verb.Stem) ||
                            (pattern == verb.Infinative);
                case ConjugationRuleType.NewStemDependent:
                    return (pattern != verb.Stem);
                case ConjugationRuleType.NewInfDependent:
                    return (pattern != verb.Infinative);
            }

            return true;
        }

        #endregion
    }
}