using APIModels;
using NotaConjugator;
using NotaConjugator.Data;
using NotaDAL.Context;
using NotaDAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotaBL
{
    public static class DataAccess
    {
        #region Methods

        public static List<VerbInfo> GetVerbs(bool conjugate = false)
        {
            var verbs = new List<VerbInfo>();
            VerbInfo verb = null;

            using (var context = new NotaContextAcces())
            {
                var dbVerbs = context.GetItemList<Verb>();

                foreach (var dbVerb in dbVerbs)
                {
                    if (!conjugate)
                        verb = new VerbInfo(dbVerb.Id, dbVerb.Description, dbVerb.Infinative, dbVerb.EnglishInfinative);
                    else
                        verb = createVerbWithConjugations(dbVerb, context);

                    verbs.Add(verb);
                }
            }

            return verbs;
        }

        private static VerbInfo createVerbWithConjugations(Verb dbVerb, NotaContextAcces context)
        {
            var conjugator = new Conjugator(context);
            var conjugationIndexes = conjugator.ConjugateVerb(dbVerb.Id, true);
            var verbConjugations = CastAndSortConjugationIndexes(dbVerb.Id, conjugationIndexes);
            var verbConjugationRulesIds = context.GetVerbConjugationRulesIds(dbVerb);

            return new VerbInfo(dbVerb.Id,
                                dbVerb.Description,
                                dbVerb.Infinative,
                                dbVerb.EnglishInfinative,
                                verbConjugations,
                                verbConjugationRulesIds);
        }

        public static VerbInfo GetVerbById(int verbId)
        {
            using (var context = new NotaContextAcces())
            {
                var dbVerb = context.GetItem<Verb>(verbId);
                return createVerbWithConjugations(dbVerb, context);
            }
        }

        public static List<ConjugationRuleInfo> GetConjugationRules()
        {
            var conjugationRules = new List<ConjugationRuleInfo>();

            using (var context = new NotaContextAcces())
            {
                var dbConjugationRules = context.GetItemList<ConjugationRule>();

                foreach (var dbConjugationRule in dbConjugationRules)
                {
                    var conjugationRulePersonsIds = context.GetConjugationRulePersons(dbConjugationRule)
                                                           .Select(p => p.Id)
                                                           .ToList();

                    var conjugationRulesVerbsIds = context.GetConjugationRuleVerbsIds(dbConjugationRule);

                    var conjugationRule = new ConjugationRuleInfo(dbConjugationRule.Id,
                                                                  dbConjugationRule.Name,
                                                                  dbConjugationRule.Description,
                                                                  dbConjugationRule.TenseId,
                                                                  dbConjugationRule.IsRegular,
                                                                  dbConjugationRule.Type,
                                                                  dbConjugationRule.PatternIndex,
                                                                  conjugationRulePersonsIds,
                                                                  conjugationRulesVerbsIds);

                    conjugationRules.Add(conjugationRule);
                }
            }

            return conjugationRules;
        }

        public static Dictionary<int, PersonInfo> GetPersons()
        {
            var persons = new Dictionary<int, PersonInfo>();

            using (var context = new NotaContextAcces())
            {
                var dbPersons = context.GetItemList<Person>();

                foreach (var dbPerson in dbPersons)
                {

                    var person = new PersonInfo(dbPerson.Id,
                                              dbPerson.Description,
                                              dbPerson.SpanishExpression,
                                              dbPerson.Plurality,
                                              dbPerson.Formality,
                                              dbPerson.Gender,
                                              dbPerson.Order);
                    persons.Add(dbPerson.Id, person);
                }
            }

            return persons;
        }

        public static List<TenseInfo> GetTenses()
        {
            var tenses = new List<TenseInfo>();
            using (var context = new NotaContextAcces())
            {
                var dbTenses = context.GetItemList<Tense>(t => t.Enabled);
                var conjugator = new Conjugator(context);

                foreach (var dbTense in dbTenses)
                {
                    var irregularConjugationRulesIds = context.GetTenseIrregularConjugationRules(dbTense)
                                                              .Select(cr => cr.Id)
                                                              .ToList();

                    var tensePersonsIds = context.GetAllTensePersons(dbTense.Id)
                                                 .Select(p => p.Id)
                                                 .ToList();

                    var tense = new TenseInfo(dbTense.Id,
                                              dbTense.Name,
                                              dbTense.Description,
                                              dbTense.RugularConjugationRuleId,
                                              irregularConjugationRulesIds,
                                              tensePersonsIds);
                    tenses.Add(tense);
                }
            }

            return tenses;
        }

        #endregion

        #region Casting Methods

        private static List<VerbConjugations> CastAndSortConjugationIndexes(int verbId, List<ConjugationIndex> conjugationIndexes)
        {
            var verbConjugations = new List<VerbConjugations>(conjugationIndexes.Count);
            conjugationIndexes = conjugationIndexes.OrderBy(ci => ci.TenseId).ToList();

            foreach (var conjugationIndex in conjugationIndexes)
            {
                var personId = conjugationIndex.PersonId;
                var tenseId = conjugationIndex.TenseId;
                var conjugation = conjugationIndex.conjugationString;

                var verbConjugation = new VerbConjugations(tenseId, 
                                                           personId,
                                                           conjugation);
                verbConjugations.Add(verbConjugation);
            }

            return verbConjugations;
        }

        #endregion
    }
}
