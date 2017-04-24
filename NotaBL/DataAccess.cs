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

        public static List<VerbInfo> GetVerbs()
        {
            List<VerbInfo> verbs = new List<VerbInfo>();
            using (var context = new NotaContextAcces())
            {
                var dbVerbs = context.GetItemList<Verb>();
                var conjugator = new Conjugator(context);

                foreach (var dbVerb in dbVerbs)
                {
                    var conjugationIndexes = conjugator.ConjugateVerb(dbVerb.Id);
                    var verbConjugations = CastConjugationIndexes(dbVerb.Id, conjugationIndexes);
                    List<int> verbConjugationRulesIds = context.GetVerbConjugationRulesIds(dbVerb);

                    var verb = new VerbInfo(dbVerb.Id,
                                            dbVerb.Description,
                                            dbVerb.Infinative,
                                            dbVerb.EnglishInfinative,
                                            verbConjugations,
                                            verbConjugationRulesIds);

                    verbs.Add(verb);

                    break;
                }
            }

            return verbs;
        }

        public static List<TenseInfo> GetTenses()
        {
            throw new NotImplementedException();
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
