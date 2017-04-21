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

        //public VerbsConjugationRule CreateVerbConjugationRule(Verb verb, ConjugationRule conjugationRule, string conjData)
        //{
        //    return new VerbsConjugationRule
        //    {
        //        VerbId = verb.Id,
        //        ConjugationRuleId = conjugationRule.Id,
        //        ConjugationData = conjData
        //    };
        //}        

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
                catch { }
            }

            return tense.IrregularConjugationRules;
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

        public ConjugationMatch getConjugationMatch(int TenseId, 
                                                    int VerbId, 
                                                    int PersonId)
        {
            return context.ConjugationMatches
                          .First(cm => cm.VerbId == VerbId &&
                                       getConjugationRuleByMatch(cm).TenseId == TenseId &&
                                       (cm.PersonId == PersonId || cm.PersonId == null));
        }

        public ConjugationRule getConjugationRuleByMatch(ConjugationMatch match)
        {
            return GetItem<ConjugationRule>(match.ConjugationRuleId);
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

        public List<T> AddItems<T>(List<T> items) where T  : NotaDbObject<T>
        {
            var insertedItems = items.Select(item => AddItem<T>(item, false)).ToList();
            context.SubmitChanges();
            return insertedItems;
        }

        #endregion
    }
}
