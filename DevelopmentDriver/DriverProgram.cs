using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using NotaDAL;
using NotaDAL.Models;
using ConjugationsIngestor;
using NotaConjugator;
using System.Configuration;
using NotaDAL.Context;

namespace DevelopmentDriver
{
    class DriverProgram
    {
        #region Consts

        const string INFINATIVES_FILE_APP_KEY = "INFINATIVES_FILE";

        #endregion

        static void Main(string[] args)
        {
            using (var context = new NotaContextAcces())
            {                
                string presentParticiple, pastParticiple, englishInf;

                var infinatives = readInfinatives();
                var allTenses = context.GetItemList<Tense>();

                foreach (var infinative in infinatives)
                {
                    var tensesConjugations = IngestConjugations(infinative,
                                                      allTenses,
                                                      out englishInf,
                                                      out presentParticiple,
                                                      out pastParticiple);

                    Verb verb = new Verb(infinative, englishInf, "Needs Description");
                    var conjugator = new ConjugationsClassifier(context);

                    verb = context.AddItem<Verb>(verb);

                    conjugator.ClassifyVerbConjugators(verb, tensesConjugations);
                }                
            }
        }   

        private static List<string> readInfinatives()
        {
            var filePath = ConfigurationManager.AppSettings[INFINATIVES_FILE_APP_KEY];
            var infinatives = System.IO.File.ReadAllText(filePath);
            infinatives = infinatives.Replace("\r", string.Empty);

            return infinatives.Split('\n').ToList();
        }

        private static Dictionary<Tense, List<string>> IngestConjugations(string infinative,
                                                                          List<Tense> allTenses,
                                                                          out string englishInf,
                                                                          out string presentParticiple,
                                                                          out string pastParticiple)
        {
            var html = ConjugationsReader.DownloadConjugationsHTML(infinative);

            var parser = new ConjugationParser();
            var conjugations = parser.parseHtml(ref html,
                                                out englishInf,
                                                out presentParticiple,
                                                out pastParticiple,
                                                allTenses);

            return conjugations;

        }
    }
}