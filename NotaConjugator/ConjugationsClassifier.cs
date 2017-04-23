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

        private NotaContextAcces    context;
        private List<int>           ignorePersonsIds = new List<int>();
        private string              conjugationString;
        private Tense               tense;
        private ConjugationRule     conjugationRule;
        private Verb                verb;

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
            this.verb = verb;
            sortTensesConjugations(ref tensesConjugations);

            foreach (var tenseConjugations in tensesConjugations)
            {
                ignorePersonsIds.Clear();
                tense = tenseConjugations.Key;
                var regularConjugationRule = context.GetTenseRegularConjugationRule(tense);
                var IrregularConjugationRules = context.GetTenseIrregularConjugationRules(tense);

                if (!IsVerbRegular(tenseConjugations.Value))
                {
                    ClassifyVerbIrregularConjugationRules(IrregularConjugationRules,
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

        private bool IsVerbRegular(List<string> tenseConjugations)
        {
            conjugationRule = tense.RegularConjugationRule;
            return IsConjugatorAppliedToVerb(tenseConjugations);
        }

        private void ClassifyVerbIrregularConjugationRules(List<ConjugationRule> conjugationRules, // TODO: Remove This and get from tense
                                                           List<string> conjugations)
        {
            ignorePersonsIds = new List<int>();
            conjugationRules.Sort();
            var conjugationRulesCounter = 0;

            foreach (var irregConjugationRule in conjugationRules)
            {
                conjugationRule = irregConjugationRule;
                if (IsConjugatorAppliedToVerb(conjugations))
                {
                    if (ignorePersonsIds.Count == tense.PersonsCount ||
                        IsVerbRegular(conjugations))
                    {
                        break;
                    }
                }

                conjugationRulesCounter++;
            }
        }

        private bool IsConjugatorAppliedToVerb(List<string> conjugations)
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

            return IsConjugationRuleAppliedToConjugations(conjugations,
                                                          instructions,
                                                          persons);
        }

        private bool IsConjugationRuleAppliedToConjugations(List<string> conjugations,
                                                            List<ConjugationRulesInstruction> instructions,
                                                            List<Person> persons)
        {
            List<int> affectedPersonsIds = new List<int>();
            conjugationString = null;
            string inferredPattern = null;
            var conjugationRuleType = conjugationRule.Type;

            if ((persons == null) || (!persons.Any()) ||
                (conjugations == null) || (!conjugations.Any()))
            {
                return false;
            }

            foreach (var person in persons)
            {
                var personIndex = person.Index;
                var conjugation = conjugations[personIndex];
                var instruction = instructions.FirstOrDefault(i => i.PersonId == person.Id);
                var suffix = instruction.Suffix;

                if (!conjugation.DiacriticsEndsWith(suffix))
                    return false;

                var suffixIndex = conjugation.DiacriticsLastIndexOf(suffix);

                var newPattern = getInferredPattern(conjugation, suffixIndex);

                if (!IsParrtnValid(newPattern, ref inferredPattern))
                    return false;

                affectedPersonsIds.Add(person.Id);
            }

            if (conjugationRule.Type != ConjugationRuleType.Independent)          
                conjugationString = inferredPattern;

            if (affectedPersonsIds.Count == tense.PersonsCount)
            {
                var verbConjugationMatch = context.CreateConjugationMatch(verb.Id, conjugationRule.Id, null, conjugationString);
                context.AddItem<ConjugationMatch>(verbConjugationMatch);
            }
            else
            {
                if (AddPersonsConjugationMatch(verb,
                                               conjugationRule,
                                               conjugations,
                                               persons,
                                               affectedPersonsIds) == null)
                {
                    throw new Exception("Failed To Add ConjugationMatch");
                }
            }

            Console.WriteLine($"{verb.Infinative} Applies { conjugationRule.Name}");                

            return true;
        }

        private string getInferredPattern(string conjugation, int suffixIndex)
        {
            //(conjugationRuleType == ConjugationRuleType.SpecialConjugation) ? conjugation : conjugation.Remove(suffixIndex);
            if (conjugationRule.Type == ConjugationRuleType.SpecialConjugation)
            {
                return conjugation;
            }
            else
            {
                return conjugation.Remove(suffixIndex);
            }
        }

        private List<ConjugationMatch> AddPersonsConjugationMatch(Verb verb,
                                                           ConjugationRule conjugationRule,
                                                           List<string> conjugations,
                                                           List<Person> persons,
                                                           List<int> affectedPersonsIds)
        {
            var conjugationMatches = new List<ConjugationMatch>();
            ignorePersonsIds.AddRange(affectedPersonsIds);

            var affectedPersons = persons.Where(p => affectedPersonsIds.Contains(p.Id))
                                            .ToList();
            string conjString;

            foreach (var person in persons)
            {
                if (conjugationRule.Type == ConjugationRuleType.SpecialConjugation)
                    conjString = conjugations[person.Index];
                else
                    conjString = conjugationString;

                var conjugationMatch = context.CreateConjugationMatch(verb.Id,
                                                                    conjugationRule.Id,
                                                                    person.Id,
                                                                    conjString);
                conjugationMatches.Add(conjugationMatch);
            }

            return context.AddItems<ConjugationMatch>(conjugationMatches);
        }

        private bool IsParrtnValid(string newPattern,
                                   ref string oldPattern)
        {
            
            if (conjugationRule.PatternType == ConjugationPatternType.None)
                return true;
            else if (string.IsNullOrEmpty(oldPattern))
                oldPattern = newPattern;
            else if (oldPattern != newPattern)
                return false;

            var expectedPattern = ConjugationUtils.getConjugationMatchPattern(verb, conjugationRule);
            var independentPattern = (newPattern == expectedPattern);

            switch (conjugationRule.Type)
            {
                case ConjugationRuleType.Independent:
                    return independentPattern;
                case ConjugationRuleType.NewPatternDependent:
                    return !independentPattern;                                    
                default:
                    throw new Exception("Unexpeted Verb Type");
            }
        }

        #endregion
    }
}