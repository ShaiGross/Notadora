using NotaDAL.Context;
using NotaDAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotaDAL.Context
{
    public class NotaContextAcces : IDisposable
    {
        #region Data Members

        private NotaContext context;

        #endregion

        #region Ctor

        public NotaContextAcces()
        {
            context = new NotaContext();
        }

        #endregion

        #region Methods

        public List<ConjugationRule> GetAllTenseConjugationRules(Tense tense)
        {
            if (tense.IrregularConjugationRules == null ||
                !tense.IrregularConjugationRules.Any())
            {
                if (GetTenseIrregularConjugationRules(tense) == null)
                    return null;
            }

            if (tense.RegularConjugationRule == null)
            {
                if (GetTenseRegularConjugationRule(tense) == null)
                    return null;
            }

            var allTenseConjugationRules = tense.IrregularConjugationRules.Select(ir => ir).ToList();
            allTenseConjugationRules.Add(tense.RegularConjugationRule);

            return allTenseConjugationRules;
        }

        public List<int> GetVerbConjugationRulesIds(Verb dbVerb)
        {
            var query = from rule
                        in context.ConjugationRules
                        join match in context.ConjugationMatches
                        on rule.Id equals match.ConjugationRuleId
                        where match.VerbId == dbVerb.Id
                        select rule.Id;

            return query.ToList();
        }

        public List<Person> GetAllTensePersons(int tenseId)
        {
            var tensePersonIds = context.TensePersons.Where(tp => tp.TenseId == tenseId)
                                                     .Select(tp => tp.PersonId);

            return context.Persons.Where(p => tensePersonIds.Contains(p.Id))
                                  .ToList();
        }                           

        public List<ConjugationRule> GetTenseIrregularConjugationRules(Tense tense)
        {
            if (tense.IrregularConjugationRules == null ||
                !tense.IrregularConjugationRules.Any())
            {
                try
                {
                    tense.IrregularConjugationRules = context.ConjugationRules.Where(cr => (cr.TenseId == tense.Id) &&
                                                                                           (!cr.IsRegular))
                                                                      .ToList();
                }
                catch (Exception e) { }
            }

            return tense.IrregularConjugationRules;
        }

        public ConjugationRulesInstruction GetConjugationInstruction(Verb verb, int conjugationRuleId, int personId)
        {
            return context.VerbConjugationInstructions.First(vci => vci.ConjugationRuleId == conjugationRuleId &&
                                                                    vci.VerbType == verb.Type &&
                                                                    vci.PersonId == personId);
        }

        public ConjugationRule GetTenseRegularConjugationRule(Tense tense)
        {
            if (tense.RegularConjugationRule == null)
            {
                tense.RegularConjugationRule = context.ConjugationRules.FirstOrDefault
                    (cr => cr.Id == tense.RugularConjugationRuleId);
            }


            return tense.RegularConjugationRule;
        }

        public List<Person> GetConjugationRulePersons(ConjugationRule conjugationRule)
        {
            if (conjugationRule.Persons == null ||
                !conjugationRule.Persons.Any())
            {
                try
                {
                    var personIds = context.ConjugationRulePersons.Where(crp => crp.ConjugationRuleId == conjugationRule.Id).
                                                                 Select(crp => crp.PersonId).ToList();
                    conjugationRule.Persons = context.Persons.Where(p => personIds.Contains(p.Id)).ToList();
                }
                catch { }
            }

            return conjugationRule.Persons;
        }

        public ConjugationMatch getConjugationMatch(int tenseId,
                                                    int verbId,
                                                    int personId)
        {
            var conjMatches = context.ConjugationMatches.ToList();

            var query = from match
                        in context.ConjugationMatches
                        join rule in context.ConjugationRules
                        on match.ConjugationRuleId equals rule.Id
                        where rule.TenseId == tenseId &&
                              match.VerbId == verbId &&
                              (match.PersonId == personId || match.PersonId == null)
                        select match;

            if (!query.Any())
                return null;

            return query.FirstOrDefault();
        }

        public ConjugationRule getConjugationMatchConjugationRule(ConjugationMatch conjugationMatch)
        {
            if (conjugationMatch.ConjugationRule == null)
                conjugationMatch.ConjugationRule = GetItem<ConjugationRule>(conjugationMatch.ConjugationRuleId);

            return conjugationMatch.ConjugationRule;
        }

        #endregion

        #region IDisposable Implementation

        public void Dispose()
        {
            context.Dispose();
        }

        #endregion

        #region Generic NotaDbObj Methods

        public T GetItem<T>(int id) where T : NotaDbObject<T>
        {
            return GetItemList<T>().FirstOrDefault(item => item.Id == id);
        }

        public List<T> GetItemList<T>()
        {
            var table = context.GetTable(typeof(T));

            return table.Cast<T>()
                        .Select(item => item)
                        .ToList();
        }


        public List<T> GetItemList<T>(Func<T, bool> predicate)
        {
            var table = context.GetTable(typeof(T));
            var allItems = GetItemList<T>();

            return allItems.Where<T>(predicate)
                           .ToList();
        }

        public T AddItem<T>(T item, bool submit = true) where T : NotaDbObject<T>
        {
            var itemsList = GetItemList<T>();
            var insertedItem = itemsList.FirstOrDefault(dbItem => dbItem.DbCompare(item));

            if (insertedItem != null)
                item = insertedItem;
            else
            {
                var table = context.GetTable(typeof(T));
                table.InsertOnSubmit(item);

                if (submit)
                    context.SubmitChanges();
            }

            return item;
        }

        public ConjugationMatch CreateConjugationMatch(int VerbId, int conjugationRuleId, int? personId, string ConjugationString)
        {
            return new ConjugationMatch
            {
                VerbId = VerbId,
                ConjugationRuleId = conjugationRuleId,
                PersonId = personId,
                ConjugationString = ConjugationString
            };
        }

        public List<T> AddItems<T>(List<T> items) where T : NotaDbObject<T>
        {
            var insertedItems = items.Select(item => AddItem<T>(item, false)).ToList();
            context.SubmitChanges();
            return insertedItems;
        }

        #endregion
    }
}
