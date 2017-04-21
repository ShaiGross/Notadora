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
        const string BAD_INFINATIVES_FILE_APP_KEY = "BAD_INFINATIVES_FILE";

        #endregion        

        static void Main(string[] args)
        {
            using (var context = new NotaContextAcces())
            {
                //RunIngestionAndClassification(context);
                RunConjugation(context);
            }
        }

        private static void RunIngestionAndClassification(NotaContextAcces context)
        {
            string presentParticiple, pastParticiple, englishInf;

            var badInfinatives = readBadInfinatives();
            var infinatives = readInfinatives().Where(i => !badInfinatives.Contains(i));
            var allTenses = context.GetItemList<Tense>();

            var classifier = new ConjugationsClassifier(context);

            foreach (var infinative in infinatives)
            {
                var tensesConjugations = IngestConjugations(infinative,
                                                  allTenses,
                                                  out englishInf,
                                                  out presentParticiple,
                                                  out pastParticiple);

                Verb verb = new Verb(infinative, englishInf, "Needs Description");
                verb = context.AddItem<Verb>(verb);

                classifier.ClassifyVerbConjugators(verb, tensesConjugations);

                System.Threading.Thread.Sleep(2000);
            }
        }

        private static void RunConjugation(NotaContextAcces context)
        {
            var conjugator = new Conjugator(context);
            var verbsList = context.GetItemList<Verb>(v => v.Infinative == "ser");
            var tense = context.GetItemList<Tense>(t => t.Name == "Present").First();

            var personId = 8;

            foreach (var verb in verbsList)
            {
                var conjugatedWord = conjugator.Conjugate(tense.Id, verb.Id, personId);
            }
        }

        private static List<string> readInfinatives()
        {
            var filePath = ConfigurationManager.AppSettings[INFINATIVES_FILE_APP_KEY];
            var infinatives = System.IO.File.ReadAllText(filePath);
            infinatives = infinatives.Replace("\r", string.Empty);

            return infinatives.Split('\n').ToList();
        }

        private static List<string> readBadInfinatives()
        {
            var filePath = ConfigurationManager.AppSettings[BAD_INFINATIVES_FILE_APP_KEY];
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