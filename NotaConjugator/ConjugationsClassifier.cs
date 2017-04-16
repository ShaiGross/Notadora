using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotaDAL;
using NotaDAL.Models;
using StringUtils;
using NotaDAL.Context;

namespace NotaConjugator
{
    public class ConjugationsClassifier
    {
        #region Data Members

        private NotaContextAcces context;
        private List<int> ignorePersonsIds = new List<int>();
        private string conjugationString;

        #endregion

        #region Ctors

        public ConjugationsClassifier(NotaContextAcces dataContext)
        {
            context = dataContext;
        }

        #endregion

        #region Methods

        public void ClassifyVerbConjugators(Verb verb, Dictionary<Tense, List<string>> tensesConjugations)
        {

            sortTensesConjugations(ref tensesConjugations);


            foreach (var tenseConjugations in tensesConjugations)
            {
                ignorePersonsIds.Clear();
                var tense = tenseConjugations.Key;
                var regularConjugationRule = context.GetTenseRegularConjugationRule(tense);
                var IrregularConjugationRules = context.GetTenseIrregularConjugationRules(tense);

                if (IsVerbRegular(verb, tense, tenseConjugations.Value))
                {
                    var verbConjugationRule = context.CreateVerbConjugationRule(verb,
                                                                                regularConjugationRule,
                                                                                conjugationString);
                    context.AddItem<VerbsConjugationRule>(verbConjugationRule);
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
                                   List<string> tenseConjugations)
        {
            return IsConjugatorAppliedToVerb(verb,
                                             tenseConjugations,
                                             tense.RegularConjugationRule);
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
                                              conjugationRule))
                {
                    var verbConjugationRule = context.CreateVerbConjugationRule(verb,
                                                                                conjugationRule,
                                                                                conjData);
                    context.AddItem<VerbsConjugationRule>(verbConjugationRule);

                    if (ignorePersonsIds.Count == tense.PersonsCount ||
                        IsVerbRegular(verb, tense, conjugations))
                    {
                        break;
                    }
                }

                conjugationRulesCounter++;
            }
        }

        private bool IsConjugatorAppliedToVerb(Verb verb,
                                               List<string> conjugations,
                                               ConjugationRule conjugationRule)
        {
            List<ConjugationRulesInstruction> instructions = null;
            try
            {
                instructions = context.GetItemList<ConjugationRulesInstruction>()
                                      .Where(vci => (vci.ConjugationRuleId == conjugationRule.Id) &&
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
                                                          persons);
        }

        private bool IsConjugationRuleAppliedToConjugations(Verb verb,
                                                            List<string> conjugations,
                                                            List<ConjugationRulesInstruction> instructions,
                                                            ConjugationRule conjugationRule,
                                                            List<Person> persons)
        {
            List<int> affectedPersonsIds = new List<int>();
            conjugationString = null;
            string verbPattern = null;
            var conjugationRuleType = conjugationRule.Type;

            if ((persons == null) || (!persons.Any()) ||
                (conjugations == null) || (!conjugations.Any()))
            {
                return false;
            }

            foreach (var person in persons)
            {
                var personIndex = person.GetIndex();
                var conjugation = conjugations[personIndex];
                var instruction = instructions.FirstOrDefault(i => i.PersonId == person.Id);
                var suffix = instruction.Suffix;

                if (!conjugation.DiacriticsEndsWith(suffix))
                    return false;

                var suffixIndex = conjugation.DiacriticsLastIndexOf(suffix);

                var newPattern = (conjugationRuleType == ConjugationRuleType.SpecialConjugation) ? conjugation : conjugation.Remove(suffixIndex);

                if (!IsParrtnValid(verb, conjugationRuleType, newPattern, ref verbPattern))
                    return false;

                affectedPersonsIds.Add(person.Id);
            }

            if (conjugationRuleType != ConjugationRuleType.Independent)
                conjugationString = verbPattern;

            Console.WriteLine($"{verb.Infinative} Applies { conjugationRule.Name}");

            ignorePersonsIds.AddRange(affectedPersonsIds);

            var conjugationMatches = affectedPersonsIds
                                     .Select(id => context.CreateConjugationMatch(verb.Id, 
                                                                                  conjugationRule.Id,
                                                                                  id,
                                                                                  conjugationString))
                                     .ToList();

            if (context.AddItems<ConjugationMatch>(conjugationMatches) == null)
                throw new Exception("Failed To Add ConjugationMatch");

            return true;
        }

        private bool IsParrtnValid(Verb verb,
                                   ConjugationRuleType conjugationRuleType,
                                   string newPattern,
                                   ref string oldPattern)
        {
            if (conjugationRuleType == ConjugationRuleType.SpecialConjugation)
            {
                if (!string.IsNullOrEmpty(oldPattern))
                    oldPattern += ";";

                oldPattern += $"{newPattern}";
                return true;
            }

            else if (string.IsNullOrEmpty(oldPattern))
                oldPattern = newPattern;
            else if (oldPattern != newPattern)
                return false;

            switch (conjugationRuleType)
            {
                case ConjugationRuleType.Independent:
                    return newPattern == verb.Stem || newPattern == verb.Infinative;
                case ConjugationRuleType.NewStemDependent:
                    return (newPattern != verb.Stem);
                case ConjugationRuleType.NewInfDependent:
                    return (newPattern != verb.Infinative);
                default:
                    throw new Exception("Unexpeted Verb Type");
            }
        }

        #endregion
    }
}