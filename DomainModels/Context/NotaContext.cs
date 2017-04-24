using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotaDAL.Models;
using System.Data.Linq.Mapping;
using System.Data.Linq;

namespace NotaDAL.Context
{
    [Database(Name = "MainDB")]
    internal class NotaContext : DataContext
    {
        #region Consts

        private const string CONN_STR_APP_KEY = "CONN_STR";

        #endregion

        #region Ctors


        public NotaContext() : base(ConfigurationManager.AppSettings[CONN_STR_APP_KEY])
        {
        }

        #endregion

        #region Properties

        public Table<ConjugationRule> ConjugationRules = null;

        public Table<Person> Persons = null;

        public Table<Tense> Tenses = null;

        public Table<Verb> Verbs = null;

        public Table<ConjugationRulePerson> ConjugationRulePersons = null;

        public Table<ConjugationRulesInstruction> VerbConjugationInstructions = null;

        public Table<ConjugationMatch> ConjugationMatches = null;

        public Table<TensePerson> TensePersons = null;

        #endregion        
    }
}
