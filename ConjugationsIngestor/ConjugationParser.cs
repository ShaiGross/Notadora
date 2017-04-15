using NotaDAL.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConjugationsIngestor
{
    public class ConjugationParser
    {
        #region Consts

        private const string MAX_PERSON_PER_TENSE_APP_KEY = "MAX_PERSON_PER_KEY";

        #endregion

        #region Methods

        public Dictionary<Tense, List<string>> parseHtml(ref string html,
                                                         out string englishInf,
                                                         out string presentParticiple,
                                                         out string pastParticiple,                                                         
                                                         List<Tense> allTenses)
        {
            englishInf = parseEnglishInfinative(ref html);
            presentParticiple = parsePresentParticiple(ref html);
            pastParticiple = parsePastParticiple(ref html);
            
            var tensesConjugations = sortTenseList(allTenses);

            parseTenses(html, tensesConjugations);

            return tensesConjugations;
        }

        private string parseEnglishInfinative(ref string html)
        {
            var englishInfLeftIdentifier = "<div class=\"el\">";
            var englishInfRightIdentifier = "</div>";

            var englishInfLeftIndex = html.IndexOf(englishInfLeftIdentifier) + englishInfLeftIdentifier.Length;
            html = html.Remove(0, englishInfLeftIndex);

            var englishInfRightIndex = html.IndexOf(englishInfRightIdentifier);

            var LinksHtml = html.Substring(0, englishInfRightIndex);
            var inEnglishInf = true;
            var englishInf = string.Empty;

            foreach (var currChar in LinksHtml)
            {
                if (currChar == '<')
                    inEnglishInf = false;
                else if (currChar == '>')
                    inEnglishInf = true;
                else if (inEnglishInf)
                    englishInf += currChar;
            }

            var comaIndex = englishInf.IndexOf(',');
            if (comaIndex > 0)
                englishInf = englishInf.Remove(comaIndex);

            return englishInf.TrimEnd();
        }

        private Dictionary<Tense, List<string>> sortTenseList(List<Tense> unsortedTenses)
        {
            var sortedTenses = new Dictionary<Tense, List<string>>(unsortedTenses.Count);
            Tense currTense;

            try
            {                
                currTense = unsortedTenses.First(t => t.Name == "Present");
                sortedTenses.Add(currTense, 
                                 new List<string>(currTense.PersonsCount));

                currTense = unsortedTenses.First(t => t.Name == "Preterite");
                sortedTenses.Add(currTense, 
                                 new List<string>(currTense.PersonsCount));

                currTense = unsortedTenses.First(t => t.Name == "Imperfect");
                sortedTenses.Add(currTense, 
                                 new List<string>(currTense.PersonsCount));

                currTense = unsortedTenses.First(t => t.Name == "Future");
                sortedTenses.Add(currTense, 
                                 new List<string>(currTense.PersonsCount));

                currTense = unsortedTenses.First(t => t.Name == "Conditional");
                sortedTenses.Add(currTense, 
                                 new List<string>(currTense.PersonsCount));

            }
            catch
            {
                return null;
            }
            
            return sortedTenses;
        }

        private static string parsePresentParticiple(ref string html)
        {
            var gerundElement = "Gerund:</a>";
            var gerundIndex = html.IndexOf(gerundElement);
            html = html.Remove(0, gerundIndex + gerundElement.Length);

            var presentParticipleLeft = html.IndexOf("<span>") + "<span>&nbsp;".Length;
            var presentParticipleRight = html.IndexOf("</span>");

            var presentParticiple = html.Substring(presentParticipleLeft, presentParticipleRight - presentParticipleLeft);

            if (presentParticiple.Contains(','))
                presentParticiple = presentParticiple.Remove(presentParticiple.IndexOf(','));

            fixAccentedLatters(ref presentParticiple);

            return presentParticiple;
        }

        private static string parsePastParticiple(ref string html)
        {
            var participleElement = "Participle:</a>";
            var gerundIndex = html.IndexOf(participleElement);
            html = html.Remove(0, gerundIndex + participleElement.Length);

            var pastParticipleLeft = html.IndexOf("<span>") + "<span>&nbsp;".Length;
            var pastParticipleRigth = html.IndexOf("</span>");

            var pastParticiple = html.Substring(pastParticipleLeft, pastParticipleRigth - pastParticipleLeft);

            if (pastParticiple.Contains(','))
                pastParticiple = pastParticiple.Remove(pastParticiple.IndexOf(','));

            fixAccentedLatters(ref pastParticiple);

            return pastParticiple;
        }

        private static void parseTenses(string html, Dictionary<Tense, List<string>> tensesConjugations)
        {
            // TODO: Fix this into config libreary
            var maxPersonPerTenses = int.Parse(ConfigurationManager.AppSettings[MAX_PERSON_PER_TENSE_APP_KEY]);
            var tableElement = "<table class=\"vtable\">";
            var tableIndex = html.IndexOf(tableElement);
            html = html.Remove(0, tableIndex + tableElement.Length);

            var tableLeft = html.IndexOf("</tr><tr>") + "</tr>".Length;
            var tableRigth = html.IndexOf("</table>");

            html = html.Substring(tableLeft, tableRigth - tableLeft).Replace(" ", "");
            var tenses = tensesConjugations.Keys.ToList();

            for (int trIndex = 0; trIndex < maxPersonPerTenses; trIndex++)
            {
                var firstTdLeft = html.IndexOf("<td");
                var firstTdRight = html.IndexOf("</td>") + "</td>".Length;
                html = html.Remove(firstTdLeft, firstTdRight - firstTdLeft);
                html = html.Replace("<spanclass='conj-irregular'>", "").Replace("</span>", "");                

                for (int tenseIndex = 0; tenseIndex < tensesConjugations.Count; tenseIndex++)
                {
                    var currTense = tenses[tenseIndex];
                    var tdLeft = html.IndexOf("\">") + "\">".Length;
                    var tdRight = html.IndexOf("</td>");
                    var conjugatedGrammPerson = html.Substring(tdLeft, tdRight - tdLeft);

                    if (conjugatedGrammPerson.Contains('>'))
                        conjugatedGrammPerson = conjugatedGrammPerson.Substring(conjugatedGrammPerson.IndexOf('>') + 1);

                    conjugatedGrammPerson = conjugatedGrammPerson.Trim();
                    fixAccentedLatters(ref conjugatedGrammPerson);

                    if (tenseIndex + 1 != tensesConjugations.Count || trIndex != 5)

                        html = html.Remove(tdLeft, tdRight - tdLeft + "<tdclass=\"vtable-word\"></td>".Length);

                    tensesConjugations[currTense].Add(conjugatedGrammPerson);
                }
            }

            foreach (var currTense in tensesConjugations.Keys)
            {
                tensesConjugations[currTense][4] = tensesConjugations[currTense][5];
            }
        }

        private static void fixAccentedLatters(ref string word)
        {
            word = (word.Replace("Ã¡", "á").
                         Replace("Ã©", "é").
                         Replace("Ã­", "í").
                         Replace("Ã³", "ó").
                         Replace("Ãº", "ú").
                         Replace("Ã±", "ñ"));
        }

        #endregion
    }
}
