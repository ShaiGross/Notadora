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

        public static Dictionary<int, VerbInfo> GetVerbs()
        {
            var verbs = new Dictionary<int, VerbInfo>();
            using (var context = new NotaContextAcces())
            {
                var dbVerbs = context.GetItemList<Verb>();
                var conjugator = new Conjugator(context);

                foreach (var dbVerb in dbVerbs)
                {
                    var conjugationIndexes = conjugator.ConjugateVerb(dbVerb.Id);
                    var verbConjugations = CastConjugationIndexes(dbVerb.Id, conjugationIndexes);
                    var verbConjugationRulesIds = context.GetVerbConjugationRulesIds(dbVerb);

                    var verb = new VerbInfo(dbVerb.Id,
                                            dbVerb.Description,
                                            dbVerb.Infinative,
                                            dbVerb.EnglishInfinative,
                                            verbConjugations,
                                            verbConjugationRulesIds);

                    verbs.Add(dbVerb.Id, verb);                    
                }
            }

            return verbs;
        }

        public static Dictionary<int, ConjugationRuleInfo> GetConjugationRules()
        {
            var conjugationRules = new Dictionary<int, ConjugationRuleInfo>();

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

                    conjugationRules.Add(dbConjugationRule.Id, conjugationRule);                    
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

        public static Dictionary<int, TenseInfo> GetTenses()
        {
            var tenses = new Dictionary<int, TenseInfo>();
            using (var context = new NotaContextAcces())
            {
                var dbTenses = context.GetItemList<Tense>();
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
                    tenses.Add(dbTense.Id, tense);                    
                }
            }

            return tenses;
        }

        #endregion

        #region Casting Methods

        private static List<VerbConjugations> CastConjugationIndexes(int verbId, List<ConjugationIndex> conjugationIndexes)
        {
            var verbConjugations = new List<VerbConjugations>(conjugationIndexes.Count);

            foreach (var conjugationIndex in conjugationIndexes)
            {
                var personId = conjugationIndex.PersonId;
                var tenseId = conjugationIndex.TenseId;
                var conjugation = conjugationIndex.conjugationString;

                var verbConjugation = new VerbConjugations(personId,
                                                           tenseId,
                                                           conjugation);
                verbConjugations.Add(verbConjugation);
            }

            return verbConjugations;
        }

        #endregion
    }
}
