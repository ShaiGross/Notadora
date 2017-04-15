using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using NotaDAL;
using NotaDAL.Models;
using System.Data.Entity;
using ConjugationsIngestor;
using NotaConjugator;

namespace DevelopmentDriver
{
    class DriverProgram
    {
        static void Main(string[] args)
        {
            using (var context = new NotaContext())
            {
                var infinative = "oír";
                string presentParticiple;
                string pastParticiple;
                string englishInf;
                var allTenses = context.Tenses.ToList();
                var tensesConjugations = IngestConjugations(infinative,
                                                      allTenses,
                                                      out englishInf,
                                                      out presentParticiple,
                                                      out pastParticiple);

                Verb verb = new Verb(infinative, englishInf, "Needs Description");
                var conjugator = new Conjugator(context);

                verb = context.AddVerb(verb);

               conjugator.ClassifyVerbConjugators(verb, tensesConjugations);
            }
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