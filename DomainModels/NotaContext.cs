using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotaDAL.Models;
using System.Data.Linq.Mapping;
using System.Data.Linq;

namespace NotaDAL
{
    [Database(Name = "MainDB")]
    public class NotaContext : DataContext
    {
        #region Consts

        private const string CONN_STR_APP_KEY = "ConnStr";

        #endregion

        #region Ctors


        public NotaContext() : base(ConfigurationManager.AppSettings[CONN_STR_APP_KEY])
        {
        }

        #endregion

        #region Props

        public Table<ConjugationRule> ConjugationRules;

        public Table<Person> Persons;

        public Table<Tense> Tenses;

        public Table<Verb> Verbs;

        public Table<ConjugationRulePerson> ConjugationRulePersons;

        public Table<ConjugationRulesInstruction> VerbConjugationInstructions;

        public Table<VerbsConjugationRule> VerbsConjugationRules;

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
        
        public VerbsConjugationRule AddVerbConjugationRule(Verb verb, ConjugationRule conjugationRule, string conjData = null)
        {
            var verbConjugationRule = VerbsConjugationRules.FirstOrDefault(vcr => vcr.VerbId == verb.Id && vcr.ConjugationRuleId == conjugationRule.Id)
                ?? new VerbsConjugationRule
                {
                    VerbId = verb.Id,
                    ConjugationRuleId = conjugationRule.Id,
                    ConjugationData = conjData
                };
            // TODO: Make generic Insert function
            if (verbConjugationRule.Id <= 0)
            {
                VerbsConjugationRules.InsertOnSubmit(verbConjugationRule);
                SubmitChanges();
            }

            return verbConjugationRule;
        }

        public Verb AddVerb(Verb verb)
        {
            verb = Verbs.FirstOrDefault(v => v.Infinative == verb.Infinative) ?? verb;


            // TODO: Make generic Insert function
            if (verb.Id <= 0)
            {
                Verbs.InsertOnSubmit(verb);
                SubmitChanges();
            }

            return verb;
        }

        public List<ConjugationRule> GetTenseIrregularConjugationRules(Tense tense)
        {
            if (tense.IrregularConjugationRules == null ||
                !tense.IrregularConjugationRules.Any())
            {
                try
                {
                    tense.IrregularConjugationRules = ConjugationRules.Where(cr => cr.TenseId == tense.Id).ToList();
                }
                catch { }
            }

            return tense.IrregularConjugationRules;
        }

        public ConjugationRule GetTenseRegularConjugationRule(Tense tense)
        {
            if (tense.RegularConjugationRule == null)
            {
                tense.RegularConjugationRule = ConjugationRules.FirstOrDefault
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


                    var personIds = ConjugationRulePersons.Where(crp => crp.ConjugationRuleId == conjugationRule.Id).
                                                                 Select(crp => crp.PersonId).ToList();
                    conjugationRule.Persons = Persons.Where(p => personIds.Contains(p.Id)).ToList();
                }
                catch { }                
            }

            return conjugationRule.Persons;
        }

        #endregion
    }
}
